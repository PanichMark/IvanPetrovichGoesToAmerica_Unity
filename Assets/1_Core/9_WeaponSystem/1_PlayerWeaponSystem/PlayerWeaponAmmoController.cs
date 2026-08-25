using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public delegate void OnAmmoChangedHandler(AmmoTypes type, int newAmount);

public class PlayerWeaponAmmoController : MonoBehaviour, IJsonSaveLoad
{
	public delegate void ReserveAmmoChangedHandler(AmmoTypes ammoType, int newCount);
	public delegate void MagazineAmmoChangedHandler(PlayerWeaponNames weapon, AmmoTypes ammoType, int newCount);

	public ReserveAmmoChangedHandler OnReserveAmmoChanged;
	public MagazineAmmoChangedHandler OnMagazineAmmoChanged;

	private Dictionary<AmmoTypes, AmmoTypeData> _ammoDictionary = new Dictionary<AmmoTypes, AmmoTypeData>();
	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary => _ammoDictionary;

	private Dictionary<PlayerWeaponNames, WeaponRangedData> _weaponsRangedDictionary = new Dictionary<PlayerWeaponNames, WeaponRangedData>();
	public Dictionary<PlayerWeaponNames, WeaponRangedData> WeaponsRangedDictionary => _weaponsRangedDictionary;

	public void SetNewInitialAmmo(AmmoTypes type, int newAmount)
	{
		if (_ammoDictionary.TryGetValue(type, out var data))
		{
			int clampedAmount = Mathf.Clamp(newAmount, 0, data.AmmoMax);

			if (data.AmmoReserve != clampedAmount)
			{
				data.AmmoReserve = clampedAmount;
				_ammoDictionary[type] = data; 

				OnReserveAmmoChanged?.Invoke(type, data.AmmoReserve);
			}
		}
	}

	public void NotifyReserveAmmoChanged(AmmoTypes type, int newAmount)
	{
		// Этот метод теперь служит "шлюзом" для вызова события извне
		OnReserveAmmoChanged?.Invoke(type, newAmount);
	}

	public void NotifyMagazineAmmoChanged(PlayerWeaponNames weaponType, AmmoTypes ammoType, int newAmount)
	{
		// А этот метод - для нового события магазина
		OnMagazineAmmoChanged?.Invoke(weaponType, ammoType, newAmount);
	}

	public void Initialize()
	{	
		_ammoDictionary[AmmoTypes.Ammo9mm] = new AmmoTypeData { AmmoType = AmmoTypes.Ammo9mm, AmmoMax = 999, AmmoReserve = 100 };
		_ammoDictionary[AmmoTypes.Ammo12gauge] = new AmmoTypeData { AmmoType = AmmoTypes.Ammo12gauge, AmmoMax = 999, AmmoReserve = 30 };
		_ammoDictionary[AmmoTypes.AmmoTranquilizerDart] = new AmmoTypeData { AmmoType = AmmoTypes.AmmoTranquilizerDart, AmmoMax = 999, AmmoReserve = 10 };

		_weaponsRangedDictionary[PlayerWeaponNames.Revolver] = new WeaponRangedData
		{
			RagnedWeapon = PlayerWeaponNames.Revolver,
			AmmoType = AmmoTypes.Ammo9mm,
			MagazineAmmoMax = 5,
			MagazineAmmoCurrent = 5
		};
		_weaponsRangedDictionary[PlayerWeaponNames.AutoPistol] = new WeaponRangedData
		{
			RagnedWeapon = PlayerWeaponNames.AutoPistol,
			AmmoType = AmmoTypes.Ammo9mm,
			MagazineAmmoMax = 30,
			MagazineAmmoCurrent = 30
		};
		_weaponsRangedDictionary[PlayerWeaponNames.Shotgun] = new WeaponRangedData
		{
			RagnedWeapon = PlayerWeaponNames.Shotgun,
			AmmoType = AmmoTypes.Ammo12gauge,
			MagazineAmmoMax = 2,
			MagazineAmmoCurrent = 2
		};
		_weaponsRangedDictionary[PlayerWeaponNames.Tranquilizer] = new WeaponRangedData
		{
			RagnedWeapon = PlayerWeaponNames.Tranquilizer,
			AmmoType = AmmoTypes.AmmoTranquilizerDart,
			MagazineAmmoMax = 1,
			MagazineAmmoCurrent = 1
		};

		Debug.Log("PlayerResourcesAmmoManager Initialized");
	}

	// В файле PlayerResourcesAmmoManager.cs

	public void AddAmmoToMagazine(AmmoTypes type, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError($"[PlayerResourcesAmmoManager] Попытка добавить в магазин неположительное количество патронов: {amount}.");
			return;
		}

		// Находим все виды оружия, которые используют данный тип патронов,
		// и увеличиваем у них значение MagazineAmmoCurrent.
		foreach (var weaponEntry in _weaponsRangedDictionary)
		{
			if (weaponEntry.Value.AmmoType == type)
			{
				var data = weaponEntry.Value;
				data.MagazineAmmoCurrent = Mathf.Min(data.MagazineAmmoMax, data.MagazineAmmoCurrent + amount);

				// Сохраняем обновленное состояние обратно в словарь
				_weaponsRangedDictionary[weaponEntry.Key] = data;

				// Оповещаем HUD об изменении.
				// Обратите внимание: мы передаем конкретный тип оружия!
				OnMagazineAmmoChanged?.Invoke(weaponEntry.Key, type, data.MagazineAmmoCurrent);
			}
		}
	}

	public void RemoveAmmoFromMagazine(AmmoTypes type, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError($"[PlayerResourcesAmmoManager] Попытка отнять из магазина неположительное количество патронов: {amount}.");
			return;
		}

		// Аналогично, находим оружие и уменьшаем его боезапас
		foreach (var weaponEntry in _weaponsRangedDictionary)
		{
			if (weaponEntry.Value.AmmoType == type)
			{
				var data = weaponEntry.Value;
				data.MagazineAmmoCurrent = Mathf.Max(0, data.MagazineAmmoCurrent - amount);

				_weaponsRangedDictionary[weaponEntry.Key] = data;

				OnMagazineAmmoChanged?.Invoke(weaponEntry.Key, type, data.MagazineAmmoCurrent);
			}
		}
	}

	public void AddAmmoToReserve(AmmoTypes type, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError($"[PlayerResourcesAmmoManager] Попытка добавить в резерв неположительное количество патронов: {amount}.");
			return;
		}
		if (_ammoDictionary.TryGetValue(type, out var data))
		{
			data.AmmoReserve = Mathf.Min(data.AmmoReserve + amount, data.AmmoMax);
			_ammoDictionary[type] = data;
			OnReserveAmmoChanged?.Invoke(type, data.AmmoReserve);
		}
	}

	public void RemoveAmmoFromReserve(AmmoTypes type, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError($"[PlayerResourcesAmmoManager] Попытка отнять из резерва неположительное количество патронов: {amount}.");
			return;
		}
		if (_ammoDictionary.TryGetValue(type, out var data))
		{
			data.AmmoReserve = Mathf.Max(data.AmmoReserve - amount, 0);
			_ammoDictionary[type] = data;
			OnReserveAmmoChanged?.Invoke(type, data.AmmoReserve);
		}
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		List<AmmoTypeData> ammoList = new List<AmmoTypeData>();
		foreach (var kvp in _ammoDictionary)
		{
			AmmoTypeData saveStruct = kvp.Value;
			saveStruct.AmmoType = kvp.Key;
			ammoList.Add(saveStruct);
		}
		data.PlayerWeapons.PlayerAmmo = ammoList;

		List<WeaponRangedData> weaponList = new List<WeaponRangedData>();
		foreach (var kvp in _weaponsRangedDictionary)
		{
			WeaponRangedData saveStruct = kvp.Value;
			saveStruct.RagnedWeapon = kvp.Key;
			weaponList.Add(saveStruct);
		}
		data.PlayerWeapons.UnlockedPlayerRangedWeapons = weaponList;

		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		//Debug.Log("LOAD AMMO");

		yield return null;
	}
}
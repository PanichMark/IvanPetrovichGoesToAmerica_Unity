using System.Collections.Generic;
using UnityEngine;

public delegate void OnAmmoChangedHandler(AmmoTypes type, int newAmount);

public class PlayerResourcesAmmoManager : MonoBehaviour, ISaveLoad
{
	public delegate void ReserveAmmoChangedHandler(AmmoTypes ammoType, int newCount);
	public delegate void MagazineAmmoChangedHandler(WeaponsRangedEnum weapon, AmmoTypes ammoType, int newCount);

	public ReserveAmmoChangedHandler OnReserveAmmoChanged;
	public MagazineAmmoChangedHandler OnMagazineAmmoChanged;

	private Dictionary<AmmoTypes, AmmoTypeData> _ammoDictionary = new Dictionary<AmmoTypes, AmmoTypeData>();
	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary => _ammoDictionary;

	private Dictionary<WeaponsRangedEnum, WeaponRangedData> _weaponsRangedDictionary = new Dictionary<WeaponsRangedEnum, WeaponRangedData>();
	public Dictionary<WeaponsRangedEnum, WeaponRangedData> WeaponsRangedDictionary => _weaponsRangedDictionary;

	

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

	public void NotifyMagazineAmmoChanged(WeaponsRangedEnum weaponType, AmmoTypes ammoType, int newAmount)
	{
		// А этот метод - для нового события магазина
		OnMagazineAmmoChanged?.Invoke(weaponType, ammoType, newAmount);
	}

	public void Initialize()
	{	
		_ammoDictionary[AmmoTypes.Ammo9mm] = new AmmoTypeData { AmmoTypeSystem = AmmoTypes.Ammo9mm, AmmoMax = 999, AmmoReserve = 100 };
		_ammoDictionary[AmmoTypes.Ammo12gauge] = new AmmoTypeData { AmmoTypeSystem = AmmoTypes.Ammo12gauge, AmmoMax = 999, AmmoReserve = 30 };
	
		_weaponsRangedDictionary[WeaponsRangedEnum.HarmonicaRevolver] = new WeaponRangedData
		{
			RagnedWeaponSystem = WeaponsRangedEnum.HarmonicaRevolver,
			AmmoTypeSystem = AmmoTypes.Ammo9mm,
			MagazineAmmoMax = 5,
			MagazineAmmoCurrent = 5
		};
		_weaponsRangedDictionary[WeaponsRangedEnum.BergmannBayard] = new WeaponRangedData
		{
			RagnedWeaponSystem = WeaponsRangedEnum.BergmannBayard,
			AmmoTypeSystem = AmmoTypes.Ammo9mm,
			MagazineAmmoMax = 30,
			MagazineAmmoCurrent = 30
		};
		_weaponsRangedDictionary[WeaponsRangedEnum.SawedOffShotgun] = new WeaponRangedData
		{
			RagnedWeaponSystem = WeaponsRangedEnum.SawedOffShotgun,
			AmmoTypeSystem = AmmoTypes.Ammo12gauge,
			MagazineAmmoMax = 2,
			MagazineAmmoCurrent = 2
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
			if (weaponEntry.Value.AmmoTypeSystem == type)
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
			if (weaponEntry.Value.AmmoTypeSystem == type)
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

	public void SaveData(ref GameData data)
	{
		List<AmmoTypeData> ammoList = new List<AmmoTypeData>();
		foreach (var kvp in _ammoDictionary)
		{
			AmmoTypeData saveStruct = kvp.Value;
			saveStruct.AmmoTypeJson = kvp.Key.ToString();
			ammoList.Add(saveStruct);
		}
		data.AmmoDictionary = ammoList;

		List<WeaponRangedData> weaponList = new List<WeaponRangedData>();
		foreach (var kvp in _weaponsRangedDictionary)
		{
			WeaponRangedData saveStruct = kvp.Value;
			saveStruct.RagnedWeaponJson = kvp.Key.ToString();
			weaponList.Add(saveStruct);
		}
		data.UnlockedRangedWeapons = weaponList;
	}

	public void LoadData(GameData data)
	{
		/*
		AmmoDictionary.Clear();

		if (data.AmmoDictionary != null)
		{
			foreach (var loadedAmmo in data.AmmoDictionary)
			{
				if (Enum.TryParse(loadedAmmo.AmmoTypeJson, out AmmoTypes parsedType))
				{
					AmmoDictionary[parsedType] = loadedAmmo;
					OnReserveAmmoChanged?.Invoke(parsedType, loadedAmmo.TotalAmmoCurrent);
				}
			}
		}

		WeaponsRangedDictionary.Clear();
		if (data.UnlockedRangedWeapons != null)
		{
			foreach (var loadedWeapon in data.UnlockedRangedWeapons)
			{
				if (Enum.TryParse(loadedWeapon.RagnedWeaponJson, out WeaponRangedEnum parsedWeaponType))
				{
					WeaponsRangedDictionary[parsedWeaponType] = loadedWeapon;
				}
			}
		}
		*/
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnAmmoChangedHandler(AmmoTypes type, int newAmount);

public class PlayerResourcesAmmoManager : MonoBehaviour, ISaveLoad
{
	// Стало:
	// Стало:
	public event System.Action<AmmoTypes, int> OnReserveAmmoChanged;
	public event System.Action<WeaponsRangedEnum, AmmoTypes, int> OnMagazineAmmoChanged;

	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary = new Dictionary<AmmoTypes, AmmoTypeData>();
	public Dictionary<WeaponsRangedEnum, WeaponRangedData> WeaponsRangedDictionary = new Dictionary<WeaponsRangedEnum, WeaponRangedData>();

	public void SetNewInitialAmmo(AmmoTypes type, int newAmount)
	{
		if (AmmoDictionary.TryGetValue(type, out var data))
		{
			int clampedAmount = Mathf.Clamp(newAmount, 0, data.TotalAmmoMax);

			if (data.TotalAmmoCurrent != clampedAmount)
			{
				data.TotalAmmoCurrent = clampedAmount;
				AmmoDictionary[type] = data; 

				OnReserveAmmoChanged?.Invoke(type, data.TotalAmmoCurrent);
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
		AmmoDictionary[AmmoTypes.Ammo9mm] = new AmmoTypeData { AmmoTypeSystem = AmmoTypes.Ammo9mm, TotalAmmoMax = 999, TotalAmmoCurrent = 100 };
		AmmoDictionary[AmmoTypes.Ammo12gauge] = new AmmoTypeData { AmmoTypeSystem = AmmoTypes.Ammo12gauge, TotalAmmoMax = 999, TotalAmmoCurrent = 30 };
	
		WeaponsRangedDictionary[WeaponsRangedEnum.HarmonicaRevolver] = new WeaponRangedData
		{
			RagnedWeaponSystem = WeaponsRangedEnum.HarmonicaRevolver,
			AmmoTypeSystem = AmmoTypes.Ammo9mm,
			MagazineAmmoMax = 5,
			MagazineAmmoCurrent = 5
		};
		WeaponsRangedDictionary[WeaponsRangedEnum.BergmannBayard] = new WeaponRangedData
		{
			RagnedWeaponSystem = WeaponsRangedEnum.BergmannBayard,
			AmmoTypeSystem = AmmoTypes.Ammo9mm,
			MagazineAmmoMax = 30,
			MagazineAmmoCurrent = 30
		};
		WeaponsRangedDictionary[WeaponsRangedEnum.SawedOffShotgun] = new WeaponRangedData
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
		foreach (var weaponEntry in WeaponsRangedDictionary)
		{
			if (weaponEntry.Value.AmmoTypeSystem == type)
			{
				var data = weaponEntry.Value;
				data.MagazineAmmoCurrent = Mathf.Min(data.MagazineAmmoMax, data.MagazineAmmoCurrent + amount);

				// Сохраняем обновленное состояние обратно в словарь
				WeaponsRangedDictionary[weaponEntry.Key] = data;

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
		foreach (var weaponEntry in WeaponsRangedDictionary)
		{
			if (weaponEntry.Value.AmmoTypeSystem == type)
			{
				var data = weaponEntry.Value;
				data.MagazineAmmoCurrent = Mathf.Max(0, data.MagazineAmmoCurrent - amount);

				WeaponsRangedDictionary[weaponEntry.Key] = data;

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
		if (AmmoDictionary.TryGetValue(type, out var data))
		{
			data.TotalAmmoCurrent = Mathf.Min(data.TotalAmmoCurrent + amount, data.TotalAmmoMax);
			AmmoDictionary[type] = data;
			OnReserveAmmoChanged?.Invoke(type, data.TotalAmmoCurrent);
		}
	}

	public void RemoveAmmoFromReserve(AmmoTypes type, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError($"[PlayerResourcesAmmoManager] Попытка отнять из резерва неположительное количество патронов: {amount}.");
			return;
		}
		if (AmmoDictionary.TryGetValue(type, out var data))
		{
			data.TotalAmmoCurrent = Mathf.Max(data.TotalAmmoCurrent - amount, 0);
			AmmoDictionary[type] = data;
			OnReserveAmmoChanged?.Invoke(type, data.TotalAmmoCurrent);
		}
	}

	public void SaveData(ref GameData data)
	{
		List<AmmoTypeData> ammoList = new List<AmmoTypeData>();
		foreach (var kvp in AmmoDictionary)
		{
			AmmoTypeData saveStruct = kvp.Value;
			saveStruct.AmmoTypeJson = kvp.Key.ToString();
			ammoList.Add(saveStruct);
		}
		data.AmmoDictionary = ammoList;

		List<WeaponRangedData> weaponList = new List<WeaponRangedData>();
		foreach (var kvp in WeaponsRangedDictionary)
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
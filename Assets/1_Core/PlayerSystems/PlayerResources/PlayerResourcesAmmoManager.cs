using UnityEngine;
using System.Collections.Generic;

public delegate void OnAmmoChangedHandler(AmmoTypes type, int newAmount);

public class PlayerResourcesAmmoManager : MonoBehaviour, ISaveLoad
{
	public System.Action<AmmoTypes, int> OnReserveAmmoChanged;
	public System.Action<AmmoTypes, int> OnMagazineAmmoChanged;

	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary = new Dictionary<AmmoTypes, AmmoTypeData>();
	public Dictionary<WeaponRangedTypes, WeaponRangedTypeData> WeaponDictionary = new Dictionary<WeaponRangedTypes, WeaponRangedTypeData>();

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

	public void Initialize()
	{
		AmmoDictionary[AmmoTypes.Ammo9mm] = new AmmoTypeData { AmmoType = AmmoTypes.Ammo9mm, TotalAmmoMax = 999, TotalAmmoCurrent = 25 };
		AmmoDictionary[AmmoTypes.Ammo12gauge] = new AmmoTypeData { AmmoType = AmmoTypes.Ammo12gauge, TotalAmmoMax = 999, TotalAmmoCurrent = 10 };

		WeaponDictionary[WeaponRangedTypes.HarmonicaRevolver] = new WeaponRangedTypeData
		{
			RagnedWeaponType = WeaponRangedTypes.HarmonicaRevolver,
			AmmoType = AmmoTypes.Ammo9mm,
			MagazineAmmoMax = 5,
			MagazineAmmoCurrent = 5
		};
		WeaponDictionary[WeaponRangedTypes.SawedOffShotgun] = new WeaponRangedTypeData
		{
			RagnedWeaponType = WeaponRangedTypes.SawedOffShotgun,
			AmmoType = AmmoTypes.Ammo12gauge,
			MagazineAmmoMax = 2,
			MagazineAmmoCurrent = 2
		};

		Debug.Log("PlayerResourcesAmmoManager Initialized");
	}

	public void AddAmmoToMagazine(AmmoTypes type, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError($"[PlayerResourcesAmmoManager] Попытка добавить в магазин неположительное количество патронов: {amount}.");
			return;
		}
		OnMagazineAmmoChanged?.Invoke(type, amount);
	}

	public void RemoveAmmoFromMagazine(AmmoTypes type, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError($"[PlayerResourcesAmmoManager] Попытка отнять из магазина неположительное количество патронов: {amount}.");
			return;
		}
		OnMagazineAmmoChanged?.Invoke(type, -amount);
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
		data.AmmoDictionary = new List<AmmoTypeData>(AmmoDictionary.Values);
		data.RangedWeapons = new List<WeaponRangedTypeData>(WeaponDictionary.Values);
	}

	public void LoadData(GameData data)
	{
		if (data.AmmoDictionary != null)
		{
			foreach (var ammoData in data.AmmoDictionary)
			{
				if (AmmoDictionary.ContainsKey(ammoData.AmmoType))
				{
					AmmoDictionary[ammoData.AmmoType] = ammoData;
				}
				else
				{
					AmmoDictionary.Add(ammoData.AmmoType, ammoData);
				}
			}
		}

		if (data.RangedWeapons != null)
		{
			foreach (var weaponData in data.RangedWeapons)
			{
				if (WeaponDictionary.ContainsKey(weaponData.RagnedWeaponType))
				{
					WeaponDictionary[weaponData.RagnedWeaponType] = weaponData;
				}
				else
				{
					WeaponDictionary.Add(weaponData.RagnedWeaponType, weaponData);
				}
			}
		}

		foreach (var ammoData in AmmoDictionary.Values)
		{
			OnReserveAmmoChanged?.Invoke(ammoData.AmmoType, ammoData.TotalAmmoCurrent);
		}

		foreach (var weaponData in WeaponDictionary.Values)
		{
			OnMagazineAmmoChanged?.Invoke(weaponData.AmmoType, weaponData.MagazineAmmoCurrent);
		}
	}
}
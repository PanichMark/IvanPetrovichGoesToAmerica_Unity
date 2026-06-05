using System;
using System.Collections.Generic;
using UnityEngine;

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
		List<AmmoTypeData> ammoListForSaving = new List<AmmoTypeData>();

		foreach (var kvp in AmmoDictionary)
		{
			AmmoTypeData saveStruct = kvp.Value;
	
			saveStruct.AmmoJson = kvp.Key.ToString();
			ammoListForSaving.Add(saveStruct);
		}

		data.AmmoDictionary = ammoListForSaving;
	}

	public void LoadData(GameData data)
	{
		if (data.AmmoDictionary != null)
		{
			for (int i = 0; i < data.AmmoDictionary.Count; i++)
			{
				var currentData = data.AmmoDictionary[i];
				if (Enum.TryParse(currentData.AmmoJson, out AmmoTypes parsedType))
				{
					AmmoTypeData updatedData = currentData;
					updatedData.AmmoType = parsedType;

					data.AmmoDictionary[i] = updatedData;

					if (AmmoDictionary.ContainsKey(parsedType))
					{
						AmmoDictionary[parsedType] = updatedData;
					}
					else
					{
						AmmoDictionary.Add(parsedType, updatedData);
					}
				}
			}
		}
	}
}
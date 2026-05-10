using UnityEngine;
using System.Collections.Generic;

public delegate void OnAmmoChangedHandler(AmmoTypes type, int newAmount);

public class PlayerResourcesAmmoManager : MonoBehaviour
{
	public event OnAmmoChangedHandler OnReserveAmmoChanged;
	public event OnAmmoChangedHandler OnMagazineAmmoChanged;

	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary = new Dictionary<AmmoTypes, AmmoTypeData>();

	public void ModifyMagazineAmmo(AmmoTypes type, int newMagazineAmount)
	{
		OnMagazineAmmoChanged?.Invoke(type, newMagazineAmount);
	}

	private void Awake()
	{
		AmmoDictionary[AmmoTypes.Ammo9mm] = new AmmoTypeData { Type = AmmoTypes.Ammo9mm, TotalAmmoMax = 99, TotalAmmoCurrent = 99 };
		AmmoDictionary[AmmoTypes.Ammo12gauge] = new AmmoTypeData { Type = AmmoTypes.Ammo12gauge, TotalAmmoMax = 99, TotalAmmoCurrent = 99 };
	}

	public void ModifyReserveAmmo(AmmoTypes type, int amount)
	{
		if (AmmoDictionary.TryGetValue(type, out AmmoTypeData data))
		{
			data.TotalAmmoCurrent = Mathf.Clamp(data.TotalAmmoCurrent + amount, 0, data.TotalAmmoMax);
			AmmoDictionary[type] = data;

			OnReserveAmmoChanged?.Invoke(type, data.TotalAmmoCurrent);
		}
		else
		{
			Debug.LogWarning($"Тип патронов {type} не найден в словаре.");
		}
	}
}
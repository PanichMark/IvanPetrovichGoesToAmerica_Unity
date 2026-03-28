// PlayerResourcesAmmoManager.cs
using UnityEngine;
using System.Collections.Generic;

// 1. Определяем делегат (сигнатуру метода)
public delegate void OnAmmoChangedHandler(AmmoTypes type, int newAmount);

public class PlayerResourcesAmmoManager : MonoBehaviour
{
	// 2. Создаем статическое событие (или обычное, если менеджер один)
	public event OnAmmoChangedHandler OnReserveAmmoChanged;
	public event OnAmmoChangedHandler OnMagazineAmmoChanged;
	// Используем словарь для быстрого доступа по типу патронов
	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary = new Dictionary<AmmoTypes, AmmoTypeData>();

	// Внутри класса PlayerResourcesAmmoManager

	// Этот метод будет вызываться оружием
	public void ModifyMagazineAmmo(AmmoTypes type, int newMagazineAmount)
	{
		// Здесь мы находимся внутри PlayerResourcesAmmoManager,
		// поэтому мы имеем полное право вызывать свое событие.
		OnMagazineAmmoChanged?.Invoke(type, newMagazineAmount);
	}

	private void Awake()
	{
		AmmoDictionary[AmmoTypes.Ammo9mm] = new AmmoTypeData { Type = AmmoTypes.Ammo9mm, TotalAmmoMax = 99, TotalAmmoCurrent = 99 };
		AmmoDictionary[AmmoTypes.Ammo12gauge] = new AmmoTypeData { Type = AmmoTypes.Ammo12gauge, TotalAmmoMax = 99, TotalAmmoCurrent = 99 };
	}

	// Метод для изменения количества патронов (учитывает особенности struct)
	public void ModifyReserveAmmo(AmmoTypes type, int amount)
	{
		if (AmmoDictionary.TryGetValue(type, out AmmoTypeData data))
		{
			// 1. Достаем структуру из словаря
			// 2. Изменяем ее копию
			data.TotalAmmoCurrent = Mathf.Clamp(data.TotalAmmoCurrent + amount, 0, data.TotalAmmoMax);
			// 3. Помещаем измененную копию обратно в словарь
			AmmoDictionary[type] = data;

			OnReserveAmmoChanged?.Invoke(type, data.TotalAmmoCurrent);
		}
		else
		{
			Debug.LogWarning($"Тип патронов {type} не найден в словаре.");
		}
	}

	
}
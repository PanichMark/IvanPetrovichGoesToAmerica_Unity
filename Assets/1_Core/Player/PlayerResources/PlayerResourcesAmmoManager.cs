// PlayerResourcesAmmoManager.cs
using UnityEngine;
using System.Collections.Generic;

public class PlayerResourcesAmmoManager : MonoBehaviour
{
	// Используем словарь для быстрого доступа по типу патронов
	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary = new Dictionary<AmmoTypes, AmmoTypeData>();

	private void Awake()
	{
		AmmoDictionary[AmmoTypes.Ammo9mm] = new AmmoTypeData { Type = AmmoTypes.Ammo9mm, Max = 100, Current = 80 };
	}

	// Метод для изменения количества патронов (учитывает особенности struct)
	public void ModifyAmmo(AmmoTypes type, int amount)
	{
		if (AmmoDictionary.TryGetValue(type, out AmmoTypeData data))
		{
			// 1. Достаем структуру из словаря
			// 2. Изменяем ее копию
			data.Current = Mathf.Clamp(data.Current + amount, 0, data.Max);
			// 3. Помещаем измененную копию обратно в словарь
			AmmoDictionary[type] = data;
		}
		else
		{
			Debug.LogWarning($"Тип патронов {type} не найден в словаре.");
		}
	}
}
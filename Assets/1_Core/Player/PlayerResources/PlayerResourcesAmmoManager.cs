// PlayerResourcesAmmoManager.cs
using UnityEngine;
using System.Collections.Generic;

// 1. Определяем делегат (сигнатуру метода)
public delegate void OnAmmoChangedHandler(AmmoTypes type, int newAmount);

public class PlayerResourcesAmmoManager : MonoBehaviour
{
	// 2. Создаем статическое событие (или обычное, если менеджер один)
	public event OnAmmoChangedHandler OnAmmoChanged;
	// Используем словарь для быстрого доступа по типу патронов
	public Dictionary<AmmoTypes, AmmoTypeData> AmmoDictionary = new Dictionary<AmmoTypes, AmmoTypeData>();

	private void Awake()
	{
		AmmoDictionary[AmmoTypes.Ammo9mm] = new AmmoTypeData { Type = AmmoTypes.Ammo9mm, Max = 100, Current = 80 };
		AmmoDictionary[AmmoTypes.Ammo12gauge] = new AmmoTypeData { Type = AmmoTypes.Ammo12gauge, Max = 20, Current = 10 };
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

			OnAmmoChanged.Invoke(type, data.Current);
		}
		else
		{
			Debug.LogWarning($"Тип патронов {type} не найден в словаре.");
		}
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigWeapons", menuName = "Scriptable Objects/Configs/ConfigWeapons")]
public class ConfigWeapons : ScriptableObject
{
	[Header("Доступные виды оружия")]
	[Tooltip("Список доступных оружий (указываются Prefab'ы оружия).")]
	public WeaponPrefabEntry[] availableWeapons;

	[Serializable]
	public struct WeaponPrefabEntry
	{
		public GameObject weaponPrefab; // Сам префаб оружия
		public bool isUnlocked;        // Доступность оружия
	}


	public GameObject[] GetAvailableWeapons()
	{
		List<GameObject> result = new List<GameObject>();
		foreach (var entry in availableWeapons)
		{
			if (entry.isUnlocked)
			{
				result.Add(entry.weaponPrefab);
			}
		}
		return result.ToArray();
	}
}
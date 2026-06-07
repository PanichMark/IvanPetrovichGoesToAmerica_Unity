using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeapons", menuName = "BootstrapConfigs/Player/PlayerWeapons")]
public class BootstrapConfigPlayerWeapons : ScriptableObject
{
	[Header("Доступные виды оружия")]
	[Tooltip("Список доступных оружий (указываются Prefab'ы оружия).")]

	public WeaponPrefabEntry[] AvailableWeapons;

	[Serializable]
	public struct WeaponPrefabEntry
	{
		public GameObject WeaponPrefab; 
		public bool IsWeaponUnlocked;       
	}

	public GameObject[] GetAvailableWeapons()
	{
		List<GameObject> result = new List<GameObject>();
		foreach (var entry in AvailableWeapons)
		{
			if (entry.IsWeaponUnlocked)
			{
				result.Add(entry.WeaponPrefab);
			}
		}
		return result.ToArray();
	}
}
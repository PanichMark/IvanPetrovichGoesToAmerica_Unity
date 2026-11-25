using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigPlayerWeapons", menuName = "Bootstrap/PlayerConfigs/Weapons")]
public class ConfigPlayerWeapons : ScriptableObject
{
	[Header("Доступные виды оружия")]
	[Tooltip("Список доступных оружий (указываются Prefab'ы оружия).")]

	public PlayerWeaponGive[] AvailableWeapons;

	public GameObject[] GetAvailableWeapons()
	{
		List<GameObject> result = new List<GameObject>();
		foreach (var entry in AvailableWeapons)
		{
			result.Add(entry.WeaponPrefab);
		}
		return result.ToArray();
	}
}
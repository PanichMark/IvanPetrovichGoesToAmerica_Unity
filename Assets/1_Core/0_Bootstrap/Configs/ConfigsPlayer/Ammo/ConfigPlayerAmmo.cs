using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ConfigPlayerAmmo", menuName = "Bootstrap/PlayerConfigs/Ammo")]
public class ConfigPlayerAmmo : ScriptableObject
{
	public AmmoGive[] AmmoEntries;

	public AmmoGive[] GetStartAmmoEntries()
	{
		return AmmoEntries;
	}
}
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ConfigPlayerAmmo", menuName = "Bootstrap/PlayerConfigs/Ammo")]
public class ConfigPlayerAmmo : ScriptableObject
{
	public AmmoEntry[] AmmoEntries;

	public AmmoEntry[] GetStartAmmoEntries()
	{
		return AmmoEntries;
	}
}
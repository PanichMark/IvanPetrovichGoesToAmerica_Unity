using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ConfigPlayerResourcesAmmo", menuName = "Configs/Player/Resources/Ammo")]
public class ConfigPlayerResourcesAmmo : ScriptableObject
{
	public AmmoEntry[] AmmoEntries;

	public AmmoEntry[] GetStartAmmoEntries()
	{
		return AmmoEntries;
	}
}
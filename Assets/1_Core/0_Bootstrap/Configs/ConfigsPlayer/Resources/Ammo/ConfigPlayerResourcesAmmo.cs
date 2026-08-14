using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ConfigPlayerResourcesAmmo", menuName = "Configs/Player/Resources/Ammo")]
public class ConfigPlayerResourcesAmmo : ScriptableObject
{
	[Serializable]
	public struct AmmoEntry
	{
		public AmmoTypes AmmoType;
		[Range(0, 999)] public int StartAmount;
	}

	public AmmoEntry[] AmmoEntries;

	public AmmoEntry[] GetStartAmmoEntries()
	{
		return AmmoEntries;
	}
}
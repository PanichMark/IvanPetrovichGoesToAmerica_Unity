using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ConfigBootstrapPlayerResourcesAmmo", menuName = "Scriptable Objects/Configs/ConfigsBootstrap/ConfigsBootstrapPlayerResources/Ammo")]
public class ConfigBootstrapPlayerResourcesAmmo : ScriptableObject
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
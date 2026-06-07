using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlayerResourcesAmmo", menuName = "BootstrapConfigs/Player/PlayerResources/Ammo")]
public class BootstrapConfigPlayerResourcesAmmo : ScriptableObject
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
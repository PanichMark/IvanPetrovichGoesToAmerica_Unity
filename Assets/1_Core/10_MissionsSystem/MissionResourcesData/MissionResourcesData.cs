using UnityEngine;

[CreateAssetMenu(fileName = "MissionResourcesData", menuName = "Missions/MissionResourcesData")]
public class MissionResourcesData : ScriptableObject
{
	public InteractionObjectChangeSceneData PlayerTransform;

	[Range(1, 100)] public int PlayerHealth;
	[Range(0, 9)] public int PlayerHealingItems;

	[Range(0, 100)] public int PlayerMana;
	[Range(0, 9)] public int PlayerManaReplenishItems;

	[Range(0, 999999)] public int PlayerMoney;

	public WeaponPrefabEntry[] WeaponsToUnlock;
	public AmmoEntry[] Ammo;
}

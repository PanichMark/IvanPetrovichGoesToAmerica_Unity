using System;

[Serializable]
public struct WeaponRangedTypeData
{
	public WeaponRangedTypes RagnedWeaponTypeSystem;
	public string RagnedWeaponTypeJson;
	public AmmoTypes AmmoTypeSystem;
	public string AmmoTypeJson;
	public int MagazineAmmoCurrent;

	[NonSerialized]
	public int MagazineAmmoMax;
}
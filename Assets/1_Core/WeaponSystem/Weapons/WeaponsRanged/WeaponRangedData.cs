using System;

[Serializable]
public struct WeaponRangedData
{
	public WeaponRangedEnum RagnedWeaponTypeSystem;
	public string RagnedWeaponTypeJson;
	public AmmoTypes AmmoTypeSystem;
	public string AmmoTypeJson;
	public int MagazineAmmoCurrent;

	[NonSerialized]
	public int MagazineAmmoMax;
}
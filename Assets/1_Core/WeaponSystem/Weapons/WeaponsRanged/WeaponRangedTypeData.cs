using System;

[Serializable]
public struct WeaponRangedTypeData
{
	[NonSerialized]
	public WeaponRangedTypes RagnedWeaponType;
	public string RagnedWeaponJson;
	[NonSerialized]
	public AmmoTypes AmmoType;
	public string AmmoJson;
	[NonSerialized]
	public int MagazineAmmoMax;
	public int MagazineAmmoCurrent;
}
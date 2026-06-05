using System;

[Serializable]
public struct WeaponRangedTypeData
{
	public WeaponRangedTypes RagnedWeaponType;
	public AmmoTypes AmmoType;
	[NonSerialized]
	public int MagazineAmmoMax;
	public int MagazineAmmoCurrent;
}
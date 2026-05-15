using System;

[Serializable]
public struct WeaponRangedTypeData
{
	public WeaponRangedTypes RagnedWeaponType;
	public AmmoTypeData AmmoType;
	public int MagazineAmmoMax;
	public int MagazineAmmoCurrent;
}
using System;

[Serializable]
public struct WeaponRangedData
{
	public PlayerWeaponNames RagnedWeapon;
	public AmmoTypes AmmoType;
	public int MagazineAmmoMax;
	public int MagazineAmmoCurrent;
}
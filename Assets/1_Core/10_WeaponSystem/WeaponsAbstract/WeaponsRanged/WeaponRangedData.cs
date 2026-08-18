using System;

[Serializable]
public struct WeaponRangedData
{
	public PlayerWeaponNames RagnedWeaponSystem;
	public string RagnedWeaponJson;
	public AmmoTypes AmmoTypeSystem;
	public string AmmoTypeJson;
	public int MagazineAmmoMax;
	public int MagazineAmmoCurrent;
}
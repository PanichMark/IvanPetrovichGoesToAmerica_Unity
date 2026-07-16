using System;

[Serializable]
public struct WeaponRangedData
{
	public WeaponNames RagnedWeaponSystem;
	public string RagnedWeaponJson;
	public AmmoTypes AmmoTypeSystem;
	public string AmmoTypeJson;
	public int MagazineAmmoMax;
	public int MagazineAmmoCurrent;
}
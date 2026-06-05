using System;

[Serializable]
public struct WeaponRangedData
{
	public WeaponRangedEnum RagnedWeaponSystem;
	public string RagnedWeaponJson;
	public AmmoTypes AmmoTypeSystem;
	public string AmmoTypeJson;
	public int MagazineAmmoMax;
	public int MagazineAmmoCurrent;
	
}
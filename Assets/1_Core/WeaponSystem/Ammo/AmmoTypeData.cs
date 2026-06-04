using System;

[Serializable]
public struct AmmoTypeData
{
	[NonSerialized]
	public AmmoTypes AmmoType;
	public string AmmoTypeString;
	public int TotalAmmoMax;
	public int TotalAmmoCurrent;
}
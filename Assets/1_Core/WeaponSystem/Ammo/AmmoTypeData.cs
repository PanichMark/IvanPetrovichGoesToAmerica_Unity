using System;

[Serializable]
public struct AmmoTypeData
{
	[NonSerialized]
	public AmmoTypes AmmoType;
	public string AmmoJson;
	[NonSerialized]
	public int TotalAmmoMax;
	public int TotalAmmoCurrent;
}
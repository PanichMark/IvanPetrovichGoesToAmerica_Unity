using System;

[Serializable]
public struct AmmoTypeData
{
	public AmmoTypes AmmoTypeSystem;
	public string AmmoTypeJson;
	public int TotalAmmoCurrent;

	[NonSerialized]
	public int TotalAmmoMax;
}
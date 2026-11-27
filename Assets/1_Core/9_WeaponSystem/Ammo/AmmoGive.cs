using System;
using UnityEngine;

[Serializable]
public struct AmmoGive
{
	public AmmoTypes AmmoType;
	[Range(0, 999)] public int StartAmount;
}
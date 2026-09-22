using System;
using UnityEngine;

[Serializable]
public struct AmmoEntry
{
	public AmmoTypes AmmoType;
	[Range(0, 999)] public int StartAmount;
}
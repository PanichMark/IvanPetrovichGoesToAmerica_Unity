// AmmoTypeData.cs
using System;

[Serializable] // Чтобы Unity показывала поля в Инспекторе
public struct AmmoTypeData
{
	public AmmoTypes Type;
	public int Max;
	public int Current;
}
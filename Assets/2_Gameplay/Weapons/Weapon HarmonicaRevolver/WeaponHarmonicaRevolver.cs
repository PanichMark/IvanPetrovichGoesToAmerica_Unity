using UnityEngine;

public class WeaponHarmonicaRevolver : RangedWeaponAbstract
{
	public override float WeaponDamage => 30f; // Устанавливаем постоянное значение урона для револьвера

	
	public override string WeaponNameSystem => "HarmonicaRevolver";
	public override string WeaponNameUI => "Револьвер Гармоника";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Pistol icon");



}



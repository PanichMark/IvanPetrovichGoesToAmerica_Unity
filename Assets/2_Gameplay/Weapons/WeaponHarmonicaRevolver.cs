using UnityEngine;

public class WeaponHarmonicaRevolver : WeaponRangedAbstract
{
	public override string WeaponNameSystem => "HarmonicaRevolver";
	public override string WeaponNameUI => "Револьвер Гармоника";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponRangedHarmoniceRevolverIcon");

	public override float WeaponDamage => 30f;
}
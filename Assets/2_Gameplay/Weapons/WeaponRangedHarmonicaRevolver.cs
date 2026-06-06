using UnityEngine;

public class WeaponRangedHarmonicaRevolver : WeaponRangedAbstract
{
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "HarmonicaRevolver";
	public override string WeaponType => WeaponTypes.Ranged.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponRangedHarmoniceRevolverIcon");

	public override float WeaponDamage => 30f;
	public override bool IsSingleAttack => true;
	protected override void InitializeWeaponRanged()
	{

	}
}
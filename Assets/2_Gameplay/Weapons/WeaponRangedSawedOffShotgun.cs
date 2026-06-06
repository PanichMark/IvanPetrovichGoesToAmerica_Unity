using UnityEngine;

public class WeaponRangedSawedOffShotgun : WeaponRangedAbstract
{
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "SawedOffShotgun";
	public override string WeaponType => WeaponTypes.Ranged.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponRangedSawedOffShotgunIcon");
	public override float WeaponDamage => 100f;
	public override bool IsSingleAttack => true;
	protected override void InitializeWeaponRanged()
	{
		
	}
}
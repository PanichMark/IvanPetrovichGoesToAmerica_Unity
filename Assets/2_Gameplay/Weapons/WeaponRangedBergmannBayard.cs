using UnityEngine;

public class WeaponRangedBergmannBayard : WeaponRangedAbstract
{
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "BergmannBayard";
	public override string WeaponType => WeaponTypes.Ranged.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheel/WeaponWheel_WeaponIcons/WeaponRangedBergmannBayardIcon");

	public override float WeaponDamage => 15f;

	protected override void InitializeWeaponRanged()
	{

	}
}
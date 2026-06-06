using UnityEngine;

public class WWeaponEugenicTeslaShock : WeaponEugenicAbstract
{
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "TeslaShock";
	public override string WeaponType => WeaponTypes.Eugenic.ToString();

	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponWheel/WeaponWheel_WeaponIcons/Weapon{WeaponType}{WeaponName}Icon");
	public override bool IsWeaponAuto => false;

	public override float WeaponDamage => 50;

	protected override void InitializeWeaponEugenic()
	{
		ManaCost = 2;
	}
}
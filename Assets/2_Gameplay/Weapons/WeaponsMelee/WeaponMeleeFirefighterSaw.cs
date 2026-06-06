using System.Collections;
using UnityEngine;

public class WeaponMeleeFirefighterSaw : WeaponMeleeAbstract
{
	public override bool IsWeaponAuto => true;

	public override float WeaponDamage => 15f;
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponName => "FirefighterSaw";

	public override string WeaponType => WeaponTypes.Melee.ToString();

	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponWheel/WeaponWheel_WeaponIcons/Weapon{WeaponType}{WeaponName}Icon");


	protected override void InitializeWeaponMelee()
	{
		_weaponAutoAttackSpeedRate = 0.2f;

		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;
		_attackDelay = 0f;
	}
}
using System.Collections;
using UnityEngine;

public class WeaponSpecialMonocular : WeaponAbstract
{
	public override PlayerWeaponNames WeaponName => PlayerWeaponNames.Monocular;

	public override WeaponTypes WeaponType => WeaponTypes.Special;

	public override float WeaponDamage => 0;

	public override bool IsWeaponAuto => true;

	public override float WeaponAttackSpeedRate => throw new System.NotImplementedException();

	public override float TimeBetweenAbilityToAttack => throw new System.NotImplementedException();

	public override IEnumerator AutoAttackWeaponPlayerCourutine()
	{
		throw new System.NotImplementedException();
	}

	public override void InitializeWeapon()
	{
	//	throw new System.NotImplementedException();
	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		//throw new System.NotImplementedException();
	}

	public override void StopAutoAttacking()
	{
		//throw new System.NotImplementedException();
	}

	public override void WeaponAttack()
	{
		//throw new System.NotImplementedException();
	}
}

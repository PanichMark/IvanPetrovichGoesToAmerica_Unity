using System.Collections;
using UnityEngine;

public abstract class WeaponPickableRangedAbstract : WeaponPickableAbstract
{
	public abstract float WeaponRange { get; }

	public abstract bool LeavesBulletHole { get; }

	public abstract int WeaponReserveAmmoCurrent { get; }
	public abstract int WeaponMagazineAmmoCurrent { get; }

	public abstract int WeaponMagazineAmmoMax { get; }

	public abstract GameObject WeaponRangedShootPoint { get; }

	public override void AttackRight()
	{
		Debug.Log("PickableWeapon RIGHT attack");
	}

	public override void AttackLeft()
	{
		Debug.Log("PickableWeapon LEFT attack");
	}

	public override void InitializeWeapon()
	{
		//throw new System.NotImplementedException();
	}

	public override void StartAutoAttacking()
	{
		//throw new System.NotImplementedException();
	}

	public override void StopAutoAttacking()
	{
		//throw new System.NotImplementedException();
	}

	public override IEnumerator AutoAttackWeaponCourutine()
	{
		//throw new System.NotImplementedException();
		yield return null;
	}
}

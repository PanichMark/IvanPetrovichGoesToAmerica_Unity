using UnityEngine;
using System.Collections;

public abstract class WeaponEugenicAbstract : WeaponAbstract
{
	public abstract int ManaCost {  get; }


	protected GameObject _eugenicSourcePoint;
	protected GameObject _eugenicAttackDirection;

	protected PlayerResourcesManaManager _playerResourcesManaManager;

	public override void InitializeWeapon()
	{
		if (_isThisPlayerWeapon == true)
		{
			_eugenicAttackDirection = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
			_eugenicSourcePoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCamera");

			_playerResourcesManaManager = ServiceLocator.Resolve<PlayerResourcesManaManager>("PlayerResourcesManaManager");
		}

		InitializeWeaponEugenic();
	}

	public override void WeaponAttack()
	{
		if (_playerResourcesManaManager.CurrentPlayerMana >= ManaCost)
		{
			if (IsWeaponAuto)
			{
				StartAutoAttacking(); 
			}
			else 
			{
				SingleEugenicAttack();
			}
		}
	}

	public override void StartAutoAttacking()
	{
		if (_isWeaponAutoAttacking) return;
		_isWeaponAutoAttacking = true;
		if (_weaponAutoAttackCourutine == null)
		{
			_weaponAutoAttackCourutine = StartCoroutine(AutoAttackCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		_isWeaponAutoAttacking = false;
		if (_weaponAutoAttackCourutine != null)
		{
			//TurnEugenicVFXOff();

			StopCoroutine(_weaponAutoAttackCourutine);
			_weaponAutoAttackCourutine = null;
		}
	}

	public abstract void TurnEugenicVFXOff();

	public override IEnumerator AutoAttackCourutine()
	{
		while (true)
		{
			if (!_isWeaponAutoAttacking)
			{
				break;
			}

			AutoEugenicAttack();

			yield return new WaitForSeconds(_weaponAutoAttackSpeedRate);

			if (_playerResourcesManaManager.CurrentPlayerMana <= 0)
			{
				_isWeaponAutoAttacking = false;
				break;
			}
		}
		_weaponAutoAttackCourutine = null;
	}

	protected virtual void SingleEugenicAttack()
	{

	}

	protected virtual void AutoEugenicAttack()
	{

	}

	protected abstract void InitializeWeaponEugenic();

	private void OnDestroy()
	{
		//TurnEugenicVFXOff();
	}
}
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
				StartAutoShootingWeaponPlayer(); 
			}
			else 
			{
				SingleEugenicAttack();
			}
		}
	}

	public override void StartAutoShootingWeaponPlayer()
	{
		if (_isWeaponPlayerAutoShooting) return;
		_isWeaponPlayerAutoShooting = true;
		if (_currentWeaponPlayerAutoShootCourutine == null)
		{
			_currentWeaponPlayerAutoShootCourutine = StartCoroutine(AutoShootWeaponPlayerCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		_isWeaponPlayerAutoShooting = false;
		if (_currentWeaponPlayerAutoShootCourutine != null)
		{
			//TurnEugenicVFXOff();

			StopCoroutine(_currentWeaponPlayerAutoShootCourutine);
			_currentWeaponPlayerAutoShootCourutine = null;
		}
	}

	public abstract void TurnEugenicVFXOff();

	public override IEnumerator AutoShootWeaponPlayerCourutine()
	{
		while (true)
		{
			if (!_isWeaponPlayerAutoShooting)
			{
				break;
			}

			AutoEugenicAttack();

			yield return new WaitForSeconds(_weaponAutoAttackSpeedRate);

			if (_playerResourcesManaManager.CurrentPlayerMana <= 0)
			{
				_isWeaponPlayerAutoShooting = false;
				break;
			}
		}
		_currentWeaponPlayerAutoShootCourutine = null;
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
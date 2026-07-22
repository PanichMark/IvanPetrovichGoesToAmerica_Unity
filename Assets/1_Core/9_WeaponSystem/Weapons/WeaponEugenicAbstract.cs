using UnityEngine;
using System.Collections;

public abstract class WeaponEugenicAbstract : WeaponAbstract
{
	public abstract int ManaCost {  get; }

	protected GameObject _eugenicSourcePoint;
	protected GameObject _eugenicAttackDirection;

	protected PlayerResourcesManaManager _playerResourcesManaManager;
	protected Coroutine _currentWeaponPlayerEugenicAttackRoutine;
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
				StartAutoAttackingWeaponPlayer(); 
			}
			else 
			{
				StartCoroutine(SingleEugenicAttack());
			}
		}
	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		if (_isWeaponPlayerAutoShooting) return;
		_isWeaponPlayerAutoShooting = true;
		if (_currentWeaponPlayerAutoAttackCourutine == null)
		{
			_currentWeaponPlayerAutoAttackCourutine = StartCoroutine(AutoAttackWeaponPlayerCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		_isWeaponPlayerAutoShooting = false;
		if (_currentWeaponPlayerAutoAttackCourutine != null)
		{
			//TurnEugenicVFXOff();

			StopCoroutine(_currentWeaponPlayerAutoAttackCourutine);
			_currentWeaponPlayerAutoAttackCourutine = null;
		}
	}

	public abstract void TurnEugenicVFXOff();

	public override IEnumerator AutoAttackWeaponPlayerCourutine()
	{
		while (true)
		{
			if (!_isWeaponPlayerAutoShooting)
			{
				break;
			}

			AutoEugenicAttack();

			yield return new WaitForSeconds(WeaponAttackSpeedRate);

			if (_playerResourcesManaManager.CurrentPlayerMana <= 0)
			{
				_isWeaponPlayerAutoShooting = false;
				break;
			}
		}
		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected virtual IEnumerator SingleEugenicAttack()
	{
		yield return null;
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
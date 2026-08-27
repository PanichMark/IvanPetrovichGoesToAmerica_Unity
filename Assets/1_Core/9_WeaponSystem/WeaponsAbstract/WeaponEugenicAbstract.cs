using UnityEngine;
using System.Collections;

public abstract class WeaponEugenicAbstract : WeaponAbstract
{
	public abstract int ManaCost {  get; }
	protected GameObject _eugenicSourcePoint;
	protected GameObject _eugenicAttackDirection;

	protected PlayerManaController _playerResourcesManaManager;
	protected Coroutine _currentWeaponPlayerEugenicAttackRoutine;

	[SerializeField] protected GameObject _VFXeffect;
	protected Transform _VFXspawnPoint;
	protected GameObject _vfxInstance;
	public override void InitializeWeapon()
	{
		if (_isThisPlayerWeapon == true)
		{
_eugenicAttackDirection = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.Player);
_eugenicSourcePoint = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCamera);

_playerResourcesManaManager = ServiceLocator.Resolve<PlayerManaController>();
		}

		InitializeWeaponEugenic();
	}

	public override void WeaponAttack()
	{
		if (_playerResourcesManaManager.CurrentPlayerMana >= ManaCost)
		{
			if (_isAttacking)
			{
				Debug.Log("Already attacking eugenic");
				return;
			}

			if (IsWeaponAuto)
			{
				_isAttacking = true;
				StartAutoAttackingWeaponPlayer();
			}
			else
			{
				_isAttacking = true;
				StartCoroutine(SingleEugenicAttack());
			}
		}

	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		if (IsWeaponPlayerAutoAttacking) return;
		IsWeaponPlayerAutoAttacking = true;
		if (_currentWeaponPlayerAutoAttackCourutine == null)
		{
			_currentWeaponPlayerAutoAttackCourutine = StartCoroutine(AutoAttackWeaponPlayerCourutine());
		}
	}

	public override void StopAutoAttacking()
	{
		IsWeaponPlayerAutoAttacking = false;
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
			if (!IsWeaponPlayerAutoAttacking)
			{
				break;
			}

			StartCoroutine(SingleEugenicAttack());

			yield return new WaitForSeconds(WeaponAttackSpeedRate);

			if (_playerResourcesManaManager.CurrentPlayerMana <= 0)
			{
				IsWeaponPlayerAutoAttacking = false;
				break;
			}
		}
		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected virtual IEnumerator SingleEugenicAttack()
	{
		yield return null;
	}

	protected abstract void InitializeWeaponEugenic();

	private void OnDestroy()
	{
		//TurnEugenicVFXOff();
	}
}
using System.Collections;
using UnityEngine;

public abstract class WeaponMeleeAbstract : WeaponAbstract
{
	protected GameObject _attackPoint;

	protected bool _isAttacking;

	protected float _capsuleHeight;
	protected float _capsuleRadius;
	protected float _forwardOffset;
	public abstract float MeleeAttackDelay { get; }
	protected Coroutine _currentWeaponPlayerMeleeAttackRoutine;
	[SerializeField] protected WeaponsMeleeTypes _weaponMeleeType;

	public WeaponsMeleeTypes WeaponMeleeType => _weaponMeleeType;

	public override void InitializeWeapon()
	{
		if (_isThisPlayerWeapon == true)
		{
			_attackPoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		}

		InitializeWeaponMelee();
	}

	public override void WeaponAttack()
	{
		if (_isAttacking)
		{
			Debug.Log("Already attacking melee");
			return;
		}

		if (IsWeaponAuto)
		{
			StartAutoAttackingWeaponPlayer();
		}
		else
		{
			_isAttacking = true;
			StartCoroutine(SingleMeleeWeaponAttack());
		}
	}

	public override void StartAutoAttackingWeaponPlayer()
	{
		if (_isWeaponPlayerAutoShooting)
		{
			return;
		}

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
			StopCoroutine(_currentWeaponPlayerAutoAttackCourutine);
			_currentWeaponPlayerAutoAttackCourutine = null;
		}
	}

	public override IEnumerator AutoAttackWeaponPlayerCourutine()
	{
		while (_isWeaponPlayerAutoShooting)
		{
			StartCoroutine(SingleMeleeWeaponAttack());

			yield return new WaitForSeconds(WeaponAttackSpeedRate);
		}

		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected virtual IEnumerator SingleMeleeWeaponAttack()
	{
		_currentWeaponPlayerMeleeAttackRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponFullArmAttackAnimation(this));

		Vector3 startPoint = _attackPoint.transform.position + _attackPoint.transform.forward * _forwardOffset;
		Vector3 endPoint = startPoint + _attackPoint.transform.up * _capsuleHeight;

		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, _capsuleRadius, _attackPoint.transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject == _attackPoint)
				continue;

			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayMeleeAttackDamageable(damageable, MeleeAttackDelay));
			}
		}

		yield return _currentWeaponPlayerMeleeAttackRoutine;

		_isAttacking = false;

		_currentWeaponPlayerMeleeAttackRoutine = null;
	}

	protected IEnumerator DelayMeleeAttackDamageable(IDamageable target, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		target.TakeDamage(WeaponDamage);
	}

	protected IEnumerator DelayMeleeAttackBreakable(IBreakable target, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		target.TakeDamage(WeaponDamage);
	}

	protected abstract void InitializeWeaponMelee();
}
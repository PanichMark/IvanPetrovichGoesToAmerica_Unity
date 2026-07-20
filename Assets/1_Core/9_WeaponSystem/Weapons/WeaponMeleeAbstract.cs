using System.Collections;
using UnityEngine;

public abstract class WeaponMeleeAbstract : WeaponAbstract
{
	protected GameObject _attackPoint;

	protected bool _isAttacking = false;

	protected float _capsuleHeight;
	protected float _capsuleRadius;
	protected float _forwardOffset;
	protected float _attackDelay;

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
		if (IsWeaponAuto)
		{
			StartAutoShootingWeaponPlayer();
		}
		else
		{
			_isAttacking = true;
			StartCoroutine(MeleeWeaponAttack());
		}
	}

	public override void StartAutoShootingWeaponPlayer()
	{
		if (_isWeaponPlayerAutoShooting) return;
		_isWeaponPlayerAutoShooting = true;
		if (_currentWeaponPlayerAutoAttackCourutine == null)
		{
			_currentWeaponPlayerAutoAttackCourutine = StartCoroutine(AutoShootWeaponPlayerCourutine());
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

	public override IEnumerator AutoShootWeaponPlayerCourutine()
	{
		while (_isWeaponPlayerAutoShooting)
		{
			StartCoroutine(MeleeWeaponAttack());

			yield return new WaitForSeconds(WeaponAttackSpeedRate);
		}

		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected virtual IEnumerator MeleeWeaponAttack()
	{
		Vector3 startPoint = _attackPoint.transform.position + _attackPoint.transform.forward * _forwardOffset;
		Vector3 endPoint = startPoint + _attackPoint.transform.up * _capsuleHeight;

		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, _capsuleRadius, _attackPoint.transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject == _attackPoint)
				continue;

			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayMeleeAttackDamage(damageable, _attackDelay));
			}
		}

		yield return new WaitForSeconds(_attackDelay + 0.15f);
		_isAttacking = false;
	}

	protected IEnumerator DelayMeleeAttackDamage(IDamageable target, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		target.TakeDamage(WeaponDamage);
	}

	protected IEnumerator DelayMeleeAttackDamage(IBreakable target, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		target.TakeDamage(WeaponDamage);
	}

	protected abstract void InitializeWeaponMelee();
}
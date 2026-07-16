using UnityEngine;
using System.Collections;

public class WeaponMeleeFirefighterSaw : WeaponMeleeAbstract
{
	public override WeaponNames WeaponName => WeaponNames.FirefighterSaw;
	public override WeaponTypes WeaponType => WeaponTypes.Melee;
	public override float WeaponDamage => 15f;
	public override bool IsWeaponAuto => true;

	protected override void InitializeWeaponMelee()
	{
		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;
		_attackDelay = 0f;

		_weaponAttackSpeedRate = 0.2f;
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

			yield return new WaitForSeconds(_weaponAttackSpeedRate);
		}

		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected override IEnumerator MeleeWeaponAttack()
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

			if (hit.collider.TryGetComponent<IBreakable>(out var breakabale))
			{
				StartCoroutine(DelayMeleeAttackDamage(breakabale, _attackDelay));
			}
		}

		yield return new WaitForSeconds(_attackDelay + 0.15f);
		_isAttacking = false;
	}
}
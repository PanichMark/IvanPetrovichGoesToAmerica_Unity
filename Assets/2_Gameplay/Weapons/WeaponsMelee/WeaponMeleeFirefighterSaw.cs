using UnityEngine;
using System.Collections;

public class WeaponMeleeFirefighterSaw : WeaponMeleeAbstract
{
	public override string WeaponName => "FirefighterSaw";
	public override string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public override string WeaponType => WeaponTypes.Melee.ToString();
	public override Sprite WeaponIcon => Resources.Load<Sprite>($"WeaponWheel/WeaponWheel_WeaponIcons/Weapon{WeaponType}{WeaponName}Icon");
	public override float WeaponDamage => 15f;
	public override bool IsWeaponAuto => true;

	protected override void InitializeWeaponMelee()
	{
		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;
		_attackDelay = 0f;

		_weaponAutoAttackSpeedRate = 0.2f;
	}

	public override void WeaponAttack()
	{
		if (IsWeaponAuto)
		{
			StartAutoAttacking();
		}
		else
		{
			_isAttacking = true;
			StartCoroutine(MeleeWeaponAttack());
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
			StopCoroutine(_weaponAutoAttackCourutine);
			_weaponAutoAttackCourutine = null;
		}
	}

	public override IEnumerator AutoAttackCourutine()
	{
		while (_isWeaponAutoAttacking)
		{
			StartCoroutine(MeleeWeaponAttack());

			yield return new WaitForSeconds(_weaponAutoAttackSpeedRate);
		}

		_weaponAutoAttackCourutine = null;
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
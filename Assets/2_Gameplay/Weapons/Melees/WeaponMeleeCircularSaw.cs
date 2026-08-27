using UnityEngine;
using System.Collections;

public class WeaponMeleeCircularSaw : WeaponMeleeAbstract
{
	public override PlayerWeaponNames WeaponName => PlayerWeaponNames.CircularSaw;
	public override WeaponTypes WeaponType => WeaponTypes.Melee;
	public override float WeaponDamage => 15f;
	public override bool IsWeaponAuto => true;

	public override float WeaponAttackSpeedRate => 0.36f;

	public override float MeleeAttackDelay => 0.2f;

	public override float TimeBetweenAbilityToAttack => throw new System.NotImplementedException();

	private GameObject _sawBlade1stPerson;
	private GameObject _sawBlade3rdPerson;

	protected override void InitializeWeaponMelee()
	{
		_sawBlade1stPerson = FirstPersonWeaponModelInstance.transform.Find("SawBlade").gameObject;
		_sawBlade3rdPerson = ThirdPersonWeaponModelInstance.transform.Find("SawBlade").gameObject;

		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;
	}

	public override void WeaponAttack()
	{
		if (IsWeaponAuto)
		{
			_isAttacking = true;
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
			StopCoroutine(_currentWeaponPlayerAutoAttackCourutine);
			_currentWeaponPlayerAutoAttackCourutine = null;
		}
	}

	public override IEnumerator AutoAttackWeaponPlayerCourutine()
	{
		while (IsWeaponPlayerAutoAttacking)
		{
			StartCoroutine(SingleMeleeWeaponAttack());

			yield return new WaitForSeconds(WeaponAttackSpeedRate);
		}

		_currentWeaponPlayerAutoAttackCourutine = null;
	}

	protected override IEnumerator SingleMeleeWeaponAttack()
	{
		_weaponAudioSource.PlayOneShot(_weaponSoundAttack);

		StartCoroutine(RotateSawBlades());

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

			if (hit.collider.TryGetComponent<IBreakable>(out var breakable))
			{
				StartCoroutine(DelayMeleeAttackBreakable(breakable, MeleeAttackDelay));
			}
		}

		yield return _currentWeaponPlayerMeleeAttackRoutine;

		_isAttacking = false;

		_currentWeaponPlayerMeleeAttackRoutine = null;
	}

	private IEnumerator RotateSawBlades()
	{
		float rotationDuration = WeaponAttackSpeedRate * 0.75f;
		float rotationSpeed = 100f;
		float elapsed = 0f;

		if (WeaponHandType == WeaponHandType.Right)
		{
			while (elapsed < rotationDuration)
			{
				_sawBlade1stPerson.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);
				_sawBlade3rdPerson.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);
				elapsed += Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			while (elapsed < rotationDuration)
			{
				_sawBlade1stPerson.transform.Rotate(Vector3.left, rotationSpeed * Time.deltaTime, Space.Self);
				_sawBlade3rdPerson.transform.Rotate(Vector3.left, rotationSpeed * Time.deltaTime, Space.Self);
				elapsed += Time.deltaTime;
				yield return null;
			}
		}

		_sawBlade1stPerson.transform.localEulerAngles = Vector3.zero;
		_sawBlade3rdPerson.transform.localEulerAngles = Vector3.zero;
	}
}
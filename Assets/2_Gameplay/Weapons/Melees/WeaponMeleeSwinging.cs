using UnityEngine;
using System.Collections;

public class WeaponMeleeSwinging : WeaponMeleeAbstract
{
	public override PlayerWeaponNames WeaponName => PlayerWeaponNames.Baton;
	public override WeaponTypes WeaponType => WeaponTypes.Melee;
	public override bool IsWeaponAuto => false;

	public override float WeaponAttackSpeedRate => 1.560f;
	[SerializeField] private AudioClip _weaponSoundSwing;
	
	public override float MeleeAttackDelay => 0.840f;

	public override float TimeBetweenAbilityToAttack => throw new System.NotImplementedException();

	public override WeaponsMeleeTypes WeaponMeleeType => WeaponsMeleeTypes.Swinging;

	protected override void InitializeWeaponMelee()
	{
		_capsuleHeight = 1.8f;
		_capsuleRadius = 0.3f;
		_forwardOffset = 0.5f;
	}

	public override void WeaponPlayerAttack()
	{
		
		if (isPlayerWeaponAttacking)
		{
			Debug.Log("Already attacking melee");
			return;
		}

		StartCoroutine(SingleMeleeWeaponAttack());
		
	}

	protected override IEnumerator SingleMeleeWeaponAttack()
	{
		isPlayerWeaponAttacking = true;

		StartCoroutine(DelayAttackSound());

		_currentWeaponPlayerMeleeAttackRoutine = StartCoroutine(_playerWeaponAnimationController.WeaponFullArmAttackAnimation(this, true));

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

		isPlayerWeaponAttacking = false;

		_currentWeaponPlayerMeleeAttackRoutine = null;
	}

	protected override IEnumerator DelayAttackSound()
	{
		Debug.Log("SOUND ON HIT");
		yield return new WaitForSeconds(MeleeAttackDelay - 0.35f);
		_weaponAudioSource.PlayOneShot(_weaponSoundSwing);
		yield return new WaitForSeconds(0.18f);

		_weaponAudioSource.PlayOneShot(_weaponSoundAttack);

		yield return null;
	}

	public override IEnumerator InspectWeaponAnimation()
	{
		throw new System.NotImplementedException();

		// baton isnt inspected but given straigth away during Bistro fight tutorial
	}




}
using UnityEngine;
using System.Collections;

public abstract class WeaponMeleeAbstract : WeaponAbstract
{
	protected float CapsuleHeight;
	protected float CapsuleRadius;
	protected float ForwardOffset;
	protected float AttackDelay;

	protected GameObject AttackPoint;

	protected bool isAttacking = false;

	private void Start()
	{
		if (IsThisPlayerWeapon == true)
		{
			AttackPoint = ServiceLocator.Resolve<GameObject>("Player");
		}
		
		SetUpMeleeWeapon();
	}

	protected abstract void SetUpMeleeWeapon();

	public override void WeaponAttack()
	{
		isAttacking = true;
		StartCoroutine(PerformAttack());
	}

	private IEnumerator PerformAttack()
	{
		Vector3 startPoint = AttackPoint.transform.position + AttackPoint.transform.forward * ForwardOffset;
		Vector3 endPoint = startPoint + AttackPoint.transform.up * CapsuleHeight;

		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, CapsuleRadius, AttackPoint.transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.gameObject == AttackPoint)
				continue;

			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayDamage(damageable, AttackDelay));
			}
		}

		yield return new WaitForSeconds(AttackDelay + 0.1f);
		isAttacking = false;
	}

	private IEnumerator DelayDamage(IDamageable target, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		target.TakeDamage(WeaponDamage);
	}
}
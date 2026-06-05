using System.Collections;
using UnityEngine;

public abstract class WeaponMeleeAbstract : WeaponAbstract
{
	protected float _capsuleHeight;
	protected float _capsuleRadius;
	protected float _forwardOffset;
	protected float _attackDelay;

	protected GameObject _attackPoint;

	protected bool _isAttacking = false;

	private void Start()
	{
		if (_isThisPlayerWeapon == true)
		{
			_attackPoint = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		}
		
		InitializeWeaponMelee();
	}

	protected abstract void InitializeWeaponMelee();

	public override void WeaponAttack()
	{
		_isAttacking = true;
		StartCoroutine(PerformAttack());
	}

	private IEnumerator PerformAttack()
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
				StartCoroutine(DelayDamage(damageable, _attackDelay));
			}
		}

		yield return new WaitForSeconds(_attackDelay + 0.1f);
		_isAttacking = false;
	}

	private IEnumerator DelayDamage(IDamageable target, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		target.TakeDamage(WeaponDamage);
	}
}
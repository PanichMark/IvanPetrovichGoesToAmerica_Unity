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

	// --- ДОБАВИТЬ ЭТОТ МЕТОД ---
	public override void StartAutoAttacking()
	{
		if (_isWeaponAutoAttacking) return;
		_isWeaponAutoAttacking = true;
		if (_weaponAutoAttackCourutine == null)
		{
			_weaponAutoAttackCourutine = StartCoroutine(AutoAttackCourutine());
		}
	}


	// --- ДОБАВИТЬ ЭТОТ МЕТОД (если его нет) ---
	public override void StopAutoAttacking()
	{
		_isWeaponAutoAttacking = false;
		if (_weaponAutoAttackCourutine != null)
		{
			StopCoroutine(_weaponAutoAttackCourutine);
			_weaponAutoAttackCourutine = null;
		}
	}

	// В классе WeaponMeleeAbstract

	// ... (поля и метод StartAutoAttacking остаются без изменений)

	// --- ИЗМЕНЕННАЯ КОРУТИНА ---
	public override IEnumerator AutoAttackCourutine()
	{
		// Бесконечный цикл для автоатаки
		while (_isWeaponAutoAttacking)
		{
			// Вызываем уже существующую логику одиночного удара
			// Она сама установит _isAttacking = true и вернет его в false после анимации/задержки
			StartCoroutine(MeleeWeaponAttack());

			// Ждем перед следующим ударом
			yield return new WaitForSeconds(_weaponAutoAttackSpeedRate);
		}
		_weaponAutoAttackCourutine = null;
	}

	protected abstract void InitializeWeaponMelee();

	public override void WeaponAttack()
	{
		if (IsWeaponAuto) // Если оружие имеет автоатаку (например, когти с ядом)
		{
			StartAutoAttacking(); // Запускаем корутину, которая будет бить с интервалом
		}
		else // Обычное оружие ближнего боя (меч, топор)
		{
			_isAttacking = true;
			StartCoroutine(MeleeWeaponAttack()); // Запускаем стандартную анимацию/логику удара
		}
	}

	private IEnumerator MeleeWeaponAttack()
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
				StartCoroutine(DelayMeleeDamage(damageable, _attackDelay));
			}
		}

		yield return new WaitForSeconds(_attackDelay + 0.15f);
		_isAttacking = false;
	}

	private IEnumerator DelayMeleeDamage(IDamageable target, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		target.TakeDamage(WeaponDamage);
	}
}
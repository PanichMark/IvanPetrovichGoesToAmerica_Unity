using UnityEngine;
using System.Collections;

public abstract class WeaponMeleeAbstract : WeaponAbstract
{
	// Параметры атаки (задаются в наследниках)
	protected float CapsuleHeight;
	protected float CapsuleRadius;
	protected float ForwardOffset;
	protected float AttackDelay; // Задержка перед нанесением урона

	// Ссылка на игрока, которую мы получаем через ServiceLocator ОДИН РАЗ
	protected GameObject AttackPoint;

	protected bool isAttacking = false; // Флаг для блокировки спама атакой

	// Получаем игрока при старте объекта
	private void Start()
	{
		//Debug.Log(IsThisPlayerWeapon);

		if (IsThisPlayerWeapon == true)
		{
			AttackPoint = ServiceLocator.Resolve<GameObject>("Player");
			//Debug.Log(AttackPoint);
		}

		
		SetUpMeleeWeapon();
	}
	protected abstract void SetUpMeleeWeapon();

	

	public override void WeaponAttack()
	{


		isAttacking = true;
		StartCoroutine(PerformAttack());
	}

	// Основная логика атаки
	private IEnumerator PerformAttack()
	{
		// Определяем точки для капсулы ПЕРЕД ИГРОКОМ,
		// используя transform игрока, который мы получили извне.

		Vector3 startPoint = AttackPoint.transform.position + AttackPoint.transform.forward * ForwardOffset;
		Vector3 endPoint = startPoint + AttackPoint.transform.up * CapsuleHeight;

		// Проверяем все объекты внутри капсулы
		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, CapsuleRadius, AttackPoint.transform.forward, 0f);

		foreach (RaycastHit hit in hits)
		{
			// Пропускаем самого игрока
			if (hit.collider.gameObject == AttackPoint)
				continue;

			// Проверяем, можно ли нанести урон
			if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
			{
				StartCoroutine(DelayDamage(damageable, AttackDelay));
			}
		}

		// Ждем окончания атаки и разблокируем возможность бить снова
		yield return new WaitForSeconds(AttackDelay + 0.1f);
		isAttacking = false;
	}

	// Корутина для задержки урона
	private IEnumerator DelayDamage(IDamageable target, float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		target.TakeDamage(WeaponDamage);
	}
}
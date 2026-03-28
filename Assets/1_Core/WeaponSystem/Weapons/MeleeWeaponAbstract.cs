using UnityEngine;
using System.Collections;

public abstract class MeleeWeaponAbstract : WeaponAbstract
{
	// Параметры атаки (задаются в наследниках)
	protected float CapsuleHeight;
	protected float CapsuleRadius;
	protected float ForwardOffset;
	protected float AttackDelay; // Задержка перед нанесением урона

	// Ссылка на игрока, которую мы получаем через ServiceLocator ОДИН РАЗ
	private GameObject player;

	private bool isAttacking = false; // Флаг для блокировки спама атакой

	// Получаем игрока при старте объекта
	private void Start()
	{
		player = ServiceLocator.Resolve<GameObject>("Player");

		
		SetUpAttackRadious();
	}
	protected abstract void SetUpAttackRadious();


	public override void WeaponAttack()
	{
		// Если игрок не найден или атака уже идет - выходим
		if (player == null || isAttacking) return;

		isAttacking = true;
		StartCoroutine(PerformAttack());
	}

	// Основная логика атаки
	private IEnumerator PerformAttack()
	{
		// Определяем точки для капсулы ПЕРЕД ИГРОКОМ,
		// используя transform игрока, который мы получили извне.
		Vector3 playerPosition = player.transform.position;
		Vector3 playerForward = player.transform.forward;

		Vector3 startPoint = playerPosition + playerForward * ForwardOffset;
		Vector3 endPoint = startPoint + player.transform.up * CapsuleHeight;

		// Проверяем все объекты внутри капсулы
		RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, CapsuleRadius, playerForward, 0f);

		foreach (RaycastHit hit in hits)
		{
			// Пропускаем самого игрока
			if (hit.collider.gameObject == player)
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
using UnityEngine;
using System.Collections;

public class WeaponPoliceBaton : WeaponAbstract
{
	PlayerMovementController playerMovementController;



	// Высота и радиус капсулы
	float CapsuleHeight;     // Высота капсулы (примерное расстояние вдоль оси Y)
	float CapsuleRadius;   // Радиус капсулы
	float ForwardOffset;    // Смещение вперёд от центра игрока

	public override float WeaponDamage => 100f; // Устанавливаем постоянное значение урона для револьвера
	public override string  WeaponNameSystem => "PoliceBaton";
	public override string WeaponNameUI => "Милицейская Дубинка";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Baton icon");


	//public bool IsPlayerPoliceBatonAttacking;
	private void Start()
	{
		playerMovementController = GetComponent<PlayerMovementController>();

		CapsuleHeight = 1.8f;      // Высота капсулы (примерное расстояние вдоль оси Y)
		CapsuleRadius = 0.3f;      // Радиус капсулы
		ForwardOffset = 0.5f;      // Смещение вперёд от центра игрока
	}

	



	public override void WeaponAttack()
	{
		Debug.Log("PoliceBaton Attack");
		/*
		if (IsPlayerPoliceBatonAttacking == false)
		{

			Debug.Log("PoliceBatonAttack");

			StartCoroutine(DisablePoliceBatonAttackActivation());
			// Нижняя и верхняя точки капсулы
			Vector3 startPoint = transform.position + transform.forward * ForwardOffset;
			Vector3 endPoint = transform.position + transform.forward * ForwardOffset + transform.up * CapsuleHeight;

			// Физически проверяем объекты, захваченные капсулой
			RaycastHit[] hits = Physics.CapsuleCastAll(startPoint, endPoint, CapsuleRadius, transform.forward, 0f);

			foreach (RaycastHit hit in hits)
			{
				if (hit.collider.gameObject.CompareTag("Player"))
					continue;

				if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
				{
					StartCoroutine(DelayPoliceBatonAttackDamage(damageable, 0.5f, WeaponDamage));
				}
			}

		}
		*/
	}

	IEnumerator DisablePoliceBatonAttackActivation()
	{
		//IsPlayerPoliceBatonAttacking = true;
		yield return new WaitForSeconds(1f);
		//IsPlayerPoliceBatonAttacking = false;
	}


	IEnumerator DelayPoliceBatonAttackDamage(IDamageable target, float delayTime, float damageAmount)
	{
		yield return new WaitForSeconds(delayTime); // Ждем нужную задержку

		// Наносим урон после окончания ожидания
		target.TakeDamage(damageAmount);
	}

}



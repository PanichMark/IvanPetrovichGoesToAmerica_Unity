using UnityEngine;

public class WeaponEugenicGenie : EugenicWeaponAbstract
{
	

	float attackRange = 5f; // Радиус области поражения
	float knockbackForce = 10f; // Сила отталкивания
	int damageAmount = 100; // Количество урона
	public override string WeaponNameSystem => "EugenicGenie";
	public override string WeaponNameUI => "Дыхание Джинна";

	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Jinny icon");

	protected override void InitializeWeaponEugenic()
	{

		ManaCost = 10;


	}



	public override void WeaponAttack()
	{
		if (playerResourcesManaManager.CurrentPlayerMana >= ManaCost)
		{
			playerResourcesManaManager.UseMana(ManaCost);
			// --- Настройки атаки ---
			// (Ваши переменные attackRange, knockbackForce и damageAmount уже объявлены в классе)

			// 1. Определяем центр сферы атаки ОТ ИГРОКА.
			// Это точка, из которой мы ищем цели для урона.
			Vector3 attackOrigin = player.transform.position + player.transform.forward * 1.5f;

			// 2. Находим все коллайдеры в радиусе от игрока.
			Collider[] hitColliders = Physics.OverlapSphere(attackOrigin, attackRange);

			// --- Этап 1: Нанесение урона ---
			foreach (Collider hit in hitColliders)
			{
				IDamageable damageable = hit.GetComponent<IDamageable>();
				if (damageable != null)
				{
					damageable.TakeDamage(damageAmount);
					Debug.Log($"Нанесено {damageAmount} урона объекту: {hit.name}");
				}
			}

			// --- Этап 2: Отталкивание объектов ---
			foreach (Collider hit in hitColliders)
			{
				Rigidbody rb = hit.GetComponent<Rigidbody>();
				if (rb != null && !rb.isKinematic)
				{
					// 3. Рассчитываем НОВОЕ направление с учётом наклона камеры (поворот по оси X).
					// Используем камеру для определения вектора "вперёд".
					Vector3 knockbackDirection = camera.transform.forward.normalized;

					// Применяем силу для отталкивания в этом направлении.
					// Сила направлена ОТ игрока, так как мы применяем её к объекту, который перед нами.
					rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
					Debug.Log($"Отброшен Rigidbody: {hit.name}");
				}
			}
		}
		else
		{
			Debug.Log($"Not enough mana for {WeaponNameSystem} attack");
		}
	}
		
}



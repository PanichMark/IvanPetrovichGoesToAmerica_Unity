using UnityEngine;

public class WeaponPoliceBaton : MeleeWeaponAbstract
{
	// Свойства оружия
	public override float WeaponDamage => 45f;
	public override string WeaponNameSystem => "PoliceBaton";
	public override string WeaponNameUI => "Милицейская Дубинка";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Baton icon");

	protected override void SetUpAttackRadious()
	{
		CapsuleHeight = 1.8f;
		CapsuleRadius = 0.3f;
		ForwardOffset = 0.5f;
		AttackDelay = 0.5f;
	}

	/*
	private void OnDrawGizmos()
	{
		// Проверяем, что все параметры заданы
		if (CapsuleHeight <= 0 || CapsuleRadius <= 0) return;

		Gizmos.color = new Color(1, 0.7f, 0, 0.6f); // Оранжевый полупрозрачный

		// --- ИЗМЕНЕНИЕ: Используем позицию родителя (ИГРОКА) ---
		// Это нужно, чтобы гизмо отображался там же, где происходит реальная проверка в коде.
		Transform playerTransform = transform.root;
		if (playerTransform == null) return;

		Vector3 playerPosition = playerTransform.position;
		Vector3 playerForward = playerTransform.forward;

		// Точки капсулы перед игроком
		Vector3 startPoint = playerPosition + playerForward * ForwardOffset;
		Vector3 endPoint = startPoint + playerTransform.up * CapsuleHeight;

		// --- ПРОСТОЙ СПОСОБ ОТРИСОВКИ ---
		// Рисуем центральную линию для наглядности
		Gizmos.DrawLine(startPoint, endPoint);

		// Рисуем сферы на концах
		Gizmos.DrawWireSphere(startPoint, CapsuleRadius);
		Gizmos.DrawWireSphere(endPoint, CapsuleRadius);
	}
	*/
}
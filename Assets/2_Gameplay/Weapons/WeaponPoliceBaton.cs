using UnityEngine;

public class WeaponPoliceBaton : MeleeWeaponAbstract
{
	// Свойства оружия
	public override float WeaponDamage => 45f;
	public override string WeaponNameSystem => "PoliceBaton";
	public override string WeaponNameUI => "Милицейская Дубинка";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/Baton icon");

	private GameObject ChokeNPCtext;
	// Состояние окружения
	private bool npcDetected = false;
	// Ссылка на контроллер движения игрока
	private PlayerMovementController playerMovementController;
	// Новое поле: флаг возможности удушения
	private bool isAbleToChoke = false;

	protected override void SetUpAttackRadious()
	{
		CapsuleHeight = 1.8f;
		CapsuleRadius = 0.3f;
		ForwardOffset = 0.5f;
		AttackDelay = 0.5f;
	}

	protected override void PoliceBatonChoke()
	{
		ChokeNPCtext = ServiceLocator.Resolve<GameObject>("ChokeNPCtext");
		playerMovementController = ServiceLocator.Resolve<PlayerMovementController>("PlayerMovementController");

	}

	private void Update()
	{
	

		// --- ЛОГИКА ОБНАРУЖЕНИЯ NPC ---
		Vector3 playerPosition = player.transform.position;
		Vector3 playerForward = player.transform.forward;

		Vector3 startPoint = playerPosition + playerForward * ForwardOffset;
		Vector3 endPoint = startPoint + player.transform.up * CapsuleHeight;

		Collider[] hitColliders = Physics.OverlapCapsule(startPoint, endPoint, CapsuleRadius);

		bool newDetection = false;
		foreach (var hit in hitColliders)
		{
			if (hit.gameObject == player) continue;
			if (hit.GetComponent<NPCAbstract>() != null)
			{
				newDetection = true;
				break;
			}
		}
		npcDetected = newDetection;


		// --- ЛОГИКА ПРОВЕРКИ СОСТОЯНИЯ ИГРОКА ---
		// Проверяем, присел ли игрок и есть ли NPC рядом.
		// Предполагаем, что CurrentPlayerMovementStateType - это строка или enum.
		// Для строк используем .Equals для надежности.
		bool isCrouching = playerMovementController.CurrentPlayerMovementStateType.Equals("PlayerCrouchingIdle") ||
						   playerMovementController.CurrentPlayerMovementStateType.Equals("PlayerCrouchingWalking");

		isAbleToChoke = npcDetected && isCrouching;


		// Обновляем видимость текста, если он был успешно получен
		if (ChokeNPCtext != null)
			ChokeNPCtext.SetActive(isAbleToChoke); // Показываем текст только если можно душить
	}

	// НОВЫЙ МЕТОД для удушения
	private void PerformChokeAttack()
	{
		Debug.Log("CHOKING!!!");

		// Здесь можно добавить логику реального удушения:
		// 1. Найти конкретного NPC в радиусе.
		// 2. Запустить на нем анимацию или состояние "Choked".
		// 3. Отключить его ИИ на время.
	}

	// ПЕРЕОПРЕДЕЛЯЕМ базовый метод атаки
	public override void WeaponAttack()
	{
		// Если условия для удушения выполнены - используем спец. атаку
		if (isAbleToChoke)
		{
			PerformChokeAttack();
			return; // Выходим, чтобы не вызывать базовую атаку дубинкой
		}

		// Если условия не выполнены, вызываем обычную атаку из базового класса
		base.WeaponAttack();
	}
}
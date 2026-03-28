using UnityEngine;
using UnityEngine.AI;

public class WeaponPlungerCrossbow : WeaponAbstract
{
	// Настройки крюка
	private float maxHookDistance = 25f;
	private float pullSpeed = 10f;

	private NPCStateMachineController npcStateMachineController;
	// Состояние крюка для NPC
	private GameObject hookedObject = null;
	private Rigidbody hookedObjectRigidbody = null;
	private bool isObjectBeingHooked = false;
	private Collider hookedObjectCollider;
	private NavMeshAgent hookedObjectNavMeshAgent = null;

	// Ссылки на объекты
	private GameObject playerCamera;
	private GameObject player;
	private Rigidbody playerRigidbody;

	private GameSceneManager gameSceneManager;
	private PlayerBehaviour playerBehaviour;
	// Состояние крюка
	private Vector3 hookPoint;
	private bool IsPlayerPlungering = false;
	private GameController gameController;
	public override string WeaponNameSystem => "PlungerCrossbow";
	private GameObject playerCollider;
	private Collider collider;
	public override string WeaponNameUI => "Абордажный Арбалет";
	public override Sprite WeaponIcon => Resources.Load<Sprite>("WeaponWheelButtons/CrossBow icon");
	public override float WeaponDamage => 0; // Урон не нужен

	private void Start()
	{
		playerCamera = ServiceLocator.Resolve<GameObject>("playerMainCameraGameObject");
		player = ServiceLocator.Resolve<GameObject>("Player");
		playerRigidbody = player.GetComponent<Rigidbody>();
		gameController = ServiceLocator.Resolve<GameController>("GameController");
		playerCollider = ServiceLocator.Resolve<GameObject>("playerColliderGameObject");
		collider = playerCollider.GetComponent<Collider>();
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		playerBehaviour = ServiceLocator.Resolve<PlayerBehaviour>("PlayerBehaviour");

		gameSceneManager.OnBeginLoadMainMenuScene += StopPlungingCompletely;
		gameSceneManager.OnBeginLoadGameplayScene += StopPlungingCompletely;
		playerBehaviour.OnPlayerDisarmed += StopPlunging;
	}

	public override void WeaponAttack()
	{
		// Если уже летим на крюке, новый не выпускаем
		if (IsPlayerPlungering) return;

		if (playerCamera == null || player == null || playerRigidbody == null) return;

		Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
		RaycastHit hit;

		// Стреляем в любую поверхность
		if (Physics.Raycast(ray, out hit, maxHookDistance))
		{
			hookPoint = hit.point;

			// Проверяем, попал ли крюк в NPC
			// Проверяем, есть ли у объекта компонент NPCAbstract
			if (hit.collider.gameObject.TryGetComponent<NPCAbstract>(out _) ||
				hit.collider.gameObject.TryGetComponent<InteractionObjectPickableAbstract>(out _))
			{
				hookedObject = hit.collider.gameObject;
				hookedObjectRigidbody = hookedObject.GetComponent<Rigidbody>();
				hookedObjectCollider = hookedObject.GetComponent<Collider>();


				// Если Rigidbody нет, добавляем его (логика остается прежней)
				if (hookedObjectRigidbody == null)
				{
					hookedObjectRigidbody = hookedObject.AddComponent<Rigidbody>();
					Debug.LogWarning($"Rigidbody добавлен к NPC: {hookedObject.name}");
			
				}
				hookedObjectNavMeshAgent = hookedObject.GetComponent<NavMeshAgent>();
				if (hookedObjectNavMeshAgent != null)
				{

					hookedObjectNavMeshAgent.enabled = false;
				}

				npcStateMachineController = hookedObject.GetComponent<NPCStateMachineController>();
				if (npcStateMachineController != null)
				{
					////
					//////
					//ПОТОМ ПОМЕНЯТЬ НА BEING HOOKED !!!
					//////
					///

					npcStateMachineController.SetNPCState(NPCStateTypes.Dead);
				}

				hookedObjectRigidbody.useGravity = false;
				hookedObjectRigidbody.linearDamping = 0;


				isObjectBeingHooked = true;
				IsPlayerPlungering = false;
				Debug.Log("Крюк зацепил NPC!");
			}
			else // Если попали в любой другой объект (стена, пол)
			{
				playerCollider.SetActive(false);
				IsPlayerPlungering = true; // Мы притягиваемся сами
				playerRigidbody.useGravity = false;
				isObjectBeingHooked = false;
				Debug.Log($"Крюк зацепился за: {hit.collider.name}");
			}
		}


		// Отключаем стандартную гравитацию, чтобы она не мешала притяжению
		
	}

	// Этот метод вызывается каждый кадр для объектов с Rigidbody
	private void FixedUpdate()
	{
		// ПРИТЯГИВАЕМ NPC (если зацепили его)
		if (isObjectBeingHooked)
		{
			// 1. Вычисляем финальную позицию
			Vector3 finalPosition = player.transform.position;
			finalPosition.y += 0.5f;
			finalPosition += player.transform.forward * 1.5f;

			// 2. Вычисляем направление к этой позиции
			Vector3 directionToTarget = (finalPosition - hookedObject.transform.position).normalized;

			// 3. Двигаем объект
			hookedObjectRigidbody.linearVelocity = directionToTarget * pullSpeed;

			// Проверяем расстояние до ФИНАЛЬНОЙ точки, а не до игрока
			float distanceToTarget = Vector3.Distance(hookedObject.transform.position, finalPosition);

			if (distanceToTarget < 0.5f) // Можно уменьшить порог, так как точка выше
			{
				hookedObjectRigidbody.linearVelocity = Vector3.zero;
				hookedObjectRigidbody.position = finalPosition; // Ставим точно в цель

				StopHookingObject();
			}
		}

		// Проверяем, нужно ли притягивать игрока
		if (IsPlayerPlungering)
		{
			gameController.StartPlunging();
			// Находим направление от игрока к точке крюка
			Vector3 directionToHook = (hookPoint - player.transform.position).normalized;

			// Применяем силу притяжения
			playerRigidbody.linearVelocity = directionToHook * pullSpeed;

			// Проверяем, достаточно ли близко мы прилетели
			float distanceToHook = Vector3.Distance(player.transform.position, hookPoint);

			// Если мы близко ИЛИ если скорость стала очень маленькой (значит мы уперлись в препятствие)
			if (distanceToHook < 1f || playerRigidbody.linearVelocity.magnitude < 0.1f)
			{
				// Останавливаем игрока точно в точке крюка
				player.transform.position = hookPoint;
				playerRigidbody.linearVelocity = Vector3.zero;

				StopPlunging();
			}
		}
	}
	private void StopHookingObject()
	{
		if (isObjectBeingHooked)
		{
			if (hookedObjectNavMeshAgent != null)
			{
			//	hookedObjectNavMeshAgent.enabled = true;
			}

			hookedObjectRigidbody.linearVelocity = Vector3.zero;

			// Включаем гравитацию обратно для NPC, если она была отключена
			hookedObjectRigidbody.useGravity = true;
		//	Destroy(hookedObjectRigidbody);
			

			hookedObject = null;
			hookedObjectRigidbody = null;
			isObjectBeingHooked = false;
			Debug.Log("Притяжение NPC завершено.");
		}
	}

	private void OnDestroy()
	{
		gameSceneManager.OnBeginLoadMainMenuScene -= StopPlungingCompletely;
		gameSceneManager.OnBeginLoadGameplayScene -= StopPlungingCompletely;
		playerBehaviour.OnPlayerDisarmed -= StopPlunging;

		StopPlunging();
	}

	private void StopPlunging()
	{
		if (IsPlayerPlungering)
		{
			// Останавливаем игрока точно в точке крюка
			//playerRigidbody.linearVelocity = Vector3.zero;

			// Включаем гравитацию обратно
			playerRigidbody.useGravity = true;

			IsPlayerPlungering = false;
			Debug.Log("Притяжение завершено.");
			gameController.StopPlunging();
			playerCollider.SetActive(true);
		}
	}
	private void StopPlungingCompletely()
	{
		playerRigidbody.linearVelocity = Vector3.zero;
		StopPlunging();
		
	}
}
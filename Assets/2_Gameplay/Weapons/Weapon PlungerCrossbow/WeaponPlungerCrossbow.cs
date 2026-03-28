using UnityEngine;

public class WeaponPlungerCrossbow : WeaponAbstract
{
	// Настройки крюка
	private float maxHookDistance = 50f;
	private float pullSpeed = 10f;

	// Ссылки на объекты
	private GameObject playerCamera;
	private GameObject player;
	private Rigidbody playerRigidbody;

	// Состояние крюка
	private Vector3 hookPoint;
	private bool isHooked = false;
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
	}

	public override void WeaponAttack()
	{
		// Если уже летим на крюке, новый не выпускаем
		if (isHooked) return;

		if (playerCamera == null || player == null || playerRigidbody == null) return;

		Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
		RaycastHit hit;

		// Стреляем в любую поверхность
		if (Physics.Raycast(ray, out hit, maxHookDistance))
		{
			hookPoint = hit.point;
			isHooked = true;
			Debug.Log($"Крюк зацепился за: {hit.collider.name}");
			playerRigidbody.useGravity = false;
			playerCollider.SetActive(false);
		}


		// Отключаем стандартную гравитацию, чтобы она не мешала притяжению
		
	}

	// Этот метод вызывается каждый кадр для объектов с Rigidbody
	private void FixedUpdate()
	{
		// Проверяем, нужно ли притягивать игрока
		if (isHooked)
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
	private void OnDestroy()
	{
		StopPlunging();
	}

	private void StopPlunging()
	{
		if (isHooked)
		{
			// Останавливаем игрока точно в точке крюка
			//playerRigidbody.linearVelocity = Vector3.zero;

			// Включаем гравитацию обратно
			playerRigidbody.useGravity = true;

			isHooked = false;
			Debug.Log("Притяжение завершено.");
			gameController.StopPlunging();
			playerCollider.SetActive(true);
		}
	}
}
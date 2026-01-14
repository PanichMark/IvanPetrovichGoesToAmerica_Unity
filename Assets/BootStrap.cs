using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class BootStrap : MonoBehaviour
{
	private IInputDevice inputDevice;

	[SerializeField] private GameObject player;
	[SerializeField] private GameObject playerCamera;
	[SerializeField] private GameObject weaponSystem;
	[SerializeField] private GameObject weaponWheelCanvas;

	[SerializeField] private MenuManager menuManager;

	private PlayerBehaviour playerBehaviour;
	private PlayerMovementController movementController;
	private PlayerCapluseCollider playerCollider;

	private PlayerCameraController cameraController;
	private PlayerCameraBlurFilter cameraBlurFilter;

	private WeaponController weaponController;
	private WeaponWheelController weaponWheelController;

	private PlayerCameraFirstPersonRender firstPersonRender;
	private GameObject PlayerFirstPersonHandRight;
	private GameObject PlayerFirstPersonHandLeft;
	private GameObject PlayerHeadParent;
	private GameObject PlayerHandRightParent;
	private GameObject PlayerHandLeftParent;


	private PlayerAnimationController playerAnimationController;
	private InteractionController interactionController;

	private GameObject wheelSegmentPrefab;          // Префаб сегмента
	private Transform centerPoint;                  // Центр круга
	private TextMeshProUGUI WeaponText;             // Текущий выбор оружия
	private TextMeshProUGUI WeaponWheelName;        // Название меню (левая/правая рука)
													// Метод для рекурсивного поиска объекта по имени
	private GameObject FindDeepChildByName(GameObject root, string targetName)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(root.transform);

		while (queue.Count > 0)
		{
			Transform current = queue.Dequeue();
			if (current.name == targetName)
				return current.gameObject;

			foreach (Transform child in current)
			{
				queue.Enqueue(child);
			}
		}

		return null;
	}
	// Простая обычная инициализация без корутин
	private void SequentialInitialization()
	{
		// Создание устройства ввода
		inputDevice = new InputKeyboard();

		// Загрузка ресурсов
		wheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButton");
		centerPoint = weaponWheelCanvas.transform.Find("Centre")?.transform;
		WeaponText = weaponWheelCanvas.transform.Find("Selected Weapon Name")?.GetComponent<TextMeshProUGUI>();
		WeaponWheelName = weaponWheelCanvas.transform.Find("WeaponWheel Hand")?.GetComponent<TextMeshProUGUI>();

		// ИНСТАНТИРУЕМ ОБЪЕКТЫ ПЕРЕД ВСЕМИ ОПЕРАЦИЯМИ
		player = Instantiate(player);
		playerCamera = Instantiate(playerCamera);
		weaponSystem = Instantiate(weaponSystem);
		weaponWheelCanvas = Instantiate(weaponWheelCanvas);


		// НАХОДИМ НУЖНЫЕ GAMEOBJECTS ПО ИМЕНАМ
		PlayerFirstPersonHandRight = FindDeepChildByName(playerCamera, "UNITY HandRight");
		PlayerFirstPersonHandLeft = FindDeepChildByName(playerCamera, "UNITY  HandLeft");
		PlayerHeadParent = FindDeepChildByName(player, "UNITY PlayerHead");
		PlayerHandRightParent = FindDeepChildByName(player, "UNITY HandRight");
		PlayerHandLeftParent = FindDeepChildByName(player, "UNITY  HandLeft");

		// Получить компоненты ПОСЛЕ инстанцирования
		playerBehaviour = player.GetComponent<PlayerBehaviour>();
		movementController = player.GetComponent<PlayerMovementController>();
		playerCollider = player.GetComponentInChildren<PlayerCapluseCollider>();
		cameraController = playerCamera.GetComponent<PlayerCameraController>();
		cameraBlurFilter = playerCamera.GetComponent<PlayerCameraBlurFilter>();
		weaponController = weaponSystem.GetComponent<WeaponController>();
		weaponWheelController = weaponSystem.GetComponent<WeaponWheelController>();
		firstPersonRender = playerCamera.GetComponent<PlayerCameraFirstPersonRender>();

		// ИНЦИАЛИЗАЦИЯ КОМПОНЕНТОВ
		menuManager.Initialize(inputDevice);
		playerBehaviour.Initialize(inputDevice);
		movementController.Initialize(inputDevice, playerBehaviour);
		playerCollider.Initialize(movementController);
		cameraController.Initialize(inputDevice, menuManager, movementController, playerCollider, player);
		cameraBlurFilter.Initialize(menuManager);
		weaponController.Initialize(inputDevice, menuManager, playerBehaviour);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController, wheelSegmentPrefab, centerPoint, weaponWheelCanvas, WeaponText, WeaponWheelName);

		firstPersonRender.Initialize(cameraController, weaponController,
							 PlayerFirstPersonHandRight, PlayerFirstPersonHandLeft,
							 PlayerHeadParent, PlayerHandRightParent, PlayerHandLeftParent);
		//playerAnimationController.Initialize(inputDevice, player, playerBehaviour, movementController, cameraController, weaponController);
		//interactionController.Initialize(inputDevice, cameraController, playerBehaviour);

		// Поднимаем флаг только после завершения всех шагов


		Debug.Log("Все компоненты успешно инициализированы!");
	}

	// Main entry point
	private void Awake()
	{
		SequentialInitialization();
	}

	void Start()
	{
	}

	void Update()
	{
	}
}
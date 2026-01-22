using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BootStrap : MonoBehaviour
{
	// === Устройства ввода и управление ===
	private IInputDevice inputDevice;
	private IGameController gameController;

	// === Менеджеры и контроллеры высокого уровня ===
	private MenuManager menuManager;
	private PauseMenuController pauseMenuController;
	private InteractionController interactionController;
	private PauseSubMenuImagesController imagesSubMenuController;

	// === Камера и отображение игрока ===
	private PlayerCameraController cameraController;
	private PlayerCameraBlurFilter cameraBlurFilter;
	private PlayerCameraFirstPersonRender firstPersonRender;
	private GameObject playerFirstPersonHandRight;
	private GameObject playerFirstPersonHandLeft;
	private GameObject playerHeadParent;
	private GameObject playerHandRightParent;
	private GameObject playerHandLeftParent;

	// === Игрок и движение ===
	private PlayerBehaviour playerBehaviour;
	private PlayerMovementController movementController;
	private PlayerCapsuleCollider playerCollider;
	private PlayerAnimationController playerAnimationController;

	// === Система оружия ===
	private WeaponController weaponController;
	private WeaponWheelController weaponWheelController;

	// === Канвасы и пользовательские интерфейсы ===
	[SerializeField] private GameObject weaponWheelCanvas;
	[SerializeField] private GameObject canvasImagesSubMenu;
	[SerializeField] private GameObject menuManagerGameobject;
	[SerializeField] private GameObject pauseMenuControllerGameObject;
	[SerializeField] private GameObject interactionControllerGameObject;
	[SerializeField] private GameObject interactionCanvas;
	[SerializeField] private GameObject pauseMenuCanvas;

	// === UI-компоненты ===
	private TextMeshProUGUI mainInteractionText;
	private TextMeshProUGUI additionalInteractionText;
	private Button exitInteraction;
	private TextMeshProUGUI readableText;
	private Image backgroundBlack;
	private TextMeshProUGUI item1Text;
	private TextMeshProUGUI item2Text;
	private TextMeshProUGUI item3Text;
	private Image item1Image;
	private Image item2Image;
	private Image item3Image;
	private Image imageNewspaper;
	private GameObject buttonImagesSubMenu;
	private GameObject wheelSegmentPrefab;
	private Transform centerPoint;
	private TextMeshProUGUI weaponText;
	private TextMeshProUGUI weaponWheelName;

	// === Сериализованные игровые объекты ===
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject playerCamera;
	[SerializeField] private GameObject weaponSystem;

	// Дополнительные методы

	// Поиск дочернего объекта по имени
	private GameObject FindDeepChildByName(GameObject root, string targetName)
	{
		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(root.transform);

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();

			if (current.name == targetName)
				return current.gameObject;

			foreach (Transform child in current)
			{
				queue.Enqueue(child);
			}
		}

		return null;
	}

	// Инициализация последовательности действий
	private IEnumerator SequentialInitialization()
	{
		yield return StartCoroutine(InitializeCoreObjects());
		yield return StartCoroutine(SetupUIElements());
		yield return StartCoroutine(RegisterDependencies());
		//yield return StartCoroutine(LoadNextScene());

		Debug.Log("Все компоненты успешно инициализированы!");
	}

	// Создание основных игровых объектов
	private IEnumerator InitializeCoreObjects()
	{
		yield return new WaitForEndOfFrame(); // Ожидание конца текущего кадра
		gameController = new GameController();
		inputDevice = new InputKeyboard(gameController);

		// Инстанцирование объектов
		weaponWheelCanvas = Instantiate(weaponWheelCanvas);
		interactionCanvas = Instantiate(interactionCanvas);
		pauseMenuCanvas = Instantiate(pauseMenuCanvas);
		canvasImagesSubMenu = Instantiate(canvasImagesSubMenu);
		player = Instantiate(player);
		playerCamera = Instantiate(playerCamera);
		weaponSystem = Instantiate(weaponSystem);
		pauseMenuControllerGameObject = Instantiate(pauseMenuControllerGameObject);
		interactionControllerGameObject = Instantiate(interactionControllerGameObject);
		menuManagerGameobject = Instantiate(menuManagerGameobject);

		Debug.Log("Основные игровые объекты созданы.");
		yield break; // Возвращаемся назад
	}

	// Настройка элементов интерфейса и их компонентов
	private IEnumerator SetupUIElements()
	{
		// Поиск элементов интерфейса
		buttonImagesSubMenu = FindDeepChildByName(pauseMenuCanvas, "PauseMenu Images Button");
		wheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButton");
		centerPoint = weaponWheelCanvas.transform.Find("Centre")?.transform;
		weaponText = weaponWheelCanvas.transform.Find("Selected Weapon Name")?.GetComponent<TextMeshProUGUI>();
		weaponWheelName = weaponWheelCanvas.transform.Find("WeaponWheel Hand")?.GetComponent<TextMeshProUGUI>();

		mainInteractionText = interactionCanvas.transform.Find("mainInteractionText")?.GetComponent<TextMeshProUGUI>();
		additionalInteractionText = interactionCanvas.transform.Find("additionalInteractionText")?.GetComponent<TextMeshProUGUI>();
		item1Text = interactionCanvas.transform.Find("Item1text")?.GetComponent<TextMeshProUGUI>();
		item2Text = interactionCanvas.transform.Find("Item2text")?.GetComponent<TextMeshProUGUI>();
		item3Text = interactionCanvas.transform.Find("Item3text")?.GetComponent<TextMeshProUGUI>();
		item1Image = interactionCanvas.transform.Find("Image1Icon")?.GetComponent<Image>();
		item2Image = interactionCanvas.transform.Find("Image2Icon")?.GetComponent<Image>();
		item3Image = interactionCanvas.transform.Find("Image3Icon")?.GetComponent<Image>();
		exitInteraction = interactionCanvas.transform.Find("ExitInteraction")?.GetComponent<Button>();
		imageNewspaper = interactionCanvas.transform.Find("ReadableImage")?.GetComponent<Image>();
		readableText = interactionCanvas.transform.Find("ReadableText")?.GetComponent<TextMeshProUGUI>();
		backgroundBlack = interactionCanvas.transform.Find("BackgroundBlack")?.GetComponent<Image>();

		Debug.Log("Элементы интерфейса настроены.");
		yield break; // Возвращаемся назад
	}

	// Регистрация зависимостей и компонентов
	private IEnumerator RegisterDependencies()
	{
		// Назначение внутренних объектов камеры и рук
		playerFirstPersonHandRight = FindDeepChildByName(playerCamera, "UNITY HandRight");
		playerFirstPersonHandLeft = FindDeepChildByName(playerCamera, "UNITY  HandLeft");
		playerHeadParent = FindDeepChildByName(player, "UNITY PlayerHead");
		playerHandRightParent = FindDeepChildByName(player, "UNITY HandRight");
		playerHandLeftParent = FindDeepChildByName(player, "UNITY  HandLeft");

		// Получение компонентов игроков
		playerBehaviour = player.GetComponent<PlayerBehaviour>();
		movementController = player.GetComponent<PlayerMovementController>();
		playerCollider = player.GetComponentInChildren<PlayerCapsuleCollider>();
		cameraController = playerCamera.GetComponent<PlayerCameraController>();
		cameraBlurFilter = playerCamera.GetComponent<PlayerCameraBlurFilter>();
		weaponController = weaponSystem.GetComponent<WeaponController>();
		weaponWheelController = weaponSystem.GetComponent<WeaponWheelController>();
		playerAnimationController = player.GetComponent<PlayerAnimationController>();
		firstPersonRender = playerCamera.GetComponent<PlayerCameraFirstPersonRender>();
		imagesSubMenuController = pauseMenuControllerGameObject.GetComponent<PauseSubMenuImagesController>();
		pauseMenuController = pauseMenuControllerGameObject.GetComponent<PauseMenuController>();
		interactionController = interactionControllerGameObject.GetComponent<InteractionController>();
		menuManager = menuManagerGameobject.GetComponent<MenuManager>();

		// Регистрация и инициализация контроллеров
		menuManager.Initialize(inputDevice, gameController);
		pauseMenuController.Initialize(inputDevice, menuManager, pauseMenuCanvas, buttonImagesSubMenu);
		imagesSubMenuController.Initialize(inputDevice, menuManager, pauseMenuController, canvasImagesSubMenu);
		playerBehaviour.Initialize(inputDevice);
		movementController.Initialize(inputDevice, playerBehaviour);
		playerCollider.Initialize(movementController);
		cameraController.Initialize(inputDevice, menuManager, movementController, playerCollider, player);
		cameraBlurFilter.Initialize(menuManager);
		weaponController.Initialize(inputDevice, menuManager, playerBehaviour);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController, wheelSegmentPrefab, centerPoint, weaponWheelCanvas, weaponText, weaponWheelName);
		firstPersonRender.Initialize(cameraController, weaponController, playerFirstPersonHandRight, playerFirstPersonHandLeft, playerHeadParent, playerHandRightParent, playerHandLeftParent);
		playerAnimationController.Initialize(inputDevice, player, playerBehaviour, movementController, cameraController, weaponController);
		interactionController.Initialize(inputDevice, cameraController, playerBehaviour, mainInteractionText, additionalInteractionText, item1Text, item2Text, item3Text, item1Image, item2Image, item3Image);

		// Регистрация служб
		ServiceLocator.Register("Player", player);
		ServiceLocator.Register("MenuManager", menuManager);
		ServiceLocator.Register("WeaponController", weaponController);
		ServiceLocator.Register("ExitInteraction", exitInteraction);
		ServiceLocator.Register("ImageNewspaper", imageNewspaper);
		ServiceLocator.Register("ReadableText", readableText);
		ServiceLocator.Register("BackgroundBlack", backgroundBlack);

		Debug.Log("Зависимости зарегистрированы.");
		yield break; // Возвращаемся назад
	}

	// Загрузка дополнительной сцены
	private IEnumerator LoadNextScene()
	{
		yield return null;
		Debug.Log("Дополнительная сцена загружена!");
	}

	// Запуск главной точки входа
	private void Awake()
	{
		StartCoroutine(SequentialInitialization());
	}

	// Освобождение ресурсов при завершении приложения
	private void OnApplicationQuit()
	{
		ServiceLocator.ClearServices();
	}
}
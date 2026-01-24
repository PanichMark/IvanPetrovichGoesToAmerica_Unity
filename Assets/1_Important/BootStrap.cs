using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BootStrap : MonoBehaviour
{
	//Интерфейсы
	private IInputDevice inputDevice;
	private IGameController gameController;

	//Система Сохранений
	private GameObject dataPersistenceManagerGameObject;
	private DataPersistenceManager dataPersistenceManager;

	//Игрок
	private GameObject playerGameObject;
	private GameObject playerHeadParent;
	private GameObject playerHandRightParent;
	private GameObject playerHandLeftParent;
	private GameObject playerFirstPersonHandRight;
	private GameObject playerFirstPersonHandLeft;
	//Игрок Системы
	private PlayerBehaviour playerBehaviour;
	private PlayerMovementController movementController;
	private PlayerCapsuleCollider playerCollider;
	private PlayerAnimationController playerAnimationController;
	//Игрок Камера
	private GameObject playerCameraGameObject;
	private PlayerCameraController playerCameraController;
	private PlayerCameraBlurFilter playerCameraBlurFilter;
	private PlayerCameraFirstPersonRender playerCameraFirstPersonRender;

	//Меню
	private GameObject menuManagerGameobject;
	private MenuManager menuManager;
	//Меню Паузы
	private PauseMenuController pauseMenuController;
	[SerializeField] private GameObject canvasPauseMenu;
	private GameObject buttonClosePauseMenu;
	private GameObject buttonOpenPauseSubMenuSave;
	private GameObject buttonOpenPauseSubMenuLoad;
	private GameObject buttonOpenPauseSubMenuImages;
	private GameObject buttonOpenPauseSubMenuSettings;
	private GameObject buttonExitToMainMenu;
	//ПодМеню Сохранения
	private PauseSubMenuSaveController pauseSubMenuSaveController;
	[SerializeField] private GameObject canvasPauseSubMenuSave;
	//ПодМеню Загрузки
	private PauseSubMenuLoadController pauseSubMenuLoadController;
	[SerializeField] private GameObject canvasPauseSubMenuLoad;
	//ПодМеню Картинок
	private PauseSubMenuImagesController pauseSubMenuImagesController;
	[SerializeField] private GameObject canvasPauseSubMenuImages;
	//ПодМеню Настроек
	private PauseSubMenuSettingsController pauseSubMenuSettingsController;
	[SerializeField] private GameObject canvasPauseSubMenuSettings;

	//Система Оружия
	private GameObject weaponSystemGameObject;
	private WeaponController weaponController;
	//Колесо Выбора Оружия
	private WeaponWheelController weaponWheelController;
	[SerializeField] private GameObject canvasMenuWeaponWheel;
	private GameObject weaponWheelSegmentPrefab;
	private TextMeshProUGUI weaponText;
	private TextMeshProUGUI weaponWheelName;
	private Transform centerPoint; // я думаю это можно удалить ?? нужно проверить

	//Система Взаимодействия
	private GameObject interactionControllerGameObject;
	private InteractionController interactionController;
	[SerializeField] private GameObject CanvasHUDInteraction;
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

	private void Awake()
	{
		StartCoroutine(SequentialInitialization());
	}

	private IEnumerator SequentialInitialization()
	{
		Time.timeScale = 0f;
		yield return StartCoroutine(InitializeInterfaces());           // Инициализация интерфейсов
		yield return StartCoroutine(InitializeSavingSystem());
		yield return StartCoroutine(InitializeMenuLogic());    // Инициализация системы сохранения
		yield return StartCoroutine(InitializePlayerComponents());    // Инициализация игрока и всех его компонент
		yield return StartCoroutine(InitializeWeaponsSystem());       // Инициализация оружия и сопутствующих компонентов
		yield return StartCoroutine(InitializeInteractionSystem());   // Инициализация взаимодействия с миром
		yield return StartCoroutine(RegisterAllDependencies());
		//yield return StartCoroutine(LoadNextScene());   // Регистрация всех сервисов
		Debug.Log("! GAME INITIALIZED !");
		dataPersistenceManager.Initialize();
		Time.timeScale = 1.0f;
	}


	private IEnumerator InitializeInterfaces()
	{
		ServiceLocator.ClearServices();
		gameController = new GameController();
		inputDevice = new InputKeyboard(gameController);
		Debug.Log("Интерфейсы инициализированы.");
		yield break;
	}

	private IEnumerator InitializeSavingSystem()
	{
		dataPersistenceManagerGameObject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapScripts/DataPersistenceManagerGameObject"));
		dataPersistenceManager = dataPersistenceManagerGameObject.GetComponent<DataPersistenceManager>();
		Debug.Log("Система сохранения создана.");
		yield break;
	}

	private IEnumerator InitializePlayerComponents()
	{
		playerGameObject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapPlayer/PlayerGameObject"));
		playerCameraGameObject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapPlayer/PlayerCameraGameObject"));

		// Получение компонентов игрока
		playerBehaviour = playerGameObject.GetComponent<PlayerBehaviour>();
		movementController = playerGameObject.GetComponent<PlayerMovementController>();
		playerCollider = playerGameObject.GetComponentInChildren<PlayerCapsuleCollider>();
		playerAnimationController = playerGameObject.GetComponent<PlayerAnimationController>();

		// Компоненты камеры игрока
		playerCameraController = playerCameraGameObject.GetComponent<PlayerCameraController>();
		playerCameraBlurFilter = playerCameraGameObject.GetComponent<PlayerCameraBlurFilter>();
		playerCameraFirstPersonRender = playerCameraGameObject.GetComponent<PlayerCameraFirstPersonRender>();

		// Внутренние объекты игрока
		playerFirstPersonHandRight = FindDeepChildByName(playerCameraGameObject, "UNITY HandRight");
		playerFirstPersonHandLeft = FindDeepChildByName(playerCameraGameObject, "UNITY  HandLeft");
		playerHeadParent = FindDeepChildByName(playerGameObject, "UNITY PlayerHead");
		playerHandRightParent = FindDeepChildByName(playerGameObject, "UNITY HandRight");
		playerHandLeftParent = FindDeepChildByName(playerGameObject, "UNITY  HandLeft");

		// Инициализация полученных компонентов
		playerBehaviour.Initialize(inputDevice);
		movementController.Initialize(inputDevice, playerBehaviour);
		playerCollider.Initialize(movementController);
		playerCameraController.Initialize(inputDevice, menuManager, movementController, playerCollider, playerGameObject);
		playerCameraBlurFilter.Initialize(menuManager);



		//playerCameraFirstPersonRender.Initialize(playerCameraController, weaponController, playerFirstPersonHandRight, playerFirstPersonHandLeft, playerHeadParent, playerHandRightParent, playerHandLeftParent);
		//playerAnimationController.Initialize(inputDevice, playerGameObject, playerBehaviour, movementController, playerCameraController, weaponController);




		Debug.Log("Компоненты игрока инициализированы.");
		yield break;
	}

	private IEnumerator InitializeMenuLogic()
	{
		menuManagerGameobject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapScripts/MenuManagerGameObject"));
		menuManager = menuManagerGameobject.GetComponent<MenuManager>();

		// Объекты меню
		canvasPauseMenu = Instantiate(canvasPauseMenu);
		canvasPauseSubMenuSave = Instantiate(canvasPauseSubMenuSave);
		canvasPauseSubMenuLoad = Instantiate(canvasPauseSubMenuLoad);
		canvasPauseSubMenuImages = Instantiate(canvasPauseSubMenuImages);
		canvasPauseSubMenuSettings = Instantiate(canvasPauseSubMenuSettings);

		// Контроллеры меню
		pauseMenuController = menuManagerGameobject.GetComponent<PauseMenuController>();
		pauseSubMenuSaveController = menuManagerGameobject.GetComponent<PauseSubMenuSaveController>();
		pauseSubMenuLoadController = menuManagerGameobject.GetComponent<PauseSubMenuLoadController>();
		pauseSubMenuImagesController = menuManagerGameobject.GetComponent<PauseSubMenuImagesController>();
		pauseSubMenuSettingsController = menuManagerGameobject.GetComponent<PauseSubMenuSettingsController>();

		// Кнопки меню
		buttonClosePauseMenu = FindDeepChildByName(canvasPauseMenu, "PauseMenu Resume Button");
		buttonOpenPauseSubMenuSave = FindDeepChildByName(canvasPauseMenu, "PauseMenu Save Button");
		buttonOpenPauseSubMenuLoad = FindDeepChildByName(canvasPauseMenu, "PauseMenu Load Button");
		buttonOpenPauseSubMenuImages = FindDeepChildByName(canvasPauseMenu, "PauseMenu Images Button");
		buttonOpenPauseSubMenuSettings = FindDeepChildByName(canvasPauseMenu, "PauseMenu Settings Button");
		buttonExitToMainMenu = FindDeepChildByName(canvasPauseMenu, "PauseMenu Exit Button");

		// Инициализация меню
		menuManager.Initialize(inputDevice, gameController);
		pauseMenuController.Initialize(inputDevice, menuManager, canvasPauseMenu, buttonClosePauseMenu, buttonOpenPauseSubMenuSave,
			buttonOpenPauseSubMenuLoad, buttonOpenPauseSubMenuImages, buttonOpenPauseSubMenuSettings, buttonExitToMainMenu);
		pauseSubMenuSaveController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuSave);
		pauseSubMenuLoadController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuLoad);
		pauseSubMenuImagesController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuImages);
		pauseSubMenuSettingsController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuSettings);

		Debug.Log("Логика меню инициализирована.");
		yield break;
	}

	private IEnumerator InitializeWeaponsSystem()
	{
		weaponSystemGameObject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapScripts/WeaponSystemGameObject"));

		// Основной компонент оружия
		weaponController = weaponSystemGameObject.GetComponent<WeaponController>();
		weaponWheelController = weaponSystemGameObject.GetComponent<WeaponWheelController>();

		// Колесо выбора оружия
		canvasMenuWeaponWheel = Instantiate(canvasMenuWeaponWheel);
		weaponWheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButton");
		centerPoint = canvasMenuWeaponWheel.transform.Find("Centre")?.transform;
		weaponText = canvasMenuWeaponWheel.transform.Find("Selected Weapon Name")?.GetComponent<TextMeshProUGUI>();
		weaponWheelName = canvasMenuWeaponWheel.transform.Find("WeaponWheel Hand")?.GetComponent<TextMeshProUGUI>();

		// Инициализация оружия
		weaponController.Initialize(inputDevice, menuManager, playerBehaviour);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController, weaponWheelSegmentPrefab, centerPoint, canvasMenuWeaponWheel, weaponText, weaponWheelName);

		Debug.Log("Система оружия инициализирована.");
		yield break;
	}

	private IEnumerator InitializeInteractionSystem()
	{
		interactionControllerGameObject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapScripts/InteractionControllerGameObject"));
		interactionController = interactionControllerGameObject.GetComponent<InteractionController>();

		// Элементы HUD
		CanvasHUDInteraction = Instantiate(CanvasHUDInteraction);
		mainInteractionText = CanvasHUDInteraction.transform.Find("mainInteractionText")?.GetComponent<TextMeshProUGUI>();
		additionalInteractionText = CanvasHUDInteraction.transform.Find("additionalInteractionText")?.GetComponent<TextMeshProUGUI>();
		item1Text = CanvasHUDInteraction.transform.Find("Item1text")?.GetComponent<TextMeshProUGUI>();
		item2Text = CanvasHUDInteraction.transform.Find("Item2text")?.GetComponent<TextMeshProUGUI>();
		item3Text = CanvasHUDInteraction.transform.Find("Item3text")?.GetComponent<TextMeshProUGUI>();
		item1Image = CanvasHUDInteraction.transform.Find("Image1Icon")?.GetComponent<Image>();
		item2Image = CanvasHUDInteraction.transform.Find("Image2Icon")?.GetComponent<Image>();
		item3Image = CanvasHUDInteraction.transform.Find("Image3Icon")?.GetComponent<Image>();
		exitInteraction = CanvasHUDInteraction.transform.Find("ExitInteraction")?.GetComponent<Button>();
		imageNewspaper = CanvasHUDInteraction.transform.Find("ReadableImage")?.GetComponent<Image>();
		readableText = CanvasHUDInteraction.transform.Find("ReadableText")?.GetComponent<TextMeshProUGUI>();
		backgroundBlack = CanvasHUDInteraction.transform.Find("BackgroundBlack")?.GetComponent<Image>();

		// Инициализация взаимодействия
		interactionController.Initialize(inputDevice, playerCameraController, playerBehaviour, mainInteractionText, additionalInteractionText, item1Text, item2Text, item3Text, item1Image, item2Image, item3Image);

		Debug.Log("Система взаимодействия инициализирована.");
		yield break;
	}

	private IEnumerator RegisterAllDependencies()
	{
		// Регистрация служб
		ServiceLocator.Register("Player", playerGameObject);
		ServiceLocator.Register("MenuManager", menuManager);
		ServiceLocator.Register("WeaponController", weaponController);
		ServiceLocator.Register("ExitInteraction", exitInteraction);
		ServiceLocator.Register("ImageNewspaper", imageNewspaper);
		ServiceLocator.Register("ReadableText", readableText);
		ServiceLocator.Register("BackgroundBlack", backgroundBlack);

		Debug.Log("Сервисы зарегистрированы.");
		yield break;
	}






	private IEnumerator LoadNextScene()
	{
		SceneManager.LoadScene("NEW_SceneTest", LoadSceneMode.Additive);

		Debug.Log("Дополнительная сцена загружена!");
		yield break;
	}




	private void OnApplicationQuit()
	{
		ServiceLocator.ClearServices();
	}



	
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

}
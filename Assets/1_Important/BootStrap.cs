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
	private LocalizationManager localizationManager;
	private GameController gameController;

	//Система Сохранений
	private GameObject dataSaveLoadControllerObject;
	private SaveLoadController saveLoadController;

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
	private GameObject[] buttonsPauseMenu;
	//ПодМеню Сохранения
	private PauseSubMenuSaveController pauseSubMenuSaveController;
	[SerializeField] private GameObject canvasPauseSubMenuSave;
	private GameObject[] buttonsSaveGame;
	//ПодМеню Загрузки
	private PauseSubMenuLoadController pauseSubMenuLoadController;
	[SerializeField] private GameObject canvasPauseSubMenuLoad;
	private GameObject[] buttonsLoadGame;
	private GameObject[] buttonsDeleteGame;
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
	private TextMeshProUGUI[] itemsTexts;
	private Image[] itemsImages;
	private Image imageNewspaper;

	private void Awake()
	{
		StartCoroutine(SequentialInitialization());
	}

	private IEnumerator SequentialInitialization()
	{
		Time.timeScale = 0f;
		yield return StartCoroutine(InitializeInterfaces());          
		yield return StartCoroutine(InitializeSavingSystem());
		
		yield return StartCoroutine(InitializeMenuLogic());
		yield return StartCoroutine(InitializePlayerComponents());
		yield return StartCoroutine(InitializeWeaponsSystem());       
		yield return StartCoroutine(InitializeInteractionSystem());   
		yield return StartCoroutine(RegisterAllDependencies());
		yield return StartCoroutine(LoadNextScene());   
		Debug.Log("!!! GAME INITIALIZED !!!");
		saveLoadController.NewGame();
		//saveLoadController.Initialize();
		Time.timeScale = 1.0f;
	}


	private IEnumerator InitializeInterfaces()
	{
		ServiceLocator.ClearServices();
		gameController = new GameController();
		localizationManager = new LocalizationManager();
		localizationManager.ChangeLanguage("Russian"); 
		inputDevice = new InputKeyboard(gameController);
		Debug.Log("INTERFACES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeSavingSystem()
	{
		dataSaveLoadControllerObject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapScripts/DataPersistenceManagerGameObject"));
		saveLoadController = dataSaveLoadControllerObject.GetComponent<SaveLoadController>();
		saveLoadController.Initialize();
		Debug.Log("SAVE SYSTEM INITIALIZED");
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




		Debug.Log("PLAYER INITIALIZED");
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
		buttonsPauseMenu = new GameObject[]
		{
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Resume Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Save Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Load Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Images Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Settings Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Exit Button")
		};

		buttonsSaveGame = new GameObject[]
		{
			FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE1 Button"),
			FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE2 Button"),
			FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE3 Button"),
			FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE4 Button"),
			FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE5 Button"),
		};

		buttonsLoadGame = new GameObject[]
		{
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD1 Button"),
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD2 Button"),
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD3 Button"),
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD4 Button"),
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD5 Button"),
		};

		buttonsDeleteGame = new GameObject[]
		{
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu DELETE1 Button"),
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu DELETE2 Button"),
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu DELETE3 Button"),
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu DELETE4 Button"),
			FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu DELETE5 Button"),
		};

		// Инициализация меню
		menuManager.Initialize(inputDevice, gameController);
		pauseMenuController.Initialize(inputDevice, menuManager, canvasPauseMenu, buttonsPauseMenu);
		pauseSubMenuSaveController.Initialize(inputDevice, menuManager, pauseMenuController, saveLoadController, canvasPauseSubMenuSave, buttonsSaveGame);
		pauseSubMenuLoadController.Initialize(inputDevice, menuManager, pauseMenuController, saveLoadController, canvasPauseSubMenuLoad, buttonsLoadGame, buttonsDeleteGame);
		pauseSubMenuImagesController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuImages);
		pauseSubMenuSettingsController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuSettings);

		Debug.Log("PAUSE MENU INITIALIZED");
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
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController, weaponWheelSegmentPrefab,
			centerPoint, canvasMenuWeaponWheel, weaponText, weaponWheelName);

		Debug.Log("WEAPON SYSTEM INITIALIZED");
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
		itemsTexts = new TextMeshProUGUI[]
		{
			CanvasHUDInteraction.transform.Find("Item1text").GetComponent<TextMeshProUGUI>(),
			CanvasHUDInteraction.transform.Find("Item2text").GetComponent<TextMeshProUGUI>(),
			CanvasHUDInteraction.transform.Find("Item3text").GetComponent<TextMeshProUGUI>()
		};
		itemsImages = new Image[]
		{
			CanvasHUDInteraction.transform.Find("Image1Icon").GetComponent<Image>(),
			CanvasHUDInteraction.transform.Find("Image2Icon").GetComponent<Image>(),
			CanvasHUDInteraction.transform.Find("Image3Icon").GetComponent<Image>()
		};
		exitInteraction = CanvasHUDInteraction.transform.Find("ExitInteraction")?.GetComponent<Button>();
		imageNewspaper = CanvasHUDInteraction.transform.Find("ReadableImage")?.GetComponent<Image>();
		readableText = CanvasHUDInteraction.transform.Find("ReadableText")?.GetComponent<TextMeshProUGUI>();
		backgroundBlack = CanvasHUDInteraction.transform.Find("BackgroundBlack")?.GetComponent<Image>();

		// Инициализация взаимодействия
		interactionController.Initialize(inputDevice, playerCameraController, playerBehaviour, mainInteractionText,
			additionalInteractionText, itemsTexts, itemsImages);
		Debug.Log("INTERACTION SYSTEM INITIALIZED");
		yield break;
	}

	private IEnumerator RegisterAllDependencies()
	{
		// Регистрация служб
		ServiceLocator.Register("LocalizationManager", localizationManager);
		ServiceLocator.Register("Player", playerGameObject);
		ServiceLocator.Register("MenuManager", menuManager);
		ServiceLocator.Register("WeaponController", weaponController);
		ServiceLocator.Register("ExitInteraction", exitInteraction);
		ServiceLocator.Register("ImageNewspaper", imageNewspaper);
		ServiceLocator.Register("ReadableText", readableText);
		ServiceLocator.Register("BackgroundBlack", backgroundBlack);

		Debug.Log("SERVICE REGISTERED");
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
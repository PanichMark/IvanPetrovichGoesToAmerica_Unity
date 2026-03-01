using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
	// CONFIG
	[Header("--- CONFIGS ---")] [SerializeField] private BootStrapConfigScene configScene;

	// Экран Инициализации Bootstrap
	private GameObject tempCameraObject;
	[Header("Bootstrap")] [SerializeField] private GameObject canvasBootstrap;
	private TMP_Text loadingStatusText;

	// Интерфейсы
	private IInputDevice inputDevice;
	private LocalizationManager localizationManager;
	private GameController gameController;

	// Система Сцен
	private GameObject gameSceneManagerGameObject;
	[Header("Loading Screen")] [SerializeField] private GameObject canvasLoadingScreen;
	private GameSceneManager gameSceneManager;
	private TMP_Text loadingScreenText;

	// Система сохранений
	private GameObject dataSaveLoadControllerGameObject;
	private SaveLoadController saveLoadController;

	// Игрок
	private GameObject playerGameObject;
	private GameObject playerHeadParent;
	private GameObject playerHandRightParent;
	private GameObject playerHandLeftParent;
	private GameObject playerFirstPersonHandRight;
	private GameObject playerFirstPersonHandLeft;
	// Игрок системы
	private PlayerBehaviour playerBehaviour;
	private PlayerMovementController playerMovementController;
	private PlayerCapsuleCollider playerCollider;
	private PlayerAnimationController playerAnimationController;
	// Игрок камера
	private GameObject playerMainCameraGameObject;
	private PlayerCameraController playerCameraController;
	private PlayerCameraBlurFilter playerCameraBlurFilter;
	private PlayerCameraFirstPersonRender playerCameraFirstPersonRender;

	// Игрок ресурсы
	private GameObject playerResourcesGameObject;
	private CanvasHUDPlayerResourcesController canvasHUDPlayerResourcesController;
	[Header("Player Resources")] [SerializeField] private GameObject canvasHUDPlayerResources;
	// Игрок ресурсы деньги
	private PlayerResourcesMoneyManager playerResourcesMoneyManager;
	private TMP_Text playerMoneyTextGameObject;
	// Игрок ресурсы здоровье
	private PlayerResourcesHealthManager playerResourcesHealthManager;
	private Slider HealthBarSlider;
	private Button HealingItemButton;
	private TextMeshProUGUI HealingItemNumber;
	// Игрок ресурсы мана
	private PlayerResourcesManaManager playerResourcesManaManager;
	private Slider ManaBarSlider;
	private Button ManaReplenishtemButton;
	private TextMeshProUGUI ManaReplenishItemNumber;
	// Игрок ресурсы патроны
	//
	//

	// Меню
	private GameObject menuManagerGameobject;
	private MenuManager menuManager;
	// Главное меню
	private MainMenuReadNews mainMenuReadNews;
	[Header("MainMenu")] [SerializeField] private GameObject canvasMainMenuReadNews;
	private Button buttonCloseMainMenuReadNews;
	// Меню паузы
	private PauseMenuController pauseMenuController;
	[Header("PauseMenu")] [SerializeField] private GameObject canvasPauseMenu;
	private GameObject[] buttonsPauseMenu;
	// Подменю сохранения
	private PauseSubMenuSaveController pauseSubMenuSaveController;
	[SerializeField] private GameObject canvasPauseSubMenuSave;
	private GameObject[] buttonsSaveGame;
	private GameObject buttonClosePauseSubMenuSave;
	// Подменю загрузки
	private PauseSubMenuLoadController pauseSubMenuLoadController;
	[SerializeField] private GameObject canvasPauseSubMenuLoad;
	private GameObject[] buttonsLoadGame;
	private GameObject[] buttonsDeleteGame;
	private GameObject buttonClosePauseSubMenuLoad;
	// Подменю картинок
	private PauseSubMenuImagesController pauseSubMenuImagesController;
	[SerializeField] private GameObject canvasPauseSubMenuImages;
	private GameObject buttonClosePauseSubMenuImages;
	// Подменю настроек
	private PauseSubMenuSettingsController pauseSubMenuSettingsController;
	[SerializeField] private GameObject canvasPauseSubMenuSettings;
	private GameObject buttonClosePauseSubMenuSettings;
	private GameObject[] FPSbuttons;
	private GameObject FOVSlider;
	private GameObject fovDisplayText;
	private GameObject[] buttonsChangeLanguage;
	private GameObject[] KeyRebinds;

	// Система оружия
	private GameObject weaponSystemGameObject;
	private WeaponController weaponController;
	private LegKickAttack legKickAttack;
	// Колесо выбора оружия
	private WeaponWheelMenuController weaponWheelController;
	[Header("Weapon Wheel Menu")] [SerializeField] private GameObject canvasMenuWeaponWheel;
	private GameObject weaponWheelSegmentPrefab;
	private TextMeshProUGUI weaponText;
	private TextMeshProUGUI weaponWheelName;
	private Transform centerPoint; // я думаю это можно удалить ??

	// Система взаимодействия
	private GameObject interactionControllerGameObject;
	private InteractionController interactionController;
	[Header("Interaction")] [SerializeField] private GameObject canvasHUDInteraction;
	[SerializeField] private GameObject canvasReadNoteMenu;
	[SerializeField] private GameObject canvasLockpickMenu;
	private TextMeshProUGUI mainInteractionText;
	private TextMeshProUGUI additionalInteractionText;
	private Button buttonExitReadNoteMenu;
	private Button buttonExitLockpickMenu;
	private TextMeshProUGUI readableText;
	private Image backgroundBlack;
	private TextMeshProUGUI[] itemsTexts;
	private Image[] itemsImages;
	private Image imageNewspaper;

	private void Awake()
	{
		ServiceLocator.ClearAllServices();
		canvasBootstrap = Instantiate(canvasBootstrap);
		loadingStatusText = canvasBootstrap.transform.Find("TextInitializationStep")?.GetComponent<TMP_Text>();
	
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		tempCameraObject = new GameObject("TempCamera");
		tempCameraObject.AddComponent<Camera>();

		StartCoroutine(SequentialInitialization());
	}

	private IEnumerator SequentialInitialization()
	{
		yield return StartCoroutine(InitializeInterfaces());
		yield return StartCoroutine(InitializePlayerPrefabs());
		yield return StartCoroutine(InitializeCanvases());
		yield return StartCoroutine(InitializeSceneSystem());
		yield return StartCoroutine(InitializeSavingSystem());
		yield return StartCoroutine(InitializeMenuSystems());
		yield return StartCoroutine(InitializePlayerSystems());
		yield return StartCoroutine(InitializePlayerResources());
		yield return StartCoroutine(InitializeWeaponSystem());
		yield return StartCoroutine(InitializeInteractionSystem());
		yield return StartCoroutine(InitializeFinalSystems());
		yield return StartCoroutine(RegisterAllDependencies());

		yield return new WaitForSecondsRealtime(0.3f);
		//yield return new WaitForSecondsRealtime(1f);

		ChangeLanguage(LanguagesEnum.Russian);

		Debug.Log("!!! GAME INITIALIZED !!!");

		yield return StartCoroutine(saveLoadController.NewGame());

		Destroy(tempCameraObject);
		Destroy(canvasBootstrap);

		//Debug.Log(bootStrapConfig.selectedScene);

		if (configScene.selectedScene.ToString() == "Scene_0_MainMenu")
		{
			yield return StartCoroutine(gameSceneManager.LoadMainMenuScene());
		}
		else yield return StartCoroutine(gameSceneManager.LoadScene(configScene.selectedScene));

		
	}

	public void ChangeLanguage(LanguagesEnum language)
	{
		localizationManager.ChangeLanguage(language);

		interactionController.ChangeLanguage(localizationManager);
		gameSceneManager.ChangeLanguage(localizationManager);

		ServiceLocator.RemoveService("LocalizationManager");
		ServiceLocator.Register("LocalizationManager", localizationManager);

	}

	private IEnumerator InitializeInterfaces()
	{
		loadingStatusText.text = "Interfaces";

		
		gameController = new GameController();
		localizationManager = new LocalizationManager();
		
		//localizationManager.ChangeLanguage(LanguagesEnum.Russian);
		inputDevice = new InputKeyboard(gameController);
		Debug.Log("INTERFACES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializePlayerPrefabs()
	{
		playerGameObject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapPlayer/PlayerGameObject"));
		playerMainCameraGameObject = Instantiate((GameObject)Resources.Load("Bootstrap/BootstrapPlayer/PlayerCameraGameObject"));
		

		Debug.Log("PLAYER PREFABS INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeCanvases()
	{
		loadingStatusText.text = "Canvases";

		canvasMainMenuReadNews = Instantiate(canvasMainMenuReadNews);
		canvasPauseMenu = Instantiate(canvasPauseMenu);
		canvasPauseSubMenuSave = Instantiate(canvasPauseSubMenuSave);
		canvasPauseSubMenuLoad = Instantiate(canvasPauseSubMenuLoad);
		canvasPauseSubMenuImages = Instantiate(canvasPauseSubMenuImages);
		canvasPauseSubMenuSettings = Instantiate(canvasPauseSubMenuSettings);
		canvasMenuWeaponWheel = Instantiate(canvasMenuWeaponWheel);
		canvasHUDInteraction = Instantiate(canvasHUDInteraction);
		canvasHUDPlayerResources = Instantiate(canvasHUDPlayerResources);
		canvasLoadingScreen = Instantiate(canvasLoadingScreen);
		canvasReadNoteMenu = Instantiate(canvasReadNoteMenu);
		canvasLockpickMenu = Instantiate(canvasLockpickMenu);
		yield break;
	}

	private IEnumerator InitializeSceneSystem()
	{
		gameSceneManagerGameObject = new GameObject("GameSceneManager");
		gameSceneManager = gameSceneManagerGameObject.AddComponent<GameSceneManager>();
		loadingScreenText = canvasLoadingScreen.transform.Find("LoadingScreenText")?.GetComponent<TMP_Text>();
		gameSceneManager.Initialize(gameController, canvasLoadingScreen, loadingScreenText);

		yield break;
	}

	private IEnumerator InitializeSavingSystem()
	{
		loadingStatusText.text = "Saving System";

		dataSaveLoadControllerGameObject = new GameObject("DataSaveLoadController");
		saveLoadController = dataSaveLoadControllerGameObject.AddComponent<SaveLoadController>();
		saveLoadController.Initialize(gameSceneManager, gameController);
		Debug.Log("SAVE SYSTEM INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeMenuSystems()
	{
		loadingStatusText.text = "Menu Systems";

		menuManagerGameobject = new GameObject("MenuManager");

		// Контроллеры меню
		menuManager = menuManagerGameobject.AddComponent<MenuManager>();
		mainMenuReadNews = menuManagerGameobject.AddComponent<MainMenuReadNews>();
		pauseMenuController = menuManagerGameobject.AddComponent<PauseMenuController>();
		pauseSubMenuSaveController = menuManagerGameobject.AddComponent<PauseSubMenuSaveController>();
		pauseSubMenuLoadController = menuManagerGameobject.AddComponent<PauseSubMenuLoadController>();
		pauseSubMenuImagesController = menuManagerGameobject.AddComponent<PauseSubMenuImagesController>();
		pauseSubMenuSettingsController = menuManagerGameobject.AddComponent<PauseSubMenuSettingsController>();

		buttonCloseMainMenuReadNews = canvasMainMenuReadNews.transform.Find("ExitReadNews").GetComponent<Button>();
		mainMenuReadNews.Initialize(inputDevice, canvasMainMenuReadNews, buttonCloseMainMenuReadNews);

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
		buttonClosePauseSubMenuSave = FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu close Button");

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
		buttonClosePauseSubMenuLoad = FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu close Button");

		buttonClosePauseSubMenuImages = FindDeepChildByName(canvasPauseSubMenuImages, "ImagesSubMenu close Button");

		FOVSlider = FindDeepChildByName(canvasPauseSubMenuSettings, "CameraFOVSlider");
		fovDisplayText = FindDeepChildByName(canvasPauseSubMenuSettings, "CameraFOVText");
		FPSbuttons = new GameObject[]
		{
			FindDeepChildByName(canvasPauseSubMenuSettings, "Fps30"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "Fps60"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "Fps90"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "Fps144"),
		};

		buttonsChangeLanguage = new GameObject[]
		{
			FindDeepChildByName(canvasPauseSubMenuSettings, "LanguageRussian"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "LanguageEnglish"),
		};

		KeyRebinds = new GameObject[]
		{
			FindDeepChildByName(canvasPauseSubMenuSettings, "MoveForward"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "MoveBackward"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "MoveRight"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "MoveLeft"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "Run"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "Jump"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "Crouch"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "Interact"),

			FindDeepChildByName(canvasPauseSubMenuSettings, "ChangeCameraView"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "ChangeCameraShoulder"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "RightHandWeaponWheel"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "LeftHandWeaponWheel"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "RightHandWeaponAttack"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "LeftHandWeaponAttack"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "Reload"),
			FindDeepChildByName(canvasPauseSubMenuSettings, "LegKick"),
		};

		buttonClosePauseSubMenuSettings = FindDeepChildByName(canvasPauseSubMenuSettings, "SettingsSubMenu close Button");

		// Инициализация меню
		menuManager.Initialize(inputDevice, gameSceneManager, gameController, saveLoadController);
		pauseMenuController.Initialize(inputDevice, gameController, gameSceneManager, saveLoadController, menuManager, canvasPauseMenu, buttonsPauseMenu);
		pauseSubMenuSaveController.Initialize(inputDevice, menuManager, pauseMenuController, saveLoadController, canvasPauseSubMenuSave, buttonsSaveGame, buttonClosePauseSubMenuSave);
		pauseSubMenuLoadController.Initialize(inputDevice, menuManager, pauseMenuController, saveLoadController, canvasPauseSubMenuLoad, buttonsLoadGame, buttonsDeleteGame, buttonClosePauseSubMenuLoad);
		pauseSubMenuImagesController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuImages, buttonClosePauseSubMenuImages);
		pauseSubMenuSettingsController.Initialize(inputDevice, this, gameController, playerMainCameraGameObject, fovDisplayText, menuManager, pauseMenuController, canvasPauseSubMenuSettings, buttonClosePauseSubMenuSettings, FOVSlider, FPSbuttons, buttonsChangeLanguage, KeyRebinds);

		Debug.Log("PAUSE MENU INITIALIZED");
		yield break;
	}

	private IEnumerator InitializePlayerSystems()
	{
		loadingStatusText.text = "Player Systems";

		// Получение компонентов игрока
		playerBehaviour = playerGameObject.GetComponent<PlayerBehaviour>();
		playerMovementController = playerGameObject.GetComponent<PlayerMovementController>();
		playerCollider = playerGameObject.GetComponentInChildren<PlayerCapsuleCollider>();
		playerAnimationController = playerGameObject.GetComponent<PlayerAnimationController>();

		// Компоненты камеры игрока
		playerCameraController = playerMainCameraGameObject.GetComponent<PlayerCameraController>();
		playerCameraBlurFilter = playerMainCameraGameObject.GetComponent<PlayerCameraBlurFilter>();
		playerCameraFirstPersonRender = playerMainCameraGameObject.GetComponent<PlayerCameraFirstPersonRender>();

		// Внутренние объекты игрока
		playerFirstPersonHandRight = FindDeepChildByName(playerMainCameraGameObject, "UNITY HandRight");
		playerFirstPersonHandLeft = FindDeepChildByName(playerMainCameraGameObject, "UNITY  HandLeft");
		playerHeadParent = FindDeepChildByName(playerGameObject, "UNITY PlayerHead");
		playerHandRightParent = FindDeepChildByName(playerGameObject, "UNITY HandRight");
		playerHandLeftParent = FindDeepChildByName(playerGameObject, "UNITY  HandLeft");

		// Инициализация полученных компонентов
		playerBehaviour.Initialize(inputDevice);
		playerMovementController.Initialize(inputDevice, gameSceneManager, playerBehaviour);
		playerCollider.Initialize(playerMovementController);
		playerCameraController.Initialize(inputDevice, gameSceneManager, menuManager, playerMovementController, playerCollider, playerGameObject);
		playerCameraBlurFilter.Initialize(menuManager);

		Debug.Log("PLAYER SYSTEMS INITIALIZED");
		yield break;
	}

	private IEnumerator InitializePlayerResources()
	{
		loadingStatusText.text = "Player Resources";

		playerResourcesGameObject = new GameObject("PlayerResources");

		playerMoneyTextGameObject = canvasPauseMenu.transform.Find("PauseMenu PlayerMoneyNumber")?.GetComponent<TMP_Text>();
		HealthBarSlider = canvasHUDPlayerResources.transform.Find("Health Slider")?.GetComponent<Slider>();
		HealingItemButton = FindDeepChildByName(canvasMenuWeaponWheel, "HealingItemButton")?.GetComponent<Button>();
		HealingItemNumber = FindDeepChildByName(canvasMenuWeaponWheel, "HealingItemsNumber")?.GetComponent<TextMeshProUGUI>();
		ManaBarSlider = canvasHUDPlayerResources.transform.Find("Mana Slider")?.GetComponent<Slider>();
		ManaReplenishtemButton = FindDeepChildByName(canvasMenuWeaponWheel, "ManaReplenishItemButton ")?.GetComponent<Button>();
		ManaReplenishItemNumber = FindDeepChildByName(canvasMenuWeaponWheel, "ManaReplenishItemsNumber")?.GetComponent<TextMeshProUGUI>();

		canvasHUDPlayerResourcesController = playerResourcesGameObject.AddComponent<CanvasHUDPlayerResourcesController>();
		playerResourcesMoneyManager = playerResourcesGameObject.AddComponent<PlayerResourcesMoneyManager>();
		playerResourcesHealthManager = playerResourcesGameObject.AddComponent<PlayerResourcesHealthManager>();
		playerResourcesManaManager = playerResourcesGameObject.AddComponent<PlayerResourcesManaManager>();

		canvasHUDPlayerResourcesController.Initialize(gameSceneManager, gameController, menuManager, canvasHUDPlayerResources);
		playerResourcesMoneyManager.Initialize(playerMoneyTextGameObject);
		playerResourcesHealthManager.Initialize(HealthBarSlider, HealingItemButton, HealingItemNumber);
		playerResourcesManaManager.Initialize(ManaBarSlider, ManaReplenishtemButton, ManaReplenishItemNumber);

		Debug.Log("PLAYER RESOURCES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeWeaponSystem()
	{
		loadingStatusText.text = "Weapon System";

		weaponSystemGameObject = new GameObject("WeaponSystem");

		// Основной компонент оружия
		weaponController = weaponSystemGameObject.AddComponent<WeaponController>();
		legKickAttack = weaponSystemGameObject.AddComponent<LegKickAttack>();
		weaponWheelController = weaponSystemGameObject.AddComponent<WeaponWheelMenuController>();

		// Колесо выбора оружия
		weaponWheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButton");
		centerPoint = canvasMenuWeaponWheel.transform.Find("Centre")?.transform;
		weaponText = canvasMenuWeaponWheel.transform.Find("Selected Weapon Name")?.GetComponent<TextMeshProUGUI>();
		weaponWheelName = canvasMenuWeaponWheel.transform.Find("WeaponWheel Hand")?.GetComponent<TextMeshProUGUI>();

		// Инициализация оружия
		weaponController.Initialize(inputDevice, menuManager, playerBehaviour);
		legKickAttack.Initialize(inputDevice, playerGameObject, playerMovementController);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController, weaponWheelSegmentPrefab,
			centerPoint, canvasMenuWeaponWheel, weaponText, weaponWheelName);

		Debug.Log("WEAPON SYSTEM INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeInteractionSystem()
	{
		loadingStatusText.text = "Interaction System";

		interactionControllerGameObject = new GameObject("InteractionController");

		interactionController = interactionControllerGameObject.AddComponent<InteractionController>();

		// Элементы HUD
		mainInteractionText = canvasHUDInteraction.transform.Find("mainInteractionText")?.GetComponent<TextMeshProUGUI>();
		additionalInteractionText = canvasHUDInteraction.transform.Find("additionalInteractionText")?.GetComponent<TextMeshProUGUI>();
		itemsTexts = new TextMeshProUGUI[]
		{
			canvasHUDInteraction.transform.Find("Item1text").GetComponent<TextMeshProUGUI>(),
			canvasHUDInteraction.transform.Find("Item2text").GetComponent<TextMeshProUGUI>(),
			canvasHUDInteraction.transform.Find("Item3text").GetComponent<TextMeshProUGUI>()
		};
		itemsImages = new Image[]
		{
			canvasHUDInteraction.transform.Find("Image1Icon").GetComponent<Image>(),
			canvasHUDInteraction.transform.Find("Image2Icon").GetComponent<Image>(),
			canvasHUDInteraction.transform.Find("Image3Icon").GetComponent<Image>()
		};
		buttonExitReadNoteMenu = canvasReadNoteMenu.transform.Find("ExitReadNote")?.GetComponent<Button>();
		imageNewspaper = canvasReadNoteMenu.transform.Find("ReadableImage")?.GetComponent<Image>();
		readableText = canvasReadNoteMenu.transform.Find("ReadableText")?.GetComponent<TextMeshProUGUI>();
		backgroundBlack = canvasReadNoteMenu.transform.Find("BackgroundBlack")?.GetComponent<Image>();
		buttonExitLockpickMenu = canvasLockpickMenu.transform.Find("ExitLockpick")?.GetComponent<Button>();

		// Инициализация взаимодействия
		interactionController.Initialize(gameController, gameSceneManager, inputDevice, menuManager, playerCameraController, playerBehaviour, canvasHUDInteraction, mainInteractionText,
			additionalInteractionText, itemsTexts, itemsImages);
		Debug.Log("INTERACTION SYSTEM INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeFinalSystems()
	{
		playerCameraFirstPersonRender.Initialize(gameSceneManager, playerCameraController, weaponController, playerFirstPersonHandRight, playerFirstPersonHandLeft, playerHeadParent, playerHandRightParent, playerHandLeftParent);
		playerAnimationController.Initialize(inputDevice, playerGameObject, playerBehaviour, playerMovementController, playerCameraController, weaponController, legKickAttack);

		Debug.Log("FINAL SYSTEMS INITIALIZED");
		yield break;
	}

	private IEnumerator RegisterAllDependencies()
	{
		loadingStatusText.text = "Service Locator";

		// Регистрация служб
		ServiceLocator.Register("LocalizationManager", localizationManager);
		ServiceLocator.Register("Player", playerGameObject);
		ServiceLocator.Register("MenuManager", menuManager);
		ServiceLocator.Register("WeaponController", weaponController);
		ServiceLocator.Register("ExitReadNote", buttonExitReadNoteMenu);
		ServiceLocator.Register("ExitLockpick", buttonExitLockpickMenu);
		ServiceLocator.Register("ImageNewspaper", imageNewspaper);
		ServiceLocator.Register("ReadableText", readableText);
		ServiceLocator.Register("BackgroundBlack", backgroundBlack);
		ServiceLocator.Register("PlayerResourcesMoneyManager", playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesHealthManager", playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", playerResourcesManaManager);
		ServiceLocator.Register("CanvasLockpickMenu", canvasLockpickMenu);
		ServiceLocator.Register("CanvasReadNoteMenu", canvasReadNoteMenu);
		ServiceLocator.Register("SaveLoadController", saveLoadController);
		ServiceLocator.Register("PauseMenuController", pauseMenuController);
		ServiceLocator.Register("GameController", gameController);
		ServiceLocator.Register("GameSceneManager", gameSceneManager);
		ServiceLocator.Register("PlayerMovementController", playerMovementController);
		ServiceLocator.Register("MainMenuReadNews", mainMenuReadNews);

		Debug.Log("SERVICE REGISTERED");
		yield break;
	}

	private void OnApplicationQuit()
	{
		ServiceLocator.ClearAllServices();
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
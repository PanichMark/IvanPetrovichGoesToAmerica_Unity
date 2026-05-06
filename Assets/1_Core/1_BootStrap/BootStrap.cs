using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
	// CONFIG
	[Header("--- CONFIGS ---")] [SerializeField] private ConfigBootstrapScene configScene;
	[SerializeField] private ConfigBootstrapPlayerPosition configPlayerPosition;
	[SerializeField] private ConfigBootstrapWeapons configWeapons;

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
	private GameObject playerColliderGameObject;
	private GameObject playerHandRightParent;
	private GameObject playerHandLeftParent;
	private GameObject playerFirstPersonHandRight;
	private GameObject playerFirstPersonHandLeft;
	// Игрок системы
	private PlayerBehaviour playerBehaviour;
	private PlayerMovementController playerMovementController;
	private PlayerCapsuleCollider playerColliderController;
	private PlayerAnimationController playerAnimationController;
	// Игрок камера
	private GameObject playerMainCameraGameObject;
	private PlayerCameraController playerCameraController;
	private PlayerCameraBlurFilter playerCameraBlurFilter;
	private PlayerCameraFirstPersonRender playerCameraFirstPersonRender;

	// Игрок ресурсы
	private GameObject playerResourcesGameObject;
	private CanvasHUDhealthAndManaController canvasHUDhealthAndManaController;
	private CanvasHUDammoController canvasHUDammoController;
	[Header("Player Resources")] [SerializeField] private GameObject canvasHUDhealthAndMana;
	[SerializeField] private GameObject canvasHUDammo;
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
	private PlayerResourcesAmmoManager playerResourcesAmmoManager;
	private GameObject RightWeaponAmmoMagazine;
	private GameObject RightWeaponAmmoReserve;
	private GameObject RightWeaponAmmoSeparator;
	private GameObject LeftWeaponAmmoMagazine;
	private GameObject LeftWeaponAmmoReserve;
	private GameObject LeftWeaponAmmoSeparator;

	// Меню
	private GameObject menuManagerGameobject;
	private MenuManager menuManager;
	// Главное меню
	private MainMenuReadNews mainMenuReadNews;
	[Header("MainMenu")] [SerializeField] private GameObject canvasMainMenuReadNews;
	private Button buttonCloseMainMenuReadNews;
	//Меню подтверждения
	private MenuConfirmActionController menuConfirmActionController;
	[SerializeField] private GameObject canvasMenuConfirmAction;
	private GameObject buttonConfirmAction;
	private GameObject buttonCancelAction;
	private GameObject textShowConfirmationMessage;
	// Меню паузы
	private PauseMenuController pauseMenuController;
	[Header("PauseMenu")] [SerializeField] private GameObject canvasPauseMenu;
	private GameObject[] buttonsPauseMenu;
	// Подменю сохранения
	private PauseSubMenuSaveController pauseSubMenuSaveController;
	[SerializeField] private GameObject canvasPauseSubMenuSave;
	private GameObject[] buttonsRewriteGame;
	private GameObject[] buttonsDeleteGame;
	private GameObject buttonSaveNewGame;
	private GameObject buttonClosePauseSubMenuSave;
	// Подменю загрузки
	private PauseSubMenuLoadController pauseSubMenuLoadController;
	[SerializeField] private GameObject canvasPauseSubMenuLoad;
	private GameObject[] buttonsLoadGame;
	private GameObject buttonClosePauseSubMenuLoad;
	// Подменю внешности
	private PauseSubMenuAppearanceController pauseSubMenuAppearanceController;
	[SerializeField] private GameObject canvasPauseSubMenuAppearance;
	private GameObject buttonClosePauseSubMenuAppearance;
	// Подменю настроек
	private PauseSubMenuSettingsController pauseSubMenuSettingsController;
	[SerializeField] private GameObject canvasPauseSubMenuSettings;
	private PauseSubMenuSettingsPlayerPrefs pauseSubMenuSettingsPlayerPrefs;
	private GameObject buttonClosePauseSubMenuSettings;
	private GameObject[] FPSbuttons;
	private GameObject FOVSlider;
	private GameObject fovDisplayText;
	private GameObject[] buttonsChangeLanguage;
	private GameObject[] KeyRebinds;
	private GameObject buttonSaveSettings;
	private GameObject buttonResetSettings;

	// Система оружия
	private GameObject weaponSystemGameObject;
	private PlayerWeaponController weaponController;
	private LegKickAttack legKickAttack;
	private WeaponAnimationController weaponAnimationController;
	private WeaponFirstPersonRender weaponFirstPersonRender;
	private GameObject firstPersonLeftHandWeaponSlotGameObject;
	private GameObject firstPersonRightHandWeaponSlotGameObject;
	private GameObject thirdPersonLeftHandWeaponSlotGameObject;
	private GameObject thirdPersonRightHandWeaponSlotGameObject;
	// Колесо выбора оружия
	private WeaponWheelMenuController weaponWheelController;
	[Header("Weapon Wheel Menu")] [SerializeField] private GameObject canvasMenuWeaponWheel;
	private GameObject weaponWheelSegmentPrefab;
	private TextMeshProUGUI weaponText;
	private TextMeshProUGUI weaponWheelName;
	private Image weaponIconBig;
	private Transform centerPoint; // я думаю это можно удалить 
	private GameObject ChokeNPCtext;

	// Система взаимодействия
	private GameObject interactionControllerGameObject;
	private InteractionController interactionController;
	private InteractionAnimationController interactionAnimationController;
	private InteractionFirstPersonRender interactionFirstPersonRender;
	[Header("Interaction")] [SerializeField] private GameObject canvasHUDInteraction;
	[SerializeField] private GameObject canvasReadNoteMenu;
	[SerializeField] private GameObject canvasLockpickMechanicalMenu;
	[SerializeField] private GameObject canvasLockpickElectronicMenu;
	[SerializeField] private GameObject canvasDialogueMenu;
	private GameObject[] buttonsLockElectrical;
	private TextMeshProUGUI mainInteractionText;
	private TextMeshProUGUI additionalInteractionText;
	private Button buttonExitReadNoteMenu;
	private Button buttonExitLockpickMechanicalMenu;
	private Button buttonExitLockpickElectronicMenu;
	private TextMeshProUGUI readableText;
	private Image backgroundBlack;
	private TextMeshProUGUI[] itemsTexts;
	private Image[] itemsImages;
	private Image imageNewspaper;
	private TextMeshProUGUI NPCphrasesText;
	private TextMeshProUGUI NPCdialogueText;
	private Button buttonDialogueYes;
	private Button buttonDialogueNo;


	private void Awake()
	{
		ServiceLocator.ClearAllServices();
		canvasBootstrap = Instantiate(canvasBootstrap);
		loadingStatusText = canvasBootstrap.transform.Find("TextInitializationStep").GetComponent<TMP_Text>();
	
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
		yield return StartCoroutine(InitializeInteractionSystem());
		yield return StartCoroutine(InitializeWeaponSystem());
		yield return StartCoroutine(RegisterAllDependencies());

		//yield return new WaitForSecondsRealtime(0.3f);
		//yield return new WaitForSecondsRealtime(1f);

		ChangeLanguage(LanguagesEnum.Russian);

		Debug.Log("!!! GAME INITIALIZED !!!");

		yield return StartCoroutine(saveLoadController.NewGame());

		// Разблокировка оружий из конфига
		GameObject[] availableWeapons = configWeapons.GetAvailableWeapons();
		foreach (GameObject weaponPrefab in availableWeapons)
		{
			weaponController.UnlockWeapon(weaponPrefab);
		}

		Destroy(tempCameraObject);
		Destroy(canvasBootstrap);

		//Debug.Log(bootStrapConfig.selectedScene);

		if (configScene.selectedScene.ToString() == "Scene_0_MainMenu")
		{
			yield return StartCoroutine(gameSceneManager.LoadMainMenuScene());
		}
		else
		{
			yield return StartCoroutine(gameSceneManager.LoadScene(configScene.selectedScene));
		}

		playerMovementController.SetPlayerPosition(configPlayerPosition.playerPosition);
		
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
		playerGameObject = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/PlayerGameObject"));
		playerMainCameraGameObject = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/PlayerCameraGameObject"));
		

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
		canvasPauseSubMenuAppearance = Instantiate(canvasPauseSubMenuAppearance);
		canvasPauseSubMenuSettings = Instantiate(canvasPauseSubMenuSettings);
		canvasMenuWeaponWheel = Instantiate(canvasMenuWeaponWheel);
		canvasHUDInteraction = Instantiate(canvasHUDInteraction);
		canvasHUDhealthAndMana = Instantiate(canvasHUDhealthAndMana);
		canvasLoadingScreen = Instantiate(canvasLoadingScreen);
		canvasReadNoteMenu = Instantiate(canvasReadNoteMenu);
		canvasLockpickMechanicalMenu = Instantiate(canvasLockpickMechanicalMenu);
		canvasLockpickElectronicMenu = Instantiate(canvasLockpickElectronicMenu);
		canvasDialogueMenu = Instantiate(canvasDialogueMenu);
		canvasHUDammo = Instantiate(canvasHUDammo);
		canvasMenuConfirmAction = Instantiate(canvasMenuConfirmAction);
		yield break;
	}

	private IEnumerator InitializeSceneSystem()
	{
		gameSceneManagerGameObject = new GameObject("GameSceneManager");
		gameSceneManager = gameSceneManagerGameObject.AddComponent<GameSceneManager>();
		loadingScreenText = canvasLoadingScreen.transform.Find("LoadingScreenText").GetComponent<TMP_Text>();
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
		pauseSubMenuAppearanceController = menuManagerGameobject.AddComponent<PauseSubMenuAppearanceController>();
		pauseSubMenuSettingsPlayerPrefs = menuManagerGameobject.AddComponent<PauseSubMenuSettingsPlayerPrefs>();
		pauseSubMenuSettingsController = menuManagerGameobject.AddComponent<PauseSubMenuSettingsController>();
		menuConfirmActionController = menuManagerGameobject.AddComponent<MenuConfirmActionController>();

		buttonCloseMainMenuReadNews = canvasMainMenuReadNews.transform.Find("ExitReadNews").GetComponent<Button>();
		mainMenuReadNews.Initialize(inputDevice, canvasMainMenuReadNews, buttonCloseMainMenuReadNews);

		buttonConfirmAction = FindDeepChildByName(canvasMenuConfirmAction, "Confirm");
		buttonCancelAction = FindDeepChildByName(canvasMenuConfirmAction, "Cancel");
		textShowConfirmationMessage = FindDeepChildByName(canvasMenuConfirmAction, "Text");

		// Кнопки меню
		buttonsPauseMenu = new GameObject[]
		{
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Resume Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Save Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Load Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Appearance Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Settings Button"),
			FindDeepChildByName(canvasPauseMenu, "PauseMenu Exit Button")
		};

		buttonsRewriteGame = new GameObject[]
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
			FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE1 Button"),
			FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE2 Button"),
			FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE3 Button"),
			FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE4 Button"),
			FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE5 Button"),
		};
		buttonClosePauseSubMenuLoad = FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu close Button");

		buttonClosePauseSubMenuAppearance = FindDeepChildByName(canvasPauseSubMenuAppearance, "ImagesSubMenu close Button");
		buttonSaveNewGame = FindDeepChildByName(canvasPauseSubMenuSave, "SaveNewGame");
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

		buttonSaveSettings = FindDeepChildByName(canvasPauseSubMenuSettings, "SaveSettings");
		buttonResetSettings = FindDeepChildByName(canvasPauseSubMenuSettings, "ResetSettings");
	

		// Инициализация меню
		menuManager.Initialize(inputDevice, gameController, saveLoadController);
		pauseMenuController.Initialize(inputDevice, gameController, gameSceneManager, saveLoadController, menuManager, canvasPauseMenu, buttonsPauseMenu);
		pauseSubMenuSaveController.Initialize(inputDevice, menuManager, pauseMenuController, saveLoadController, canvasPauseSubMenuSave, buttonsRewriteGame, buttonsDeleteGame, buttonClosePauseSubMenuSave, buttonSaveNewGame);
		pauseSubMenuLoadController.Initialize(inputDevice, menuManager, pauseMenuController, saveLoadController, canvasPauseSubMenuLoad, buttonsLoadGame, buttonClosePauseSubMenuLoad);
		pauseSubMenuAppearanceController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuAppearance, buttonClosePauseSubMenuAppearance);
		pauseSubMenuSettingsController.Initialize(inputDevice, this, gameController, playerMainCameraGameObject, fovDisplayText, menuManager, pauseMenuController, canvasPauseSubMenuSettings, buttonClosePauseSubMenuSettings, FOVSlider, FPSbuttons, buttonsChangeLanguage, KeyRebinds, pauseSubMenuSettingsPlayerPrefs, buttonSaveSettings, buttonResetSettings);

		menuConfirmActionController.Initialize(canvasMenuConfirmAction, buttonConfirmAction, buttonCancelAction, saveLoadController, pauseSubMenuSaveController, pauseSubMenuLoadController, textShowConfirmationMessage);

		Debug.Log("PAUSE MENU INITIALIZED");
		yield break;
	}

	private IEnumerator InitializePlayerSystems()
	{
		loadingStatusText.text = "Player Systems";

		playerColliderGameObject = FindDeepChildByName(playerGameObject, "Collider");

		// Получение компонентов игрока
		playerBehaviour = playerGameObject.GetComponent<PlayerBehaviour>();
		playerMovementController = playerGameObject.GetComponent<PlayerMovementController>();
		playerColliderController = playerGameObject.GetComponentInChildren<PlayerCapsuleCollider>();
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
		playerColliderController.Initialize(playerMovementController);
		playerCameraController.Initialize(inputDevice, gameSceneManager, menuManager, playerMovementController, playerColliderController, playerGameObject);
		playerCameraBlurFilter.Initialize(menuManager);
		playerAnimationController.Initialize(inputDevice, playerGameObject, playerBehaviour, playerMovementController, playerCameraController);
		playerCameraFirstPersonRender.Initialize(playerCameraController, playerHeadParent);

		Debug.Log("PLAYER SYSTEMS INITIALIZED");
		yield break;
	}

	private IEnumerator InitializePlayerResources()
	{
		loadingStatusText.text = "Player Resources";

		playerResourcesGameObject = new GameObject("PlayerResources");

		playerMoneyTextGameObject = canvasPauseMenu.transform.Find("PauseMenu PlayerMoneyNumber").GetComponent<TMP_Text>();
		HealthBarSlider = canvasHUDhealthAndMana.transform.Find("Health Slider").GetComponent<Slider>();
		HealingItemButton = FindDeepChildByName(canvasMenuWeaponWheel, "HealingItemButton").GetComponent<Button>();
		HealingItemNumber = FindDeepChildByName(canvasMenuWeaponWheel, "HealingItemsNumber").GetComponent<TextMeshProUGUI>();
		ManaBarSlider = canvasHUDhealthAndMana.transform.Find("Mana Slider").GetComponent<Slider>();
		ManaReplenishtemButton = FindDeepChildByName(canvasMenuWeaponWheel, "ManaReplenishItemButton ").GetComponent<Button>();
		ManaReplenishItemNumber = FindDeepChildByName(canvasMenuWeaponWheel, "ManaReplenishItemsNumber").GetComponent<TextMeshProUGUI>();

		canvasHUDhealthAndManaController = playerResourcesGameObject.AddComponent<CanvasHUDhealthAndManaController>();
		canvasHUDammoController = playerResourcesGameObject.AddComponent<CanvasHUDammoController>();
		playerResourcesMoneyManager = playerResourcesGameObject.AddComponent<PlayerResourcesMoneyManager>();
		playerResourcesHealthManager = playerResourcesGameObject.AddComponent<PlayerResourcesHealthManager>();
		playerResourcesManaManager = playerResourcesGameObject.AddComponent<PlayerResourcesManaManager>();
		playerResourcesAmmoManager = playerResourcesGameObject.AddComponent<PlayerResourcesAmmoManager>();

		canvasHUDhealthAndManaController.Initialize(gameSceneManager, gameController, menuManager, canvasHUDhealthAndMana);
		playerResourcesMoneyManager.Initialize(playerMoneyTextGameObject);
		playerResourcesHealthManager.Initialize(gameController, playerBehaviour, HealthBarSlider, HealingItemButton, HealingItemNumber);
		playerResourcesManaManager.Initialize(ManaBarSlider, ManaReplenishtemButton, ManaReplenishItemNumber);

		RightWeaponAmmoMagazine = canvasHUDammo.transform.Find("RightWeaponAmmoMagazine").gameObject;
		RightWeaponAmmoReserve = canvasHUDammo.transform.Find("RightWeaponAmmoReserve").gameObject;
		RightWeaponAmmoSeparator = canvasHUDammo.transform.Find("RightWeaponAmmoSeparator").gameObject;
		LeftWeaponAmmoMagazine = canvasHUDammo.transform.Find("LeftWeaponAmmoMagazine").gameObject;
		LeftWeaponAmmoReserve = canvasHUDammo.transform.Find("LeftWeaponAmmoReserve").gameObject;
		LeftWeaponAmmoSeparator = canvasHUDammo.transform.Find("LeftWeaponAmmoSeparator").gameObject;




		Debug.Log("PLAYER RESOURCES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeInteractionSystem()
	{
		loadingStatusText.text = "Interaction System";

		interactionControllerGameObject = new GameObject("InteractionController");

		interactionController = interactionControllerGameObject.AddComponent<InteractionController>();
		interactionAnimationController = interactionControllerGameObject.AddComponent<InteractionAnimationController>();
		interactionFirstPersonRender = interactionControllerGameObject.AddComponent<InteractionFirstPersonRender>();

		// Элементы HUD
		mainInteractionText = canvasHUDInteraction.transform.Find("mainInteractionText").GetComponent<TextMeshProUGUI>();
		additionalInteractionText = canvasHUDInteraction.transform.Find("additionalInteractionText").GetComponent<TextMeshProUGUI>();
		NPCphrasesText = canvasHUDInteraction.transform.Find("NPCphrases").GetComponent<TextMeshProUGUI>();
		NPCdialogueText = canvasDialogueMenu.transform.Find("NPCdialogue").GetComponent<TextMeshProUGUI>();

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

		buttonsLockElectrical = new GameObject[]
        {
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton1"),
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton2"),
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton3"),
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton4"),
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton5"),
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton6"),
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton7"),
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton8"),
			FindDeepChildByName(canvasLockpickElectronicMenu, "ElectronicLockButton9"),
		};	
		buttonExitReadNoteMenu = canvasReadNoteMenu.transform.Find("ExitReadNote").GetComponent<Button>();
		imageNewspaper = canvasReadNoteMenu.transform.Find("ReadableImage").GetComponent<Image>();
		readableText = canvasReadNoteMenu.transform.Find("ReadableText").GetComponent<TextMeshProUGUI>();
		backgroundBlack = canvasReadNoteMenu.transform.Find("BackgroundBlack").GetComponent<Image>();
		buttonExitLockpickMechanicalMenu = canvasLockpickMechanicalMenu.transform.Find("ExitLockpickMechanical").GetComponent<Button>();
		buttonExitLockpickElectronicMenu = canvasLockpickElectronicMenu.transform.Find("ExitLockpickElectronic").GetComponent<Button>();
		buttonDialogueYes = canvasDialogueMenu.transform.Find("buttonYes").GetComponent<Button>();
		buttonDialogueNo = canvasDialogueMenu.transform.Find("buttonNo").GetComponent<Button>();

		// Инициализация взаимодействия
		interactionController.Initialize(gameController, gameSceneManager, inputDevice, menuManager, playerCameraController, playerBehaviour, canvasHUDInteraction, mainInteractionText,
			additionalInteractionText, itemsTexts, itemsImages);

		//interactionAnimationController.Initialize(playerGameObject, interactionController);
		//interactionFirstPersonRender.Initialize(gameSceneManager, playerCameraController, playerFirstPersonHandRight, playerFirstPersonHandLeft, playerHandRightParent, playerHandLeftParent, interactionController);

		Debug.Log("INTERACTION SYSTEM INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeWeaponSystem()
	{
		loadingStatusText.text = "Weapon System";

		weaponSystemGameObject = new GameObject("WeaponSystem");

		// Основной компонент оружия
		weaponController = weaponSystemGameObject.AddComponent<PlayerWeaponController>();
		legKickAttack = weaponSystemGameObject.AddComponent<LegKickAttack>();
		weaponWheelController = weaponSystemGameObject.AddComponent<WeaponWheelMenuController>();
		weaponAnimationController = weaponSystemGameObject.AddComponent<WeaponAnimationController>();
		weaponFirstPersonRender = weaponSystemGameObject.AddComponent<WeaponFirstPersonRender>();


		// Колесо выбора оружия
		weaponWheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButtons/WeaponWheelButton");
		centerPoint = canvasMenuWeaponWheel.transform.Find("Centre").transform;
		weaponText = canvasMenuWeaponWheel.transform.Find("Selected Weapon Name").GetComponent<TextMeshProUGUI>();
		weaponWheelName = canvasMenuWeaponWheel.transform.Find("WeaponWheel Hand").GetComponent<TextMeshProUGUI>();
		weaponIconBig = canvasMenuWeaponWheel.transform.Find("WeaponBig").GetComponent<Image>();

		firstPersonLeftHandWeaponSlotGameObject = GameObject.Find("Slot1.L");
		thirdPersonLeftHandWeaponSlotGameObject = GameObject.Find("Slot.L");
		firstPersonRightHandWeaponSlotGameObject= GameObject.Find("Slot1.R");
		thirdPersonRightHandWeaponSlotGameObject = GameObject.Find("Slot.R");

		ChokeNPCtext = canvasHUDammo.transform.Find("ChokingText").gameObject;

		// Инициализация оружия
		weaponController.Initialize(inputDevice, menuManager, playerBehaviour, interactionController);
		legKickAttack.Initialize(inputDevice, playerGameObject, playerMovementController);
		weaponWheelController.Initialize(inputDevice, menuManager, playerBehaviour, weaponController, weaponWheelSegmentPrefab,
			centerPoint, canvasMenuWeaponWheel, weaponText, weaponWheelName, weaponIconBig);
		weaponAnimationController.Initialize(playerGameObject, playerBehaviour, playerCameraController, weaponController, legKickAttack);
		weaponFirstPersonRender.Initialize(gameSceneManager, playerCameraController, weaponController, playerFirstPersonHandRight, playerFirstPersonHandLeft, playerHandRightParent, playerHandLeftParent);

		canvasHUDammoController.Initialize(gameSceneManager, gameController, menuManager, canvasHUDammo, weaponController, playerResourcesAmmoManager, playerBehaviour,
				RightWeaponAmmoMagazine,
		RightWeaponAmmoReserve,
		RightWeaponAmmoSeparator,
		LeftWeaponAmmoMagazine,
		LeftWeaponAmmoReserve,
		LeftWeaponAmmoSeparator);

	

		Debug.Log("WEAPON SYSTEM INITIALIZED");
		yield break;
	}

	private IEnumerator RegisterAllDependencies()
	{
		loadingStatusText.text = "Service Locator";

		// Регистрация служб
		ServiceLocator.Register("LocalizationManager", localizationManager);
		ServiceLocator.Register("Player", playerGameObject);
		ServiceLocator.Register("PlayerCameraController", playerCameraController);
		ServiceLocator.Register("MenuManager", menuManager);
		ServiceLocator.Register("WeaponController", weaponController);
		ServiceLocator.Register("ExitReadNote", buttonExitReadNoteMenu);
		ServiceLocator.Register("ExitLockpickMechanical", buttonExitLockpickMechanicalMenu);
		ServiceLocator.Register("ImageNewspaper", imageNewspaper);
		ServiceLocator.Register("ExitLockpickElectronic", buttonExitLockpickElectronicMenu);
		ServiceLocator.Register("ReadableText", readableText);
		ServiceLocator.Register("BackgroundBlack", backgroundBlack);
		ServiceLocator.Register("PlayerResourcesMoneyManager", playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesHealthManager", playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", playerResourcesManaManager);
		ServiceLocator.Register("CanvasLockpickMechanicalMenu", canvasLockpickMechanicalMenu);
		ServiceLocator.Register("CanvasLockpickElectronicMenu", canvasLockpickElectronicMenu);
		ServiceLocator.Register("CanvasReadNoteMenu", canvasReadNoteMenu);
		ServiceLocator.Register("SaveLoadController", saveLoadController);
		ServiceLocator.Register("PauseMenuController", pauseMenuController);
		ServiceLocator.Register("GameController", gameController);
		ServiceLocator.Register("GameSceneManager", gameSceneManager);
		ServiceLocator.Register("PlayerMovementController", playerMovementController);
		ServiceLocator.Register("MainMenuReadNews", mainMenuReadNews);
		ServiceLocator.Register("PlayerCameraBlurFilter", playerCameraBlurFilter);
		ServiceLocator.Register("buttonsLockElectrical", buttonsLockElectrical);
		ServiceLocator.Register("PlayerBehaviour", playerBehaviour);

		ServiceLocator.Register("NPCphrases", NPCphrasesText);
		ServiceLocator.Register("NPCdialogueText", NPCdialogueText);
		ServiceLocator.Register("playerMainCameraGameObject", playerMainCameraGameObject);

		ServiceLocator.Register("firstPersonLeftHandWeaponSlotGameObject", firstPersonLeftHandWeaponSlotGameObject);
		ServiceLocator.Register("firstPersonRightHandWeaponSlotGameObject", firstPersonRightHandWeaponSlotGameObject);
		ServiceLocator.Register("thirdPersonLeftHandWeaponSlotGameObject", thirdPersonLeftHandWeaponSlotGameObject);
		ServiceLocator.Register("thirdPersonRightHandWeaponSlotGameObject", thirdPersonRightHandWeaponSlotGameObject);

		ServiceLocator.Register("CanvasDialogueMenu", canvasDialogueMenu);
		ServiceLocator.Register("buttonDialogueYes", buttonDialogueYes);
		ServiceLocator.Register("buttonDialogueNo", buttonDialogueNo);

		ServiceLocator.Register("playerColliderGameObject", playerColliderGameObject);
		ServiceLocator.Register("playerResourcesAmmoManager", playerResourcesAmmoManager);

		ServiceLocator.Register("ChokeNPCtext", ChokeNPCtext);

		ServiceLocator.Register("inputDevice", inputDevice);

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
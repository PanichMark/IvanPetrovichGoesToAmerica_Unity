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

	///////
	///////
	
	private BootstrapSubSystemMenu bootstrapSubSystemMenu;
	[Header("MainMenu")][SerializeField] private GameObject canvasMainMenuReadNews;
	[Header("PauseMenu")][SerializeField] private GameObject canvasPauseMenu;
	[SerializeField] private GameObject canvasMenuConfirmAction;
	[SerializeField] private GameObject canvasPauseSubMenuSave;
	[SerializeField] private GameObject canvasPauseSubMenuLoad;
	[SerializeField] private GameObject canvasPauseSubMenuAppearance;
	[SerializeField] private GameObject canvasPauseSubMenuSettings;
	[SerializeField] private GameObject canvasCutscene;


	// Система Сцен
	private GameObject gameSceneManagerGameObject;
	[Header("Loading Screen")] [SerializeField] private GameObject canvasLoadingScreen;
	private GameSceneManager gameSceneManager;
	private TMP_Text loadingScreenText;
	private TMP_Text sceneNameText;
	private Image sceneLoadingScreenImage;

	// Система сохранений
	private BootstrapSubSystemSaveLoad bootstrapSubSystemSaveLoad;
	//private GameObject dataSaveLoadControllerGameObject;
	//private SaveLoadController saveLoadController;

	// Игрок

	private BootstrapSubSystemPlayerSystems bootstrapSubSystemPlayerSystems;
	private GameObject playerGameObject;
	private GameObject playerMainCameraGameObject;

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

	private BootstrapSubSystemInteraction bootstrapSubSystemInteraction;
	[Header("Interaction")] [SerializeField] private GameObject canvasHUDInteraction;
	[SerializeField] private GameObject canvasReadNoteMenu;
	[SerializeField] private GameObject canvasLockpickMechanicalMenu;
	[SerializeField] private GameObject canvasLockpickElectronicMenu;
	[SerializeField] private GameObject canvasDialogueMenu;



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

		///
		bootstrapSubSystemSaveLoad = new BootstrapSubSystemSaveLoad(gameSceneManager, gameController);
		yield return StartCoroutine(bootstrapSubSystemSaveLoad.InitializeSaveLoadSystem());
		//
		bootstrapSubSystemMenu = new BootstrapSubSystemMenu(this, inputDevice, gameController, gameSceneManager,
	bootstrapSubSystemSaveLoad.saveLoadController, playerMainCameraGameObject, canvasMainMenuReadNews,
	canvasPauseMenu, canvasMenuConfirmAction, canvasPauseSubMenuSave,
	canvasPauseSubMenuLoad, canvasPauseSubMenuAppearance, canvasPauseSubMenuSettings, canvasCutscene);
		yield return StartCoroutine(bootstrapSubSystemMenu.InitializeMenuSystems());
		//
		bootstrapSubSystemPlayerSystems = new BootstrapSubSystemPlayerSystems(inputDevice,
	gameSceneManager,
	this,
	playerGameObject,
	playerMainCameraGameObject,
	bootstrapSubSystemMenu);
		yield return StartCoroutine(bootstrapSubSystemPlayerSystems.InitializePlayerSystems());
		///
		yield return StartCoroutine(InitializePlayerResources());

		//
		bootstrapSubSystemInteraction = new BootstrapSubSystemInteraction(this,
	bootstrapSubSystemMenu,
	gameController,
	gameSceneManager,
	inputDevice,
	bootstrapSubSystemPlayerSystems.playerCameraController,
	bootstrapSubSystemPlayerSystems.playerBehaviour,
	canvasHUDInteraction,
	canvasReadNoteMenu,
	canvasLockpickMechanicalMenu,
	canvasLockpickElectronicMenu,
	canvasDialogueMenu);
	   yield return StartCoroutine(bootstrapSubSystemInteraction.InitializeInteractionSystem());
		//


		yield return StartCoroutine(InitializeWeaponSystem());
		yield return StartCoroutine(RegisterAllDependencies());
		

		//yield return new WaitForSecondsRealtime(0.3f);
		//yield return new WaitForSecondsRealtime(1f);

		ChangeLanguage(LanguagesEnum.Russian);

		Debug.Log("!!! GAME INITIALIZED !!!");

		yield return StartCoroutine(bootstrapSubSystemSaveLoad.saveLoadController.NewGame());

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

		bootstrapSubSystemPlayerSystems.playerMovementController.SetPlayerPosition(configPlayerPosition.playerPosition);
		
	}

	public void ChangeLanguage(LanguagesEnum language)
	{
		localizationManager.ChangeLanguage(language);

		bootstrapSubSystemInteraction.interactionController.ChangeLanguage(localizationManager);
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
		canvasCutscene = Instantiate(canvasCutscene);
		yield break;
	}

	private IEnumerator InitializeSceneSystem()
	{
		gameSceneManagerGameObject = new GameObject("GameSceneManager");
		gameSceneManager = gameSceneManagerGameObject.AddComponent<GameSceneManager>();
		loadingScreenText = canvasLoadingScreen.transform.Find("LoadingScreenText").GetComponent<TMP_Text>();
		sceneNameText = canvasLoadingScreen.transform.Find("SceneName").GetComponent<TMP_Text>();
		sceneLoadingScreenImage = canvasLoadingScreen.transform.Find("BackgroundImage").GetComponent<Image>();
		gameSceneManager.Initialize(gameController, canvasLoadingScreen, loadingScreenText, sceneNameText, sceneLoadingScreenImage);

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

		canvasHUDhealthAndManaController.Initialize(gameSceneManager, gameController, bootstrapSubSystemMenu.menuManager, canvasHUDhealthAndMana);
		playerResourcesMoneyManager.Initialize(playerMoneyTextGameObject);
		playerResourcesHealthManager.Initialize(gameController, bootstrapSubSystemPlayerSystems.playerBehaviour, HealthBarSlider, HealingItemButton, HealingItemNumber);
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
		weaponController.Initialize(inputDevice, bootstrapSubSystemMenu.menuManager, bootstrapSubSystemPlayerSystems.playerBehaviour, bootstrapSubSystemInteraction.interactionController);
		legKickAttack.Initialize(inputDevice, playerGameObject, bootstrapSubSystemPlayerSystems.playerMovementController);
		weaponWheelController.Initialize(inputDevice, bootstrapSubSystemMenu.menuManager, bootstrapSubSystemPlayerSystems.playerBehaviour, weaponController, weaponWheelSegmentPrefab,
			centerPoint, canvasMenuWeaponWheel, weaponText, weaponWheelName, weaponIconBig);
		weaponAnimationController.Initialize(playerGameObject, bootstrapSubSystemPlayerSystems.playerBehaviour, bootstrapSubSystemPlayerSystems.playerCameraController, weaponController, legKickAttack);
		weaponFirstPersonRender.Initialize(gameSceneManager, bootstrapSubSystemPlayerSystems.playerCameraController, weaponController, bootstrapSubSystemPlayerSystems.playerFirstPersonHandRight, bootstrapSubSystemPlayerSystems.playerFirstPersonHandLeft, bootstrapSubSystemPlayerSystems.playerHandRightParent, bootstrapSubSystemPlayerSystems.playerHandLeftParent);

		canvasHUDammoController.Initialize(gameSceneManager, gameController, bootstrapSubSystemMenu.menuManager, canvasHUDammo, weaponController, playerResourcesAmmoManager, bootstrapSubSystemPlayerSystems.playerBehaviour,
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
		
		
		ServiceLocator.Register("WeaponController", weaponController);
	
		ServiceLocator.Register("PlayerResourcesMoneyManager", playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesHealthManager", playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", playerResourcesManaManager);
		ServiceLocator.Register("CanvasLockpickMechanicalMenu", canvasLockpickMechanicalMenu);
		ServiceLocator.Register("CanvasLockpickElectronicMenu", canvasLockpickElectronicMenu);
		ServiceLocator.Register("CanvasReadNoteMenu", canvasReadNoteMenu);
		
		
		ServiceLocator.Register("GameController", gameController);
		ServiceLocator.Register("GameSceneManager", gameSceneManager);
	

	
		ServiceLocator.Register("playerMainCameraGameObject", playerMainCameraGameObject);

		ServiceLocator.Register("firstPersonLeftHandWeaponSlotGameObject", firstPersonLeftHandWeaponSlotGameObject);
		ServiceLocator.Register("firstPersonRightHandWeaponSlotGameObject", firstPersonRightHandWeaponSlotGameObject);
		ServiceLocator.Register("thirdPersonLeftHandWeaponSlotGameObject", thirdPersonLeftHandWeaponSlotGameObject);
		ServiceLocator.Register("thirdPersonRightHandWeaponSlotGameObject", thirdPersonRightHandWeaponSlotGameObject);

		ServiceLocator.Register("CanvasDialogueMenu", canvasDialogueMenu);
	


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

	public GameObject FindDeepChildByName(GameObject root, string targetName)
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
	// CONFIG
	[Header("--- CONFIGS ---")]
	[SerializeField] private ConfigBootstrapScene configScene;
	[SerializeField] private ConfigBootstrapPlayerPosition configPlayerPosition;
	[SerializeField] private ConfigBootstrapWeapons configWeapons;

	// Экран Инициализации Bootstrap
	private GameObject tempCameraObject;
	[Header("Bootstrap")]
	[SerializeField] private GameObject canvasBootstrap;

	// Интерфейсы
	private IInputDevice inputDevice;
	private LocalizationManager localizationManager;
	private GameController gameController;

	// Меню
	private BootstrapSubSystemMenu bootstrapSubSystemMenu;
	[Header("MainMenu")]
	[SerializeField] private GameObject canvasMainMenuReadNews;
	[Header("PauseMenu")]
	[SerializeField] private GameObject canvasPauseMenu;
	[SerializeField] private GameObject canvasMenuConfirmAction;
	[SerializeField] private GameObject canvasPauseSubMenuSave;
	[SerializeField] private GameObject canvasPauseSubMenuLoad;
	[SerializeField] private GameObject canvasPauseSubMenuAppearance;
	[SerializeField] private GameObject canvasPauseSubMenuSettings;
	[SerializeField] private GameObject canvasCutscene;

	// Система Сцен
	private BootstrapSubSystemScene bootstrapSubSystemScene;
	private GameObject gameSceneManagerGameObject;
	[Header("Loading Screen")]
	[SerializeField] private GameObject canvasLoadingScreen;

	// Система сохранений
	private BootstrapSubSystemSaveLoad bootstrapSubSystemSaveLoad;

	// Игрок
	private BootstrapSubSystemPlayerSystems bootstrapSubSystemPlayerSystems;
	private GameObject playerGameObject;
	private GameObject playerMainCameraGameObject;

	// Игрок ресурсы
	private BootstrapSubSystemPlayerResources bootstrapSubSystemPlayerResources;
	[Header("Player Resources")]
	[SerializeField] private GameObject canvasHUDhealthAndMana;
	[SerializeField] private GameObject canvasHUDammo;

	// Система оружия
	private BootstrapSubSystemWeapon bootstrapSubSystemWeapon;
	[Header("Weapon Wheel Menu")]
	[SerializeField] private GameObject canvasMenuWeaponWheel;

	// Система взаимодействия
	private BootstrapSubSystemInteraction bootstrapSubSystemInteraction;
	[Header("Interaction")]
	[SerializeField] private GameObject canvasHUDInteraction;
	[SerializeField] private GameObject canvasReadNoteMenu;
	[SerializeField] private GameObject canvasLockpickMechanicalMenu;
	[SerializeField] private GameObject canvasLockpickElectronicMenu;
	[SerializeField] private GameObject canvasDialogueMenu;

	private void Awake()
	{
		ServiceLocator.ClearAllServices();
		canvasBootstrap = Instantiate(canvasBootstrap);
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		tempCameraObject = new GameObject("TempCamera");
		tempCameraObject.AddComponent<Camera>();
		StartCoroutine(SequentialInitialization());
	}

	private void OnApplicationQuit()
	{
		ServiceLocator.ClearAllServices();
	}

	private IEnumerator SequentialInitialization()
	{
		yield return StartCoroutine(InitializeInterfaces());
		yield return StartCoroutine(InitializePlayerPrefabs());
		yield return StartCoroutine(InitializeCanvases());

		bootstrapSubSystemScene = new BootstrapSubSystemScene(gameController, canvasLoadingScreen);
		yield return StartCoroutine(bootstrapSubSystemScene.InitializeSceneSystem());

		bootstrapSubSystemSaveLoad = new BootstrapSubSystemSaveLoad(bootstrapSubSystemScene.gameSceneManager, gameController);
		yield return StartCoroutine(bootstrapSubSystemSaveLoad.InitializeSaveLoadSystem());

		bootstrapSubSystemMenu = new BootstrapSubSystemMenu(this, inputDevice, gameController, bootstrapSubSystemScene.gameSceneManager,
			bootstrapSubSystemSaveLoad.saveLoadController, playerMainCameraGameObject, canvasMainMenuReadNews,
			canvasPauseMenu, canvasMenuConfirmAction, canvasPauseSubMenuSave,
			canvasPauseSubMenuLoad, canvasPauseSubMenuAppearance, canvasPauseSubMenuSettings, canvasCutscene);
		yield return StartCoroutine(bootstrapSubSystemMenu.InitializeMenuSystems());

		bootstrapSubSystemPlayerSystems = new BootstrapSubSystemPlayerSystems(inputDevice,
			bootstrapSubSystemScene.gameSceneManager,
			this,
			playerGameObject,
			playerMainCameraGameObject,
			bootstrapSubSystemMenu);
		yield return StartCoroutine(bootstrapSubSystemPlayerSystems.InitializePlayerSystems());

		bootstrapSubSystemPlayerResources = new BootstrapSubSystemPlayerResources(
			canvasPauseMenu,
			canvasMenuWeaponWheel,
			bootstrapSubSystemScene,
			gameController,
			bootstrapSubSystemMenu,
			bootstrapSubSystemPlayerSystems,
			canvasHUDhealthAndMana,
			this,
			canvasHUDammo);
		yield return StartCoroutine(bootstrapSubSystemPlayerResources.InitializePlayerResources());

		bootstrapSubSystemInteraction = new BootstrapSubSystemInteraction(this,
			bootstrapSubSystemMenu,
			gameController,
			bootstrapSubSystemScene.gameSceneManager,
			inputDevice,
			bootstrapSubSystemPlayerSystems.playerCameraController,
			bootstrapSubSystemPlayerSystems.playerBehaviour,
			canvasHUDInteraction,
			canvasReadNoteMenu,
			canvasLockpickMechanicalMenu,
			canvasLockpickElectronicMenu,
			canvasDialogueMenu);
		yield return StartCoroutine(bootstrapSubSystemInteraction.InitializeInteractionSystem());

		bootstrapSubSystemWeapon = new BootstrapSubSystemWeapon(
			bootstrapSubSystemScene,
			gameController,
			bootstrapSubSystemPlayerSystems,
			bootstrapSubSystemMenu,
			inputDevice,
			playerGameObject,
			bootstrapSubSystemPlayerResources.playerResourcesAmmoManager,
			bootstrapSubSystemInteraction,
			bootstrapSubSystemPlayerResources.canvasHUDammoController,
			canvasHUDammo,
			bootstrapSubSystemPlayerResources.RightWeaponAmmoMagazine,
			bootstrapSubSystemPlayerResources.RightWeaponAmmoReserve,
			bootstrapSubSystemPlayerResources.RightWeaponAmmoSeparator,
			bootstrapSubSystemPlayerResources.LeftWeaponAmmoMagazine,
			bootstrapSubSystemPlayerResources.LeftWeaponAmmoReserve,
			bootstrapSubSystemPlayerResources.LeftWeaponAmmoSeparator,
			canvasMenuWeaponWheel);
		yield return StartCoroutine(bootstrapSubSystemWeapon.InitializeWeaponSystem());

		yield return StartCoroutine(RegisterBootstrapDependencies());

		ChangeLanguage(LanguagesEnum.Russian);

		Debug.Log("!!! GAME INITIALIZED !!!");

		yield return StartCoroutine(bootstrapSubSystemSaveLoad.saveLoadController.NewGame());

		GameObject[] availableWeapons = configWeapons.GetAvailableWeapons();
		foreach (GameObject weaponPrefab in availableWeapons)
		{
			bootstrapSubSystemWeapon.weaponController.UnlockWeapon(weaponPrefab);
		}

		Destroy(tempCameraObject);
		Destroy(canvasBootstrap);

		if (configScene.selectedScene.ToString() == "Scene_0_MainMenu")
		{
			yield return StartCoroutine(bootstrapSubSystemScene.gameSceneManager.LoadMainMenuScene());
		}
		else
		{
			yield return StartCoroutine(bootstrapSubSystemScene.gameSceneManager.LoadScene(configScene.selectedScene));
		}

		bootstrapSubSystemPlayerSystems.playerMovementController.SetPlayerPosition(configPlayerPosition.playerPosition);
	}

	public void ChangeLanguage(LanguagesEnum language)
	{
		localizationManager.ChangeLanguage(language);
		bootstrapSubSystemInteraction.interactionController.ChangeLanguage(localizationManager);
		bootstrapSubSystemScene.gameSceneManager.ChangeLanguage(localizationManager);
		ServiceLocator.RemoveService("LocalizationManager");
		ServiceLocator.Register("LocalizationManager", localizationManager);
	}

	private IEnumerator InitializeInterfaces()
	{
		gameController = new GameController();
		localizationManager = new LocalizationManager();
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

		Debug.Log("CANVASES INITIALIZED");
		yield break;
	}

	private IEnumerator RegisterBootstrapDependencies()
	{
		ServiceLocator.Register("LocalizationManager", localizationManager);
		ServiceLocator.Register("GameController", gameController);
		ServiceLocator.Register("inputDevice", inputDevice);
		Debug.Log("SERVICE REGISTERED");
		yield break;
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
		throw new System.Exception($"Child with name {targetName} not found in {root.name}");
	}
}
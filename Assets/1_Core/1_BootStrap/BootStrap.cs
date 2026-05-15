using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
	// CONFIG
	[Header("--- CONFIGS ---")]
	[SerializeField] private ConfigBootstrapInitializationScreenDuration _configBootstrapInitializationScreenDuration;
	[SerializeField] private ConfigBootstrapKeyPauseMenu _configBootstrapKeyPauseMenu;
	[SerializeField] private ConfigBootstrapScene _configBootstrapScene;
	[SerializeField] private ConfigBootstrapPlayerTransform _configBootstrapPlayerPosition;
	[SerializeField] private ConfigBootstrapPlayerWeapons _configBootstrapWeapons;
	private KeyCode _keyPauseMenu;

	// Экран Инициализации Bootstrap
	private GameObject _gameObjectBootstrapTemporaryCamera;
	[Header("Bootstrap")]
	[SerializeField] private GameObject _canvasBootstrap;

	// Интерфейсы
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	// Меню
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	[Header("MainMenu")]
	[SerializeField] private GameObject _canvasMainMenuReadNews;
	[Header("PauseMenu")]
	[SerializeField] private GameObject _canvasPauseMenu;
	[SerializeField] private GameObject _canvasPauseSubMenuSave;
	[SerializeField] private GameObject _canvasPauseSubMenuLoad;
	[SerializeField] private GameObject _canvasPauseSubMenuAppearance;
	[SerializeField] private GameObject _canvasPauseSubMenuSettings;
	[SerializeField] private GameObject _canvasMenuConfirmAction;
	[SerializeField] private GameObject _canvasMenuCutscene;

	// Система Сцен
	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;
	[Header("Loading Screen")]
	[SerializeField] private GameObject _canvasLoadingScreen;

	// Система сохранений
	private BootstrapSubProcessSaveLoadSystem _bootstrapSubProcessSaveLoadSystem;

	// Игрок
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private GameObject _playerGameObject;
	private GameObject _playerCameraGameObject;

	// Игрок ресурсы
	private BootstrapSubProcessPlayerResources _bootstrapSubProcessPlayerResources;
	[Header("Player Resources")]
	[SerializeField] private GameObject _canvasHUDhealthAndMana;
	[SerializeField] private GameObject _canvasHUDammo;

	// Система оружия
	private BootstrapSubProcessWeaponSystem _bootstrapSubProcessWeaponSystem;
	[Header("Weapon Wheel Menu")]
	[SerializeField] private GameObject _canvasMenuWeaponWheel;

	// Система взаимодействия
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;
	[Header("Interaction")]
	[SerializeField] private GameObject _canvasHUDinteraction;
	[SerializeField] private GameObject _canvasMenuNote;
	[SerializeField] private GameObject _canvasMenuLockpickMechanical;
	[SerializeField] private GameObject _canvasMenuLockpickElectronic;
	[SerializeField] private GameObject _canvasMenuDialogue;

	private void Awake()
	{
		ServiceLocator.ClearAllServices();
		_canvasBootstrap = Instantiate(_canvasBootstrap);
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		_gameObjectBootstrapTemporaryCamera = new GameObject("TempCamera");
		_gameObjectBootstrapTemporaryCamera.AddComponent<Camera>();
		StartCoroutine(SequentialInitialization());
	}

	private void OnApplicationQuit()
	{
		ServiceLocator.ClearAllServices();
	}

	private IEnumerator SequentialInitialization()
	{
		yield return StartCoroutine(InitializeInterfaces());
		yield return StartCoroutine(InitializeCanvases());
		yield return StartCoroutine(InitializePlayerPrefabs());

		_bootstrapSubProcessSceneSystem = new BootstrapSubProcessSceneSystem(_gameController, _canvasLoadingScreen);
		yield return StartCoroutine(_bootstrapSubProcessSceneSystem.InitializeSceneSystem());

		_bootstrapSubProcessSaveLoadSystem = new BootstrapSubProcessSaveLoadSystem(_gameController, _bootstrapSubProcessSceneSystem.GameSceneManager);
		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.InitializeSaveLoadSystem());

		_bootstrapSubProcessMenuSystem = new BootstrapSubProcessMenuSystem(
			this,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessSaveLoadSystem.SaveLoadController,
			_playerCameraGameObject,
			_canvasPauseMenu,
			_canvasPauseSubMenuSave,
			_canvasPauseSubMenuLoad,
			_canvasPauseSubMenuAppearance,
			_canvasPauseSubMenuSettings,
			_canvasMenuConfirmAction,
			_canvasMainMenuReadNews,
			_canvasMenuCutscene);
		yield return StartCoroutine(_bootstrapSubProcessMenuSystem.InitializeMenuSystems());

		_bootstrapSubProcessPlayerSystems = new BootstrapSubProcessPlayerSystems(
			this,
			_bootstrapSubProcessMenuSystem,
			_inputDevice,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_playerGameObject,
			_playerCameraGameObject);
		yield return StartCoroutine(_bootstrapSubProcessPlayerSystems.InitializePlayerSystems());

		_bootstrapSubProcessPlayerResources = new BootstrapSubProcessPlayerResources(
			this, 
			_gameController,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_bootstrapSubProcessPlayerSystems,
			_canvasPauseMenu,
			_canvasHUDhealthAndMana,
			_canvasHUDammo,
			_canvasMenuWeaponWheel);
		yield return StartCoroutine(_bootstrapSubProcessPlayerResources.InitializePlayerResources());

		_bootstrapSubProcessInteractionSystem = new BootstrapSubProcessInteractionSystem(
			this,
			_bootstrapSubProcessMenuSystem,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerCameraController,
			_bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController,
			_canvasHUDinteraction,
			_canvasMenuNote,
			_canvasMenuLockpickMechanical,
			_canvasMenuLockpickElectronic,
			_canvasMenuDialogue);
		yield return StartCoroutine(_bootstrapSubProcessInteractionSystem.InitializeInteractionSystem());

		_bootstrapSubProcessWeaponSystem = new BootstrapSubProcessWeaponSystem(
			_gameController,
			_inputDevice,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_bootstrapSubProcessPlayerSystems,
			_bootstrapSubProcessPlayerResources,
			_bootstrapSubProcessInteractionSystem,
			_canvasMenuWeaponWheel,
			_playerGameObject,
			_bootstrapSubProcessPlayerResources.PlayerResourcesAmmoManager,
			_bootstrapSubProcessPlayerResources.CanvasHUDammoController,
			_canvasHUDammo,
			_bootstrapSubProcessPlayerResources.TextRightWeaponAmmoMagazineNumber,
			_bootstrapSubProcessPlayerResources.TextRightWeaponAmmoReserveNumber,
			_bootstrapSubProcessPlayerResources.RightWeaponAmmoSeparator,
			_bootstrapSubProcessPlayerResources.TextLeftWeaponAmmoMagazineNumber,
			_bootstrapSubProcessPlayerResources.TextLeftWeaponAmmoReserveNumber,
			_bootstrapSubProcessPlayerResources.LeftWeaponAmmoSeparator);
		yield return StartCoroutine(_bootstrapSubProcessWeaponSystem.InitializeWeaponSystem());

		yield return StartCoroutine(RegisterBootstrapDependencies());

		yield return new WaitForSecondsRealtime(_configBootstrapInitializationScreenDuration.InitializationScreenDuration);

		ChangeLanguage(LanguagesEnum.Russian);

		Debug.Log("!!! GAME INITIALIZED !!!");

		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.SaveLoadController.NewGame());

		GameObject[] availableWeapons = _configBootstrapWeapons.GetAvailableWeapons();
		if (availableWeapons != null)
		{
			foreach (GameObject weaponPrefab in availableWeapons)
			{
				_bootstrapSubProcessWeaponSystem.WeaponController.UnlockWeapon(weaponPrefab);
			}
		}

		Destroy(_gameObjectBootstrapTemporaryCamera);
		Destroy(_canvasBootstrap);

		if (_configBootstrapScene.SelectedScene.ToString() == "Scene_0_MainMenu")
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadMainMenuScene());
		}
		else
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadGameplayScene(_configBootstrapScene.SelectedScene));
		}

		_bootstrapSubProcessPlayerSystems.PlayerMovementController.SetPlayerPosition(_configBootstrapPlayerPosition.PlayerPosition);
	}

	public void ChangeLanguage(LanguagesEnum newLanguage)
	{
		_localizationManager.ChangeLanguage(newLanguage);
		_bootstrapSubProcessSceneSystem.GameSceneManager.ChangeLanguage(_localizationManager);
		_bootstrapSubProcessInteractionSystem.InteractionController.ChangeLanguage(_localizationManager);
		ServiceLocator.RemoveService("LocalizationManager");
		ServiceLocator.Register("LocalizationManager", _localizationManager);
	}

	private IEnumerator InitializeInterfaces()
	{
		_keyPauseMenu = _configBootstrapKeyPauseMenu.KeyPauseMenu;
		_gameController = new GameController();
		_inputDevice = new InputKeyboard(_gameController, _keyPauseMenu);
		_localizationManager = new LocalizationManager();
		Debug.Log("INTERFACES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeCanvases()
	{
		_canvasMainMenuReadNews = Instantiate(_canvasMainMenuReadNews);
		_canvasPauseMenu = Instantiate(_canvasPauseMenu);
		_canvasPauseSubMenuSave = Instantiate(_canvasPauseSubMenuSave);
		_canvasPauseSubMenuLoad = Instantiate(_canvasPauseSubMenuLoad);
		_canvasPauseSubMenuAppearance = Instantiate(_canvasPauseSubMenuAppearance);
		_canvasPauseSubMenuSettings = Instantiate(_canvasPauseSubMenuSettings);
		_canvasMenuWeaponWheel = Instantiate(_canvasMenuWeaponWheel);
		_canvasHUDinteraction = Instantiate(_canvasHUDinteraction);
		_canvasHUDhealthAndMana = Instantiate(_canvasHUDhealthAndMana);
		_canvasLoadingScreen = Instantiate(_canvasLoadingScreen);
		_canvasMenuNote = Instantiate(_canvasMenuNote);
		_canvasMenuLockpickMechanical = Instantiate(_canvasMenuLockpickMechanical);
		_canvasMenuLockpickElectronic = Instantiate(_canvasMenuLockpickElectronic);
		_canvasMenuDialogue = Instantiate(_canvasMenuDialogue);
		_canvasHUDammo = Instantiate(_canvasHUDammo);
		_canvasMenuConfirmAction = Instantiate(_canvasMenuConfirmAction);
		_canvasMenuCutscene = Instantiate(_canvasMenuCutscene);

		Debug.Log("CANVASES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializePlayerPrefabs()
	{
		_playerGameObject = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/Bootstrap_PlayerGameObject"));
		_playerCameraGameObject = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/Bootstrap_PlayerCameraGameObject"));
		Debug.Log("PLAYER PREFABS INITIALIZED");
		yield break;
	}

	private IEnumerator RegisterBootstrapDependencies()
	{
		ServiceLocator.Register("LocalizationManager", _localizationManager);
		ServiceLocator.Register("GameController", _gameController);
		ServiceLocator.Register("InputDevice", _inputDevice);
		ServiceLocator.Register("KeyPauseMenu", _keyPauseMenu);

		Debug.Log("BOOTSTRAP SERVICES REGISTERED");
		yield break;
	}

	public GameObject FindDeepGameObject(GameObject root, string targetName)
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
	[Header("--- BOOTSTRAP CONFIGS ---")]
	[SerializeField] private ConfigBootstrapFirstGameLaunch _firstGameLaunch;
	[SerializeField] private ConfigBootstrapInitializationScreenDuration _initializationScreenDuration;
	[SerializeField] private ConfigBootstrapKeyPauseMenu _pauseMenuKey;
	[SerializeField] private ConfigBootstrapScene _sceneToLoad;

	[Header("--- PLAYER CONFIGs ---")]
	[SerializeField] private ConfigBootstrapPlayerTransform _playerPosition;
	[SerializeField] private ConfigBootstrapPlayerWeapons _playerWeapons;
	[SerializeField] private ConfigBootstrapPlayerResourcesAmmo _playerAmmo;

	[Header("--- TUTORIAL CONFIG ---")]
	[SerializeField] private TutorialNotesList _tutorialNotes;
	public TutorialNotesList ConfigPauseSubMenuTutorial => _tutorialNotes;

	[Header("Bootstrap")]
	[SerializeField] private GameObject _canvasBootstrap;
	[SerializeField] private GameObject _canvasChooseLanguage;

	[Header("Loading Screen")]
	[SerializeField] private GameObject _canvasLoadingScreen;

	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	[Header("Pause Menu")]
	[SerializeField] private GameObject _canvasPauseMenu;
	[SerializeField] private GameObject _canvasPauseSubMenuSave;
	[SerializeField] private GameObject _canvasPauseSubMenuLoad;
	[SerializeField] private GameObject _canvasPauseSubMenuAppearance;
	[SerializeField] private GameObject _canvasPauseSubMenuTutorial;
	[SerializeField] private GameObject _canvasPauseSubMenuSettings;
	[SerializeField] private GameObject _canvasPauseMenuConfirmAction;

	[Header("Main Menu")]
	[SerializeField] private GameObject _canvasMainMenuReadNews;

	[Header("HUD")]
	[SerializeField] private GameObject _canvasHUDinteraction;
	[SerializeField] private GameObject _canvasHUDhealthAndMana;
	[SerializeField] private GameObject _canvasHUDammo;

	[Header("Weapon Wheel Menu")]
	[SerializeField] private GameObject _canvasMenuWeaponWheel;

	[Header("Interaction Menu")]
	[SerializeField] private GameObject _canvasMenuNote;
	[SerializeField] private GameObject _canvasMenuLockpickMechanical;
	[SerializeField] private GameObject _canvasMenuLockpickElectronic;
	[SerializeField] private GameObject _canvasMenuDialogue;
	[SerializeField] private GameObject _canvasMenuCutscene;

	private FirstLaunchPlayerPrefs _firstLaunchPlayerPrefs;
	private Button _buttonRussianLangage;
	private Button _buttonEnglishLanguage;

	private GameObject _gameObjectBootstrapTemporaryCamera;

	// Интерфейсы
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private KeyCode _keyPauseMenu; // Кнопка открывания/закрывания меню паузы

	// Система Сцен
	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;

	// Система сохранений
	private BootstrapSubProcessSaveLoadSystem _bootstrapSubProcessSaveLoadSystem;

	// Игрок
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private GameObject _playerGameObject;
	private GameObject _playerCameraGameObject;

	// Система оружия
	private BootstrapSubProcessWeaponSystem _bootstrapSubProcessWeaponSystem;
	
	// Система взаимодействия
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;

	private void Awake()
	{
		StartCoroutine(StartGame());
	}

	private IEnumerator StartGame()
	{
		ServiceLocator.ClearAllServices();

		_canvasBootstrap = Instantiate(_canvasBootstrap);

		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		CreateBootstrapTemporaryCamera();

		yield return StartCoroutine(BootstrapSystemsInitialization());

		yield return new WaitForSecondsRealtime(_initializationScreenDuration.InitializationScreenDuration);

		Debug.Log("!!! GAME INITIALIZED !!!");

		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.SaveLoadController.NewGame());

		Destroy(_canvasBootstrap);

		if (_firstLaunchPlayerPrefs.IsFirstLaunch || _firstGameLaunch.IsFirstGameLaunch)
		{
			yield return StartCoroutine(ChooseFirstLanguage());
		}
		else
		{
			ChangeLanguage((LanguagesEnum)Enum.Parse(typeof(LanguagesEnum), PlayerPrefs.GetString("Language")));
		}

		Destroy(_gameObjectBootstrapTemporaryCamera);

		yield return StartCoroutine(LoadFirstGameplayScene());

		ApplyBootstrapPlayerConfigs();
	}

	private IEnumerator BootstrapSystemsInitialization()
	{
		yield return StartCoroutine(InitializeInterfaces());
		yield return StartCoroutine(InitializeCanvases());
		yield return StartCoroutine(InitializePlayerPrefabs());

		yield return StartCoroutine(InitializeSceneSystem());
		yield return StartCoroutine(InitializeSaveLoadSystem());
		yield return StartCoroutine(InitializeMenuSystem());
		yield return StartCoroutine(InitializePlayerSystems());
		yield return StartCoroutine(InitializeInteractionSystem());
		yield return StartCoroutine(InitializeWeaponSystem());

		yield return StartCoroutine(RegisterBootstrapDependencies());
	}

	private IEnumerator InitializeInterfaces()
	{
		_gameController = new GameController();

		_keyPauseMenu = _pauseMenuKey.KeyPauseMenu;
		_inputDevice = new InputKeyboard(_gameController, _keyPauseMenu);

		_localizationManager = new LocalizationManager();

		_firstLaunchPlayerPrefs = new FirstLaunchPlayerPrefs();

		Debug.Log("INTERFACES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeCanvases()
	{
		_canvasLoadingScreen = Instantiate(_canvasLoadingScreen);
		_canvasChooseLanguage = Instantiate(_canvasChooseLanguage);

		_canvasPauseMenu = Instantiate(_canvasPauseMenu);
		_canvasPauseSubMenuSave = Instantiate(_canvasPauseSubMenuSave);
		_canvasPauseSubMenuLoad = Instantiate(_canvasPauseSubMenuLoad);
		_canvasPauseSubMenuAppearance = Instantiate(_canvasPauseSubMenuAppearance);
		_canvasPauseSubMenuTutorial = Instantiate(_canvasPauseSubMenuTutorial);
		_canvasPauseSubMenuSettings = Instantiate(_canvasPauseSubMenuSettings);

		_canvasPauseMenuConfirmAction = Instantiate(_canvasPauseMenuConfirmAction);

		_canvasMenuCutscene = Instantiate(_canvasMenuCutscene);

		_canvasMainMenuReadNews = Instantiate(_canvasMainMenuReadNews);

		_canvasHUDhealthAndMana = Instantiate(_canvasHUDhealthAndMana);

		_canvasHUDinteraction = Instantiate(_canvasHUDinteraction);
		_canvasMenuNote = Instantiate(_canvasMenuNote);
		_canvasMenuLockpickElectronic = Instantiate(_canvasMenuLockpickElectronic);
		_canvasMenuLockpickMechanical = Instantiate(_canvasMenuLockpickMechanical);
		_canvasMenuDialogue = Instantiate(_canvasMenuDialogue);

		_canvasMenuWeaponWheel = Instantiate(_canvasMenuWeaponWheel);
		_canvasHUDammo = Instantiate(_canvasHUDammo);

		Debug.Log("CANVASES INITIALIZED");
		yield break;
	}

	private IEnumerator ChooseFirstLanguage()
	{
		Cursor.lockState = CursorLockMode.None; 
		Cursor.visible = true;

		_canvasChooseLanguage.SetActive(true);

		_buttonRussianLangage = FindDeepGameObject(_canvasChooseLanguage, "Russian").GetComponent<Button>();
		_buttonEnglishLanguage = FindDeepGameObject(_canvasChooseLanguage, "English").GetComponent<Button>();

		bool languageSelected = false;

		_buttonRussianLangage.onClick.AddListener(() =>
		{
			ChangeLanguage(LanguagesEnum.Russian);
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsController.SaveSettings();
			languageSelected = true;
		});

		_buttonEnglishLanguage.onClick.AddListener(() =>
		{
			ChangeLanguage(LanguagesEnum.English);
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsController.SaveSettings();
			languageSelected = true;
		});

		yield return new WaitUntil(() => languageSelected);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		Destroy(_canvasChooseLanguage);

		_firstLaunchPlayerPrefs.SetNotFirstLaunch();
	}

	private IEnumerator InitializePlayerPrefabs()
	{
		_playerGameObject = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/Bootstrap_PlayerGameObject"));
		_playerCameraGameObject = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/Bootstrap_PlayerCameraGameObject"));

		Debug.Log("PLAYER PREFABS INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeSceneSystem()
	{
		_bootstrapSubProcessSceneSystem = new BootstrapSubProcessSceneSystem(_gameController, _canvasLoadingScreen);
		yield return StartCoroutine(_bootstrapSubProcessSceneSystem.InitializeSceneSystem());
	}

	private IEnumerator InitializeSaveLoadSystem()
	{
		_bootstrapSubProcessSaveLoadSystem = new BootstrapSubProcessSaveLoadSystem(_gameController, _bootstrapSubProcessSceneSystem.GameSceneManager);
		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.InitializeSaveLoadSystem());
	}

	private IEnumerator InitializeMenuSystem()
	{
		_bootstrapSubProcessMenuSystem = new BootstrapSubProcessMenuSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_gameController,
			_inputDevice,
			_localizationManager,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessSaveLoadSystem.SaveLoadController,
			_playerCameraGameObject,
			_canvasPauseMenu,
			_canvasPauseSubMenuSave,
			_canvasPauseSubMenuLoad,
			_canvasPauseSubMenuTutorial,
			_canvasPauseSubMenuAppearance,
			_canvasPauseSubMenuSettings,
			_canvasPauseMenuConfirmAction,
			_canvasMainMenuReadNews,
			_canvasMenuCutscene,
			_canvasMenuWeaponWheel,
			_canvasHUDhealthAndMana,
			_canvasHUDammo);
		yield return StartCoroutine(_bootstrapSubProcessMenuSystem.InitializeMenuSystem());
	}

	private IEnumerator InitializePlayerSystems()
	{
		_bootstrapSubProcessPlayerSystems = new BootstrapSubProcessPlayerSystems(
			this,
			_bootstrapSubProcessMenuSystem,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_playerGameObject,
			_playerCameraGameObject);
		yield return StartCoroutine(_bootstrapSubProcessPlayerSystems.InitializePlayerSystems());
	}

	private IEnumerator InitializeInteractionSystem()
	{
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
	}

	private IEnumerator InitializeWeaponSystem()
	{
		_bootstrapSubProcessWeaponSystem = new BootstrapSubProcessWeaponSystem(
			_gameController,
			_inputDevice,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_bootstrapSubProcessPlayerSystems,
			_bootstrapSubProcessInteractionSystem,
			_canvasMenuWeaponWheel,
			_playerGameObject,
			_bootstrapSubProcessPlayerSystems.PlayerResourcesAmmoManager,
			_canvasHUDammo,
			_bootstrapSubProcessMenuSystem.TextRightWeaponAmmoMagazineNumber,
			_bootstrapSubProcessMenuSystem.TextRightWeaponAmmoReserveNumber,
			_bootstrapSubProcessMenuSystem.RightWeaponAmmoSeparator,
			_bootstrapSubProcessMenuSystem.TextLeftWeaponAmmoMagazineNumber,
			_bootstrapSubProcessMenuSystem.TextLeftWeaponAmmoReserveNumber,
			_bootstrapSubProcessMenuSystem.LeftWeaponAmmoSeparator);
		yield return StartCoroutine(_bootstrapSubProcessWeaponSystem.InitializeWeaponSystem());
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

	private void CreateBootstrapTemporaryCamera()
	{
		_gameObjectBootstrapTemporaryCamera = new GameObject("BootstrapTemporaryCamera");
		_gameObjectBootstrapTemporaryCamera.AddComponent<Camera>();
	}

	public void ChangeLanguage(LanguagesEnum newLanguage)
	{
		_localizationManager.ChangeLanguage(newLanguage);
		_bootstrapSubProcessSceneSystem.GameSceneManager.ChangeLanguage(_localizationManager);
		_bootstrapSubProcessInteractionSystem.InteractionController.ChangeLanguage(_localizationManager);
		ServiceLocator.RemoveService("LocalizationManager");
		ServiceLocator.Register("LocalizationManager", _localizationManager);
	}

	private IEnumerator LoadFirstGameplayScene()
	{
		if (_sceneToLoad.SelectedScene.ToString() == "Scene_0_MainMenu")
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadMainMenuScene());
		}
		else
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadGameplayScene(_sceneToLoad.SelectedScene));
		}
	}

	private void ApplyBootstrapPlayerConfigs()
	{
		GameObject[] availableWeapons = _playerWeapons.GetAvailableWeapons();
		if (availableWeapons != null)
		{
			foreach (GameObject weaponPrefab in availableWeapons)
			{
				_bootstrapSubProcessWeaponSystem.WeaponController.UnlockWeapon(weaponPrefab);
			}
		}

		var startAmmoEntries = _playerAmmo.GetStartAmmoEntries();
		if (startAmmoEntries != null && startAmmoEntries.Length > 0)
		{
			foreach (var ammoEntry in startAmmoEntries)
			{
				_bootstrapSubProcessPlayerSystems.PlayerResourcesAmmoManager.SetNewInitialAmmo(
					ammoEntry.AmmoType,
					ammoEntry.StartAmount
				);
			}
		}

		_bootstrapSubProcessPlayerSystems.PlayerMovementController.SetPlayerPosition(_playerPosition.PlayerPosition);
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

	private void OnApplicationQuit()
	{
		ServiceLocator.ClearAllServices();
	}
}
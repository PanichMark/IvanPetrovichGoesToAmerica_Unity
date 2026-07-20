using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
	public delegate void SettingsDataEventHandler();
	public event SettingsDataEventHandler OnLoadSettingsData;

	[Header("--- CANVASES ---")]
	[SerializeField] private GameCanvasesList _gameCanvasesList;

	[Header("--- BOOTSTRAP CONFIGS ---")]
	[SerializeField] private BootstrapConfigIsFirstGameLaunch _configIsFirstGameLaunch;
	[SerializeField] private BootstrapConfigInitializationScreenDuration _configInitializationScreenDuration;
	[SerializeField] private BootstrapConfigKeyPauseMenu _configKeyPauseMenu;
	[SerializeField] private BootstrapConfigFirstSceneToLoad _configFirstSceneToLoad;

	[Header("--- PLAYER CONFIGS ---")]
	[SerializeField] private BootstrapConfigPlayerTransform _playerPosition;
	[SerializeField] private BootstrapConfigPlayerWeapons _playerWeapons;
	[SerializeField] private BootstrapConfigPlayerResourcesAmmo _playerAmmo;

	private GameObject _canvasBootstrap;
	private GameObject _canvasChooseFirstLanguage;
	private GameObject _canvasLoadingScreen;
	private GameObject _canvasMenuBackground;
	private GameObject _canvasPauseMenu;
	private GameObject _canvasPauseSubMenuSave;
	private GameObject _canvasPauseSubMenuLoad;
	private GameObject _canvasPauseSubMenuAppearance;
	private GameObject _canvasPauseSubMenuTutorial;
	private GameObject _canvasPauseSubMenuSettings;
	private GameObject _canvasPauseSubMenuSettingsGameDifficulty;
	private GameObject _canvasPauseMenuConfirmAction;
	private GameObject _canvasMainMenuReadNews;
	private GameObject _canvasHUDinteraction;
	private GameObject _canvasHUDmission;
	private GameObject _canvasHUDhealthAndMana;
	private GameObject _canvasHUDammo;
	private GameObject _canvasMenuWeaponWheel;
	private GameObject _canvasMenuNote;
	private GameObject _canvasMenuLockpickMechanical;
	private GameObject _canvasMenuLockpickElectronic;
	private GameObject _canvasMenuDialogue;
	private GameObject _canvasMenuCutscene;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	public LocalizationManager LocalizationManager { get; private set; }
	private PlayerPrefsData _playerPrefsData;

	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessSaveLoadSystem _bootstrapSubProcessSaveLoadSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;
	private BootstrapSubProcessWeaponSystem _bootstrapSubProcessWeaponSystem;
	private BootstrapSubProcessMissionsSystem _bootstrapSubProcessMissionsSystem;

	private KeyCode _keyPauseMenu;

	private GameObject _gameObjectPlayer;
	public GameObject GameObjectPlayerCamera { get; private set; }
	private GameObject _gameObjectBootstrapTemporaryCamera;

	/*
	private bool _isGamepadConnected; // TODO add gamepad support
	*/

	public bool IsBootstrapInitialized { get; private set; }

	private IEnumerator Start()
	{
		Debug.Log("!!! STARTED GAME INITIALIZATION !!!");

		ServiceLocator.ClearAllServices();

		_canvasBootstrap = Instantiate(_gameCanvasesList.CanvasBootstrap);

		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		CreateBootstrapTemporaryCamera();

		yield return StartCoroutine(BootstrapSystemsInitialization());

		yield return new WaitForSecondsRealtime(_configInitializationScreenDuration.InitializationScreenDuration);

		Debug.Log("!!! GAME INITIALIZED !!!");

		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.SaveLoadController.NewGame());

		Destroy(_canvasBootstrap);

		//PlayerPrefs.DeleteAll(); // DO NOT DELETE, for testing purporses

		if (_playerPrefsData.IsFirstLaunch || _configIsFirstGameLaunch.IsFirstGameLaunch)
		{
			yield return StartCoroutine(ChooseFirstLanguage());
		}
		else
		{
			ChangeLanguage((LanguagesEnum)Enum.Parse(typeof(LanguagesEnum), PlayerPrefs.GetString("Language")));
		}

		Destroy(_canvasChooseFirstLanguage);

		Destroy(_gameObjectBootstrapTemporaryCamera);

		yield return StartCoroutine(LoadFirstGameplayScene());

		ApplyBootstrapPlayerConfigs();

		OnLoadSettingsData?.Invoke();

		IsBootstrapInitialized = true;
	}

	private IEnumerator BootstrapSystemsInitialization()
	{
		yield return StartCoroutine(InitializeInterfaces());
		yield return StartCoroutine(InitializeCanvases());
		yield return StartCoroutine(InitializeSceneSystem());
		yield return StartCoroutine(InitializeSaveLoadSystem());
		yield return StartCoroutine(InitializeMenuSystem());
		yield return StartCoroutine(InitializePlayerSystems());
		yield return StartCoroutine(InitializeInteractionSystem());
		yield return StartCoroutine(InitializeWeaponSystem());
		yield return StartCoroutine(InitializeMissionsSystem());

		yield return StartCoroutine(RegisterBootstrapDependencies());
	}

	private IEnumerator InitializeInterfaces()
	{
		_gameController = new GameController();

		_keyPauseMenu = _configKeyPauseMenu.KeyPauseMenu;

		_inputDevice = new InputKeyboard(_gameController, _keyPauseMenu);
		//_inputDevice = new InputGamepad(_gameController);

		LocalizationManager = new LocalizationManager();

		_playerPrefsData = new PlayerPrefsData();

		Debug.Log("=== INTERFACES INITIALIZED ===");
		yield break;
	}

	private IEnumerator InitializeCanvases()
	{
		_canvasChooseFirstLanguage = Instantiate(_gameCanvasesList.CanvasChooseFirstLanguage);

		_canvasLoadingScreen = Instantiate(_gameCanvasesList.CanvasLoadingScreen);
	
		_canvasMenuBackground = Instantiate(_gameCanvasesList.CanvasMenuBackground);

	    _canvasPauseMenu = Instantiate(_gameCanvasesList.CanvasPauseMenu);
		_canvasPauseSubMenuSave = Instantiate(_gameCanvasesList.CanvasPauseSubMenuSave);
		_canvasPauseSubMenuLoad = Instantiate(_gameCanvasesList.CanvasPauseSubMenuLoad);
		_canvasPauseSubMenuAppearance = Instantiate(_gameCanvasesList.CanvasPauseSubMenuAppearance);
		_canvasPauseSubMenuTutorial = Instantiate(_gameCanvasesList.CanvasPauseSubMenuTutorial);
		_canvasPauseSubMenuSettings = Instantiate(_gameCanvasesList.CanvasPauseSubMenuSettings);
		_canvasPauseSubMenuSettingsGameDifficulty = Instantiate(_gameCanvasesList.CanvasPauseSubMenuSettingsGameDifficulty);
		_canvasPauseMenuConfirmAction = Instantiate(_gameCanvasesList.CanvasPauseMenuConfirmAction);

		_canvasMainMenuReadNews = Instantiate(_gameCanvasesList.CanvasMainMenuReadNews);

		_canvasMenuWeaponWheel = Instantiate(_gameCanvasesList.CanvasMenuWeaponWheel);

		_canvasMenuWeaponWheel = Instantiate(_gameCanvasesList.CanvasMenuWeaponWheel);
		_canvasMenuCutscene = Instantiate(_gameCanvasesList.CanvasMenuCutscene);

		_canvasHUDhealthAndMana = Instantiate(_gameCanvasesList.CanvasHUDhealthAndMana);
		_canvasHUDammo = Instantiate(_gameCanvasesList.CanvasHUDammo);
		_canvasHUDinteraction = Instantiate(_gameCanvasesList.CanvasHUDinteraction);
		_canvasHUDmission = Instantiate(_gameCanvasesList.CanvasHUDmission);

		_canvasMenuNote = Instantiate(_gameCanvasesList.CanvasMenuNote);
		_canvasMenuLockpickElectronic = Instantiate(_gameCanvasesList.CanvasMenuLockpickElectronic);
		_canvasMenuLockpickMechanical = Instantiate(_gameCanvasesList.CanvasMenuLockpickMechanical);
		_canvasMenuDialogue = Instantiate(_gameCanvasesList.CanvasMenuDialogue);

		Debug.Log("=== CANVASES INITIALIZED ===");
		yield break;
	}

	private IEnumerator InitializeSceneSystem()
	{
		_bootstrapSubProcessSceneSystem = new BootstrapSubProcessScenesSystem(
			this, 
			_gameController,
			LocalizationManager,
			_canvasLoadingScreen);

		yield return StartCoroutine(_bootstrapSubProcessSceneSystem.InitializeSceneSystem());

		Debug.Log("=== SCENE SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializeSaveLoadSystem()
	{
		_bootstrapSubProcessSaveLoadSystem = new BootstrapSubProcessSaveLoadSystem(
			_gameController,
			_bootstrapSubProcessSceneSystem);

		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.InitializeSaveLoadSystem());

		Debug.Log("=== SAVELOAD SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializeMenuSystem()
	{
		_bootstrapSubProcessMenuSystem = new BootstrapSubProcessMenuSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessSaveLoadSystem,
			_gameController,
			_inputDevice,
			LocalizationManager,
			_canvasChooseFirstLanguage,
			_canvasMenuBackground,
			_canvasPauseMenu,
			_canvasPauseSubMenuSave,
			_canvasPauseSubMenuLoad,
			_canvasPauseSubMenuAppearance,
			_canvasPauseSubMenuTutorial,
			_canvasPauseSubMenuSettings,
			_canvasPauseSubMenuSettingsGameDifficulty,
			_canvasPauseMenuConfirmAction,
			_canvasMainMenuReadNews,
			_canvasMenuWeaponWheel,
			_canvasMenuCutscene,
			_canvasHUDhealthAndMana,
			_canvasHUDammo,
			_canvasHUDinteraction,
			_canvasHUDmission,
			_canvasMenuNote,
			_canvasMenuLockpickMechanical,
			_canvasMenuLockpickElectronic,
			_canvasMenuDialogue);

		yield return StartCoroutine(_bootstrapSubProcessMenuSystem.InitializeMenuSystem());

		Debug.Log("=== MENU SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializePlayerSystems()
	{
		_gameObjectPlayer = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/Bootstrap_PlayerGameObject"));
		GameObjectPlayerCamera = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/Bootstrap_PlayerCameraGameObject"));

		_bootstrapSubProcessPlayerSystems = new BootstrapSubProcessPlayerSystems(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_gameController,
			_inputDevice,
			_canvasMenuBackground,
			_gameObjectPlayer,
			GameObjectPlayerCamera);

		yield return StartCoroutine(_bootstrapSubProcessPlayerSystems.InitializePlayerSystems());

		Debug.Log("=== PLAYER SYSTEMS INITIALIZED ===");
	}

	private IEnumerator InitializeInteractionSystem()
	{
		_bootstrapSubProcessInteractionSystem = new BootstrapSubProcessInteractionSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_bootstrapSubProcessPlayerSystems,
			_gameController,
			_inputDevice,
			LocalizationManager);

		yield return StartCoroutine(_bootstrapSubProcessInteractionSystem.InitializeInteractionSystem());

		Debug.Log("=== INTERACTION SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializeWeaponSystem()
	{
		_bootstrapSubProcessWeaponSystem = new BootstrapSubProcessWeaponSystem(
			this,
			_gameController,
			_inputDevice,
			LocalizationManager,
			_gameObjectPlayer,
			GameObjectPlayerCamera,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_bootstrapSubProcessPlayerSystems,
			_bootstrapSubProcessInteractionSystem);

		yield return StartCoroutine(_bootstrapSubProcessWeaponSystem.InitializeWeaponSystem());

		Debug.Log("=== WEAPON SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializeMissionsSystem()
	{
		_bootstrapSubProcessMissionsSystem = new BootstrapSubProcessMissionsSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			GameObjectPlayerCamera);

		yield return StartCoroutine(_bootstrapSubProcessMissionsSystem.InitializeMissionsSystem());

		Debug.Log("=== MISSIONS SYSTEM INITIALIZED ===");
	}

	private IEnumerator RegisterBootstrapDependencies()
	{
		ServiceLocator.Register("LocalizationManager", LocalizationManager);
		ServiceLocator.Register("GameController", _gameController);
		ServiceLocator.Register("InputDevice", _inputDevice);
		ServiceLocator.Register("KeyPauseMenu", _keyPauseMenu);

		Debug.Log("=== BOOTSTRAP SERVICES REGISTERED ===");

		yield break;
	}

	private void CreateBootstrapTemporaryCamera()
	{
		_gameObjectBootstrapTemporaryCamera = new GameObject("BootstrapTemporaryCamera");
		_gameObjectBootstrapTemporaryCamera.AddComponent<Camera>();
	}

	public void ChangeLanguage(LanguagesEnum newLanguage)
	{
		LocalizationManager.ChangeLanguage(newLanguage);

		ServiceLocator.RemoveService("LocalizationManager");
		ServiceLocator.Register("LocalizationManager", LocalizationManager);
	}

	public void ChangeWeaponWheelType(WeaponWheelMenuTypes weaponWheelMenuTypes)
	{
		_bootstrapSubProcessWeaponSystem.ChangeWeaponWheelType(weaponWheelMenuTypes);
	}

	private IEnumerator LoadFirstGameplayScene()
	{
		if (_configFirstSceneToLoad.FirstSceneToLoad == GameScenesEnum.Scene_0_MainMenu)
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadMainMenuScene());
		}
		else
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadGameplayScene(_configFirstSceneToLoad.FirstSceneToLoad));
		}
	}

	private IEnumerator ChooseFirstLanguage()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		_canvasChooseFirstLanguage.SetActive(true);

		bool languageSelected = false;

		_bootstrapSubProcessMenuSystem.ViewModelMenuChooseFirstLanguage.ButtonRussianLangauge.GetComponent<Button>().onClick.AddListener(() =>
		{
			ChangeLanguage(LanguagesEnum.Russian);
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionAudioController.SaveSettingsAudio();
			languageSelected = true;
		});

		_bootstrapSubProcessMenuSystem.ViewModelMenuChooseFirstLanguage.ButtonEnglishLanguage.GetComponent<Button>().onClick.AddListener(() =>
		{
			ChangeLanguage(LanguagesEnum.English);
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionAudioController.SaveSettingsAudio();
			languageSelected = true;
		});

		yield return new WaitUntil(() => languageSelected);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		Destroy(_canvasChooseFirstLanguage);

		_playerPrefsData.SetNotFirstLaunch();
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

		if (_configFirstSceneToLoad.FirstSceneToLoad != GameScenesEnum.Scene_0_MainMenu)
		{
			_bootstrapSubProcessPlayerSystems.PlayerMovementController.SetPlayerPosition(_playerPosition.PlayerPosition);
		}
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

	// TODO add gamepad support

	/*
	private void Update()
	{
		if (!_isInitialized)
			return;

		string[] joysticks = Input.GetJoystickNames();
		bool isGamepadConnected = joysticks.Length > 0 && !string.IsNullOrEmpty(joysticks[0]);

		if (isGamepadConnected)
		{
			if (!_isGamepadConnected)
			{
				_isGamepadConnected = true;
				Debug.Log("Геймпад подключен: " + joysticks[0]);
			}

			//////////////////////////////////////////////////////////////
			// Проверяем, была ли нажата любая клавиша в этом кадре
			if (Input.anyKeyDown)
			{
				// Обычно у геймпада 20 кнопок (0-19). Цикл проверяет их все.
				// Если у вашего геймпада больше кнопок, увеличьте число 20.
				for (int i = 0; i < 20; i++)
				{
					// Проверяем конкретную кнопку по её индексу
					if (Input.GetKeyDown($"joystick button {i}"))
					{
						// Если кнопка нажата, выводим её индекс в консоль
						Debug.Log($"Нажата кнопка геймпада с индексом: {i}");

						// Раскомментируйте следующую строку, если хотите,
						// чтобы лог выводился только для первой найденной кнопки.
						// return; 
					}
				}
			}
			//////////////////////////////////////////////////////////////////
		}
		else
		{
			if (_isGamepadConnected)
			{
				_isGamepadConnected = false;
				Debug.Log("Геймпад отключен");
			}
		}
	}
	*/

	/*
	public void ChangeInputDevice(IInputDevice inputDevice)
	{
		//_inputDevice = new InputKeyboard(_gameController, _keyPauseMenu);

		//_inputDevice = new InputGamepad(_gameController, _keyPauseMenu);
	}
	*/
}
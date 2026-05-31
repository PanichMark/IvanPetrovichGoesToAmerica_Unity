using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
	public delegate void SettingsDataEventHandler();
	public event SettingsDataEventHandler OnLoadSettingsData;

	[Header("--- BOOTSTRAP CONFIGS ---")]
	[SerializeField] private ConfigBootstrapFirstGameLaunch _firstGameLaunch;
	[SerializeField] private ConfigBootstrapInitializationScreenDuration _initializationScreenDuration;
	[SerializeField] private ConfigBootstrapKeyPauseMenu _pauseMenuKey;
	[SerializeField] private ConfigBootstrapScene _sceneToLoad;

	[Header("--- PLAYER CONFIGs ---")]
	[SerializeField] private ConfigBootstrapPlayerTransform _playerPosition;
	[SerializeField] private ConfigBootstrapPlayerWeapons _playerWeapons;
	[SerializeField] private ConfigBootstrapPlayerResourcesAmmo _playerAmmo;

	[Header("--- GAME MISSIONS ---")]
	[SerializeField] private GameMissions _allMissions;

	[Header("--- TUTORIAL CONFIG ---")]
	[SerializeField] private TutorialNotesList _tutorialNotes;
	public TutorialNotesList ConfigPauseSubMenuTutorial => _tutorialNotes;

	[Header("Bootstrap")]
	[SerializeField] private GameObject _canvasBootstrap;
	[SerializeField] private GameObject _canvasChooseLanguage;

	[Header("Loading Screen")]
	[SerializeField] private GameObject _canvasLoadingScreen;

	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	[Header("Menu")]
	[SerializeField] private GameObject _canvasMenuBackground;
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
	[SerializeField] private GameObject _canvasHUDmission;
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

	private PlayerPrefsData _playerPrefsData;
	private Button _buttonRussianLangage;
	private Button _buttonEnglishLanguage;

	private GameObject _gameObjectBootstrapTemporaryCamera;

	// Интерфейсы
	private GameController _gameController;
	private IInputDevice _inputDevice;
	public LocalizationManager LocalizationManager { get; private set; }
	private KeyCode _keyPauseMenu; // Кнопка открывания/закрывания меню паузы
	private bool _isInitialized;
	private bool _isGamepadConnected;

	// Система Сцен
	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;

	// Система сохранений
	private BootstrapSubProcessSaveLoadSystem _bootstrapSubProcessSaveLoadSystem;

	// Игрок
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private GameObject _gameObjectPlayer;
	private GameObject _gameObjectPlayerCamera;

	// Система взаимодействия
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;

	// Система оружия
	private BootstrapSubProcessWeaponSystem _bootstrapSubProcessWeaponSystem;

	private BootstrapSubProcessMissionsSystem _bootstrapSubProcessMissionsSystem;

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

		//PlayerPrefs.DeleteAll(); // ПРОВЕРЯТЬ!!! НЕ УДАЛЯТЬ!!!

		if (_playerPrefsData.IsFirstLaunch || _firstGameLaunch.IsFirstGameLaunch)
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

		OnLoadSettingsData?.Invoke();

		_isInitialized = true;
	}

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

		_keyPauseMenu = _pauseMenuKey.KeyPauseMenu;

		_inputDevice = new InputKeyboard(_gameController, _keyPauseMenu);
		//_inputDevice = new InputGamepad(_gameController);

		LocalizationManager = new LocalizationManager();

		_playerPrefsData = new PlayerPrefsData();

		Debug.Log("INTERFACES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeCanvases()
	{
		_canvasChooseLanguage = Instantiate(_canvasChooseLanguage);

		_canvasLoadingScreen = Instantiate(_canvasLoadingScreen);
	
		_canvasMenuBackground = Instantiate(_canvasMenuBackground);

	    _canvasPauseMenu = Instantiate(_canvasPauseMenu);
		_canvasPauseSubMenuSave = Instantiate(_canvasPauseSubMenuSave);
		_canvasPauseSubMenuLoad = Instantiate(_canvasPauseSubMenuLoad);
		_canvasPauseSubMenuAppearance = Instantiate(_canvasPauseSubMenuAppearance);
		_canvasPauseSubMenuTutorial = Instantiate(_canvasPauseSubMenuTutorial);
		_canvasPauseSubMenuSettings = Instantiate(_canvasPauseSubMenuSettings);
		_canvasPauseMenuConfirmAction = Instantiate(_canvasPauseMenuConfirmAction);

		_canvasMainMenuReadNews = Instantiate(_canvasMainMenuReadNews);

		_canvasMenuWeaponWheel = Instantiate(_canvasMenuWeaponWheel);

		_canvasMenuCutscene = Instantiate(_canvasMenuCutscene);

		_canvasHUDhealthAndMana = Instantiate(_canvasHUDhealthAndMana);
		_canvasHUDammo = Instantiate(_canvasHUDammo);
		_canvasHUDinteraction = Instantiate(_canvasHUDinteraction);
		_canvasHUDmission = Instantiate(_canvasHUDmission);

		_canvasMenuNote = Instantiate(_canvasMenuNote);
		_canvasMenuLockpickElectronic = Instantiate(_canvasMenuLockpickElectronic);
		_canvasMenuLockpickMechanical = Instantiate(_canvasMenuLockpickMechanical);
		_canvasMenuDialogue = Instantiate(_canvasMenuDialogue);

		Debug.Log("CANVASES INITIALIZED");
		yield break;
	}

	private IEnumerator InitializeSceneSystem()
	{
		_bootstrapSubProcessSceneSystem = new BootstrapSubProcessSceneSystem(
			this, 
			_gameController,
			_canvasLoadingScreen);

		yield return StartCoroutine(_bootstrapSubProcessSceneSystem.InitializeSceneSystem());
	}

	private IEnumerator InitializeSaveLoadSystem()
	{
		_bootstrapSubProcessSaveLoadSystem = new BootstrapSubProcessSaveLoadSystem(
			_gameController,
			_bootstrapSubProcessSceneSystem);

		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.InitializeSaveLoadSystem());
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
			_canvasMenuBackground,
			_canvasPauseMenu,
			_canvasPauseSubMenuSave,
			_canvasPauseSubMenuLoad,
			_canvasPauseSubMenuAppearance,
			_canvasPauseSubMenuTutorial,
			_canvasPauseSubMenuSettings,
			_canvasPauseMenuConfirmAction,
			_canvasMainMenuReadNews,
			_canvasMenuWeaponWheel,
			_canvasMenuCutscene,
			_canvasHUDhealthAndMana,
			_canvasHUDammo,
			_canvasHUDmission);

		yield return StartCoroutine(_bootstrapSubProcessMenuSystem.InitializeMenuSystem());
	}

	private IEnumerator InitializePlayerSystems()
	{
		_gameObjectPlayer = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/Bootstrap_PlayerGameObject"));
		_gameObjectPlayerCamera = Instantiate((GameObject)Resources.Load("1_Bootstrap/BootstrapPlayer/Bootstrap_PlayerCameraGameObject"));

		_bootstrapSubProcessPlayerSystems = new BootstrapSubProcessPlayerSystems(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_gameController,
			_inputDevice,
			_canvasMenuBackground,
			_gameObjectPlayer,
			_gameObjectPlayerCamera);

		yield return StartCoroutine(_bootstrapSubProcessPlayerSystems.InitializePlayerSystems());
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
			LocalizationManager,
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
			_gameObjectPlayer);

		yield return StartCoroutine(_bootstrapSubProcessWeaponSystem.InitializeWeaponSystem());
	}

	private IEnumerator InitializeMissionsSystem()
	{
		_bootstrapSubProcessMissionsSystem = new BootstrapSubProcessMissionsSystem(
			this,
			_bootstrapSubProcessMenuSystem,
			_allMissions,
			_gameObjectPlayerCamera);

		yield return StartCoroutine(_bootstrapSubProcessMissionsSystem.InitializeMissionsSystem());
	}

	private IEnumerator RegisterBootstrapDependencies()
	{
		ServiceLocator.Register("LocalizationManager", LocalizationManager);
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
		LocalizationManager.ChangeLanguage(newLanguage);
		_bootstrapSubProcessSceneSystem.GameSceneManager.ChangeLanguage(LocalizationManager);
		_bootstrapSubProcessInteractionSystem.InteractionController.ChangeLanguage(LocalizationManager);
		_bootstrapSubProcessMissionsSystem.ChangeLanguage();
		ServiceLocator.RemoveService("LocalizationManager");
		ServiceLocator.Register("LocalizationManager", LocalizationManager);
	}

	public void ChangeInputDevice(IInputDevice inputDevice)
	{
		//_inputDevice = new InputKeyboard(_gameController, _keyPauseMenu);

		//_inputDevice = new InputGamepad(_gameController, _keyPauseMenu);
	}

	private IEnumerator LoadFirstGameplayScene()
	{
		if (_sceneToLoad.SelectedScene.ToString() == "Scene0_MainMenu")
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadMainMenuScene());
		}
		else
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadGameplayScene(_sceneToLoad.SelectedScene));
		}
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
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionAudioController.SaveSettingsAudio();
			languageSelected = true;
		});

		_buttonEnglishLanguage.onClick.AddListener(() =>
		{
			ChangeLanguage(LanguagesEnum.English);
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionAudioController.SaveSettingsAudio();
			languageSelected = true;
		});

		yield return new WaitUntil(() => languageSelected);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		Destroy(_canvasChooseLanguage);

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

		if (_sceneToLoad.SelectedScene.ToString() != "Scene0_MainMenu")
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
}
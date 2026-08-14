using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
	public delegate void SettingsDataEventHandler();
	public event SettingsDataEventHandler OnLoadSettingsData;

	[Header("--- GAME DATA ---")]
	[SerializeField] private BootstrapGameDataList _gameData;
	public BootstrapGameDataList GameData => _gameData;

	[Header("---  CONFIGS PLAYERPREFS ---")]
	[SerializeField] private ConfigPlayerPrefsPresequisites _playerPrefsPresequisites;
	[SerializeField] private ConfigPlayerPrefsReset _playerPrefsReset;

	[Header("---  CONFIGS BOOTSTRAP ---")]
	[SerializeField] private ConfigBootstrapInitializationScreenDuration _initializationScreenDuration;
	[SerializeField] private ConfigBootstrapKeyPauseMenu _keyPauseMenu;
	[SerializeField] private ConfigBootstrapFirstSceneToLoad _firstSceneToLoad;

	[Header("--- CONFIGS PLAYER  ---")]
	[SerializeField] private ConfigPlayerTransform _playerTransform;
	[SerializeField] private ConfigPlayerWeapons _playerWeapons;
	[SerializeField] private ConfigPlayerResourcesAmmo _playerAmmo;

	private GameObject _canvasBootstrapInitialization;
	private GameObject _canvasBootstrapChooseFirstLanguage;
	private GameObject _canvasBootstrapSignTermsAndConditions;
	private GameObject _canvasSceneLoadingScreen;
	private GameObject _canvasMenuBackground;
	private GameObject _canvasPauseMenu;
	private GameObject _canvasPauseSubMenuSave;
	private GameObject _canvasPauseSubMenuLoad;
	private GameObject _canvasPauseSubMenuAppearance;
	private GameObject _canvasPauseSubMenuTutorial;
	private GameObject _canvasPauseSubMenuSettings;
	private GameObject _canvasPauseSubMenuSettingsGameDifficulty;
	private GameObject _canvasPauseMenuConfirmAction;
	private GameObject _canvasMainMenuChooseMission;
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
	private BootstrapSubProcessPlayerPrefsSystem _bootstrapSubProcessPlayerPrefsSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;
	private BootstrapSubProcessWeaponSystem _bootstrapSubProcessWeaponSystem;
	private BootstrapSubProcessMissionsSystem _bootstrapSubProcessMissionsSystem;
	private BootstrapSubProcessObjectPoolSystem _bootstrapSubProcessObjectPoolSystem;

	private KeyCode _keyCodePauseMenu;

	private GameObject _gameObjectPlayer;
	public GameObject GameObjectPlayerCamera { get; private set; }
	private GameObject _gameObjectBootstrapTemporaryCamera;
	public bool IsBootstrapInitialized { get; private set; }

	private IEnumerator Start()
	{
		Debug.Log("!!! STARTED GAME INITIALIZATION !!!");

		ServiceLocator.ClearAllServices();

		_canvasBootstrapInitialization = Instantiate(_gameData.GameCanvasesList.CanvasBootstrapInitialization);

		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		CreateBootstrapTemporaryCamera();

		yield return StartCoroutine(BootstrapSystemsInitialization());

		yield return new WaitForSecondsRealtime(_initializationScreenDuration.InitializationScreenDuration);

		Debug.Log("!!! GAME INITIALIZED !!!");

		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.SaveLoadController.NewGame());

		Destroy(_canvasBootstrapInitialization);

		if (_playerPrefsReset.ResetPlayerPrefs == true)
		{
			PlayerPrefs.DeleteAll();
		}

		if (_playerPrefsData.BootstrapArePrerequisitesMet == false || _playerPrefsPresequisites.ArePrerequisitesMet == false)
		{
			yield return StartCoroutine(BootstrapPrerequisites());
		}
		else
		{
			ChangeLanguage((LanguagesEnum)Enum.Parse(typeof(LanguagesEnum), PlayerPrefs.GetString(PlayerPrefsSettingsSectionAudioEnum.Language.ToString())));
		}

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
		yield return StartCoroutine(InitializePlayerPrefsSystem());
		yield return StartCoroutine(InitializeInteractionSystem());
		yield return StartCoroutine(InitializeWeaponSystem());
		yield return StartCoroutine(InitializeMissionsSystem());
		yield return StartCoroutine(InitializeObjectPoolSystem());

		yield return StartCoroutine(RegisterBootstrapDependencies());
	}

	private IEnumerator InitializeInterfaces()
	{
		_gameController = new GameController();

		_keyCodePauseMenu = _keyPauseMenu.KeyPauseMenu;

		_inputDevice = new InputKeyboard(_gameController, _keyCodePauseMenu);

		LocalizationManager = new LocalizationManager(this);

		_playerPrefsData = new PlayerPrefsData();

		Debug.Log("=== INTERFACES INITIALIZED ===");
		yield break;
	}

	private IEnumerator InitializeCanvases()
	{
		_canvasBootstrapChooseFirstLanguage = Instantiate(_gameData.GameCanvasesList.CanvasBootstrapChooseFirstLanguage);
		_canvasBootstrapSignTermsAndConditions = Instantiate(_gameData.GameCanvasesList.CanvasBootstrapSignTermsAndConditions);

		_canvasSceneLoadingScreen = Instantiate(_gameData.GameCanvasesList.CanvasSceneLoadingScreen);
	
		_canvasMenuBackground = Instantiate(_gameData.GameCanvasesList.CanvasMenuBackground);

	    _canvasPauseMenu = Instantiate(_gameData.GameCanvasesList.CanvasPauseMenu);
		_canvasPauseSubMenuSave = Instantiate(_gameData.GameCanvasesList.CanvasPauseSubMenuSave);
		_canvasPauseSubMenuLoad = Instantiate(_gameData.GameCanvasesList.CanvasPauseSubMenuLoad);
		_canvasPauseSubMenuAppearance = Instantiate(_gameData.GameCanvasesList.CanvasPauseSubMenuAppearance);
		_canvasPauseSubMenuTutorial = Instantiate(_gameData.GameCanvasesList.CanvasPauseSubMenuTutorial);
		_canvasPauseSubMenuSettings = Instantiate(_gameData.GameCanvasesList.CanvasPauseSubMenuSettings);
		_canvasPauseSubMenuSettingsGameDifficulty = Instantiate(_gameData.GameCanvasesList.CanvasPauseSubMenuSettingsGameDifficulty);
		_canvasPauseMenuConfirmAction = Instantiate(_gameData.GameCanvasesList.CanvasPauseMenuConfirmAction);

		_canvasMainMenuChooseMission = Instantiate(_gameData.GameCanvasesList.CanvasMainMenuChooseMission);
		_canvasMainMenuReadNews = Instantiate(_gameData.GameCanvasesList.CanvasMainMenuReadNews);

		_canvasMenuWeaponWheel = Instantiate(_gameData.GameCanvasesList.CanvasMenuWeaponWheel);

		_canvasMenuWeaponWheel = Instantiate(_gameData.GameCanvasesList.CanvasMenuWeaponWheel);
		_canvasMenuCutscene = Instantiate(_gameData.GameCanvasesList.CanvasMenuCutscene);

		_canvasHUDhealthAndMana = Instantiate(_gameData.GameCanvasesList.CanvasHUDhealthAndMana);
		_canvasHUDammo = Instantiate(_gameData.GameCanvasesList.CanvasHUDammo);
		_canvasHUDinteraction = Instantiate(_gameData.GameCanvasesList.CanvasHUDinteraction);
		_canvasHUDmission = Instantiate(_gameData.GameCanvasesList.CanvasHUDmission);

		_canvasMenuNote = Instantiate(_gameData.GameCanvasesList.CanvasMenuNote);
		_canvasMenuLockpickElectronic = Instantiate(_gameData.GameCanvasesList.CanvasMenuLockpickElectronic);
		_canvasMenuLockpickMechanical = Instantiate(_gameData.GameCanvasesList.CanvasMenuLockpickMechanical);
		_canvasMenuDialogue = Instantiate(_gameData.GameCanvasesList.CanvasMenuDialogue);

		Debug.Log("=== CANVASES INITIALIZED ===");
		yield break;
	}

	private IEnumerator InitializeSceneSystem()
	{
		_bootstrapSubProcessSceneSystem = new BootstrapSubProcessScenesSystem(
			this, 
			_gameController,
			LocalizationManager,
			_canvasSceneLoadingScreen);

		yield return StartCoroutine(_bootstrapSubProcessSceneSystem.Initialize());

		Debug.Log("=== SCENE SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializeSaveLoadSystem()
	{
		_bootstrapSubProcessSaveLoadSystem = new BootstrapSubProcessSaveLoadSystem(
			this,
			_gameController,
			_bootstrapSubProcessSceneSystem);

		yield return StartCoroutine(_bootstrapSubProcessSaveLoadSystem.Initialize());

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
			_canvasBootstrapChooseFirstLanguage,
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
			_canvasMenuDialogue,
			_canvasMainMenuChooseMission,
			_canvasBootstrapSignTermsAndConditions);

		yield return StartCoroutine(_bootstrapSubProcessMenuSystem.Initialize());

		Debug.Log("=== MENU SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializePlayerPrefsSystem()
	{
		_bootstrapSubProcessPlayerPrefsSystem = new BootstrapSubProcessPlayerPrefsSystem(
			this,
			_inputDevice,
			_bootstrapSubProcessMenuSystem);

		yield return StartCoroutine(_bootstrapSubProcessPlayerPrefsSystem.Initialize());

		Debug.Log("=== PLAYERPREFS SYSTEM INITIALIZED ===");
	}


	private IEnumerator InitializePlayerSystems()
	{
		_gameObjectPlayer = Instantiate((GameObject)Resources.Load("1_Bootstrap/Player/Bootstrap_PlayerGameObject"));
		GameObjectPlayerCamera = Instantiate((GameObject)Resources.Load("1_Bootstrap/Player/Bootstrap_PlayerCameraGameObject"));

		_bootstrapSubProcessPlayerSystems = new BootstrapSubProcessPlayerSystems(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			_gameController,
			_inputDevice,
			_canvasMenuBackground,
			_gameObjectPlayer,
			GameObjectPlayerCamera);

		yield return StartCoroutine(_bootstrapSubProcessPlayerSystems.Initialize());

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
			LocalizationManager,
			_gameObjectPlayer,
			GameObjectPlayerCamera);

		yield return StartCoroutine(_bootstrapSubProcessInteractionSystem.Initialize());

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

		yield return StartCoroutine(_bootstrapSubProcessWeaponSystem.Initialize());

		Debug.Log("=== WEAPON SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializeMissionsSystem()
	{
		_bootstrapSubProcessMissionsSystem = new BootstrapSubProcessMissionsSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem,
			GameObjectPlayerCamera);

		yield return StartCoroutine(_bootstrapSubProcessMissionsSystem.Initialize());

		Debug.Log("=== MISSIONS SYSTEM INITIALIZED ===");
	}

	private IEnumerator InitializeObjectPoolSystem()
	{
		_bootstrapSubProcessObjectPoolSystem = new BootstrapSubProcessObjectPoolSystem(
			this,
			_bootstrapSubProcessSceneSystem,
			_bootstrapSubProcessMenuSystem);

		yield return StartCoroutine(_bootstrapSubProcessObjectPoolSystem.Initialize());

		Debug.Log("=== OBJECT POOL SYSTEM INITIALIZED ===");
	}

	private IEnumerator RegisterBootstrapDependencies()
	{
		ServiceLocator.Register("LocalizationManager", LocalizationManager);
		ServiceLocator.Register("GameController", _gameController);
		ServiceLocator.Register("InputDevice", _inputDevice);
		ServiceLocator.Register("KeyPauseMenu", _keyCodePauseMenu);
		ServiceLocator.Register("GameScenesList", GameData.GameScenesList);

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
		if (_firstSceneToLoad.FirstSceneToLoad == GameScenesEnum.Scene_0_MainMenu)
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadMainMenuScene());
		}
		else
		{
			yield return StartCoroutine(_bootstrapSubProcessSceneSystem.GameSceneManager.LoadGameplayScene(_firstSceneToLoad.FirstSceneToLoad));
		}
	}

	private IEnumerator BootstrapPrerequisites()
	{
		yield return StartCoroutine(ChooseInitialLanguage());

		yield return StartCoroutine(SignTermsAndConditions());

		_playerPrefsData.SetBootstrapPrerequisitesMet();
	}

	private IEnumerator ChooseInitialLanguage()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		_canvasBootstrapChooseFirstLanguage.SetActive(true);

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

		Destroy(_canvasBootstrapChooseFirstLanguage);
	}

	private IEnumerator SignTermsAndConditions()
	{
		_canvasBootstrapSignTermsAndConditions.SetActive(true);

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		bool termsAkcnowledged = false;
		bool termsSigned = false;

		var toggleComponent = _bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.ToggleAgreeWithTerms.GetComponent<Toggle>();
		var buttonSignComponent = _bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.ButtonSign.GetComponent<Button>();

		buttonSignComponent.interactable = false;

		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextHeaderTermsAndConditions.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedString("UI_Menu_BootstrapSignTermsnAndConditions_TextHeaderTermsAndConditions");
		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextButtonSign.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedString("UI_Menu_BootstrapSignTermsnAndConditions_ButtonSign");
		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextButtonRefuse.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedString("UI_Menu_BootstrapSignTermsnAndConditions_ButtonRefuse");
		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextToggleAgreeWithTerms.GetComponent<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedString("UI_Menu_BootstrapSignTermsnAndConditions_ToggleAcceptWithTerms");

		if (LocalizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextTermsAndConditions.GetComponent<TextMeshProUGUI>().text = GameData.TermsAndConditions.TermsAndConditions_RU.text;
		}
		else
		{
			_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.TextTermsAndConditions.GetComponent<TextMeshProUGUI>().text = GameData.TermsAndConditions.TermsAndConditions_EN.text;
		}

		toggleComponent.isOn = false;

		toggleComponent.onValueChanged.AddListener((bool isOn) =>
		{
			termsAkcnowledged = isOn;
			buttonSignComponent.interactable = isOn;
		});

		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.ButtonSign.GetComponent<Button>().onClick.AddListener(() =>
		{
			termsSigned = true;
		});

		_bootstrapSubProcessMenuSystem.ViewModelBootstrapSignTermsAndConditions.ButtonRefuse.GetComponent<Button>().onClick.AddListener(() =>
		{
			Debug.Log("EXIT GAME");
			Application.Quit();
		});

		yield return new WaitUntil(() => termsSigned);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		Destroy(_canvasBootstrapSignTermsAndConditions);
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

		if (_firstSceneToLoad.FirstSceneToLoad != GameScenesEnum.Scene_0_MainMenu)
		{
			_bootstrapSubProcessPlayerSystems.PlayerMovementController.SetPlayerPosition(_playerTransform.PlayerPosition);
			_bootstrapSubProcessPlayerSystems.PlayerMovementController.SetPlayerRotationY(_playerTransform.PlayerRotationY);
			_bootstrapSubProcessPlayerSystems.PlayerCameraController.SetCameraRotationY(_playerTransform.PlayerRotationY);
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
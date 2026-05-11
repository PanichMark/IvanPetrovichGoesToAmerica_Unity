using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubProcessMenuSystem
{
	private Bootstrap _bootstrap;

	private GameController _gameController;
	private IInputDevice _inputDevice;

	private GameSceneManager _gameSceneManager;
	private SaveLoadController _saveLoadController;

	private GameObject _gameObjectPlayerCamera;

	private GameObject _gameObjectBootstrapMenuSystem;
	public MenuManager MenuManager { get; private set; }

	private PauseMenuController _pauseMenuController;
	private GameObject _canvasPauseMenu;
	private GameObject[] _buttonsPauseMenu;

	private PauseSubMenuSaveController _pauseSubMenuSaveController;
	private GameObject _canvasPauseSubMenuSave;
	private GameObject _buttonCreateNewGameFile;
	private GameObject[] _buttonsRewriteGameFile;
	private GameObject[] _buttonsDeleteGameFile;
	private GameObject _buttonClosePauseSubMenuSave;

	private PauseSubMenuLoadController _pauseSubMenuLoadController;
	private GameObject _canvasPauseSubMenuLoad;
	private GameObject[] _buttonsLoadGameFile;
	private GameObject _buttonClosePauseSubMenuLoad;

	private PauseSubMenuAppearanceController _pauseSubMenuAppearanceController;
	private GameObject _canvasPauseSubMenuAppearance;
	private GameObject _buttonClosePauseSubMenuAppearance;

	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;
	private PauseSubMenuSettingsPlayerPrefs _pauseSubMenuSettingsPlayerPrefs;
	private GameObject _canvasPauseSubMenuSettings;
	private GameObject _sliderChangeFOV;
	private GameObject _textFOV;
	private GameObject[] _buttonsChangeFPS;
	private GameObject[] _buttonsChangeLanguage;
	private GameObject[] _inputFieldsKeyRebinds;
	private GameObject _buttonSaveGameSettings;
	private GameObject _buttonResetGameSettings;
	private GameObject _buttonClosePauseSubMenuSettings;

	private ConfirmActionMenuController _confirmActionMenuController;
	private GameObject _canvasMenuConfirmAction;
	private GameObject _buttonConfirmAction;
	private GameObject _buttonCancelAction;
	private GameObject _textConfirmActionMessage;

	private MainMenuReadNewsController _mainMenuReadNewsController;
	private GameObject _canvasMainMenuReadNews;
	private Button _buttonCloseMainMenuReadNews;

	private CutsceneMenuController _cutsceneMenuController;
	private GameObject _canvasMenuCutscene;

	public BootstrapSubProcessMenuSystem(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		SaveLoadController saveLoadController,
		GameObject playerCameraGameObject,
		GameObject canvasPauseMenu,
		GameObject canvasPauseSubMenuSave,
		GameObject canvasPauseSubMenuLoad,
		GameObject canvasPauseSubMenuAppearance,
		GameObject canvasPauseSubMenuSettings,
		GameObject canvasMenuConfirmAction,
		GameObject canvasMainMenuReadNews,
		GameObject canvasMenuCutscene)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_saveLoadController = saveLoadController;
		_gameObjectPlayerCamera = playerCameraGameObject;
		_canvasPauseMenu = canvasPauseMenu;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;
		_canvasMenuConfirmAction = canvasMenuConfirmAction;
		_canvasMainMenuReadNews = canvasMainMenuReadNews;
		_canvasMenuCutscene = canvasMenuCutscene;
	}

	public IEnumerator InitializeMenuSystems()
	{
		_gameObjectBootstrapMenuSystem = new GameObject("Bootstrap_MenuSystem");

		MenuManager = _gameObjectBootstrapMenuSystem.AddComponent<MenuManager>();
		_pauseMenuController = _gameObjectBootstrapMenuSystem.AddComponent<PauseMenuController>();
		_pauseSubMenuSaveController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSaveController>();
		_pauseSubMenuLoadController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuLoadController>();
		_pauseSubMenuAppearanceController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuAppearanceController>();
		_pauseSubMenuSettingsPlayerPrefs = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsPlayerPrefs>();
		_pauseSubMenuSettingsController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsController>();
		_confirmActionMenuController = _gameObjectBootstrapMenuSystem.AddComponent<ConfirmActionMenuController>();
		_mainMenuReadNewsController = _gameObjectBootstrapMenuSystem.AddComponent<MainMenuReadNewsController>();
		_cutsceneMenuController = _gameObjectBootstrapMenuSystem.AddComponent<CutsceneMenuController>();

		_buttonsPauseMenu = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuResume"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuSave"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuLoad"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuAppearance"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuSettings"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuExitToMainMenu")
		};

		_buttonCreateNewGameFile = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonSaveNewGameFile");
		_buttonsRewriteGameFile = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile1"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile2"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile3"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile4"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile5")
		};
		_buttonsDeleteGameFile = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile1"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile2"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile3"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile4"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile5")
		};
		_buttonClosePauseSubMenuSave = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonClosePauseSubMenuSave");

		_buttonConfirmAction = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "ButtonConfirmAction");
		_buttonCancelAction = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "ButtonCancelAction");
		_textConfirmActionMessage = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "TextConfirmationMessage");

		_buttonCloseMainMenuReadNews = _canvasMainMenuReadNews.transform.Find("ButtonCloseMainMenuReadNews").GetComponent<Button>();

		_buttonsLoadGameFile = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile1"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile2"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile3"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile4"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile5")
		};
		_buttonClosePauseSubMenuLoad = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonClosePauseSubMenuLoad");

		_buttonClosePauseSubMenuAppearance = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuAppearance, "ButtonClosePauseSubMenuAppearance");

		_sliderChangeFOV = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "SliderChangeFOV");
		_textFOV = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "TextFOV");
		_buttonsChangeFPS = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeFPS_30"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeFPS_60"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeFPS_90"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeFPS_144")
		};
		_buttonsChangeLanguage = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeLanguage_Russian"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeLanguage_English"),
		};
		_inputFieldsKeyRebinds = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "MoveForward"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "MoveBackward"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "MoveRight"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "MoveLeft"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Run"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Jump"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Crouch"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Interact"),

			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ChangeCameraView"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ChangeCameraShoulder"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "RightHandWeaponWheel"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "LeftHandWeaponWheel"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "RightHandWeaponAttack"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "LeftHandWeaponAttack"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Reload"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "LegKick"),
		};
		_buttonSaveGameSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonSaveSettings");
		_buttonResetGameSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonResetSettings");
		_buttonClosePauseSubMenuSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonClosePauseSubMenuSettings");
	
		MenuManager.Initialize(_gameController, _inputDevice, _gameSceneManager);
		_pauseMenuController.Initialize(_gameController, _inputDevice, _gameSceneManager, MenuManager, _canvasPauseMenu, _buttonsPauseMenu, _saveLoadController);
		_pauseSubMenuSaveController.Initialize(_inputDevice, _saveLoadController, MenuManager, _pauseMenuController, _canvasPauseSubMenuSave, _buttonCreateNewGameFile, _buttonsRewriteGameFile, _buttonsDeleteGameFile, _buttonClosePauseSubMenuSave);
		_pauseSubMenuLoadController.Initialize(_inputDevice, _saveLoadController, MenuManager, _pauseMenuController, _canvasPauseSubMenuLoad, _buttonsLoadGameFile, _buttonClosePauseSubMenuLoad);
		_pauseSubMenuAppearanceController.Initialize(_inputDevice, MenuManager, _pauseMenuController, _canvasPauseSubMenuAppearance, _buttonClosePauseSubMenuAppearance);
		_pauseSubMenuSettingsController.Initialize(_bootstrap, _gameController, _inputDevice, MenuManager, _pauseMenuController, _pauseSubMenuSettingsPlayerPrefs, _canvasPauseSubMenuSettings, _sliderChangeFOV, _textFOV, _buttonsChangeFPS, _buttonsChangeLanguage, _inputFieldsKeyRebinds, _buttonSaveGameSettings, _buttonResetGameSettings, _buttonClosePauseSubMenuSettings, _gameObjectPlayerCamera);
		_confirmActionMenuController.Initialize(_saveLoadController, MenuManager, _pauseMenuController, _pauseSubMenuSaveController, _pauseSubMenuLoadController, _pauseSubMenuSettingsController, _canvasMenuConfirmAction, _buttonConfirmAction, _buttonCancelAction, _textConfirmActionMessage);
		_mainMenuReadNewsController.Initialize(_inputDevice, _canvasMainMenuReadNews, _buttonCloseMainMenuReadNews);
		_cutsceneMenuController.Initialize(_gameSceneManager, MenuManager, _canvasMenuCutscene);

		ServiceLocator.Register("MenuManager", MenuManager);
		ServiceLocator.Register("PauseMenuController", _pauseMenuController);
		ServiceLocator.Register("MainMenuReadNews", _mainMenuReadNewsController);

		Debug.Log("MENU INITIALIZED");
		yield break;
	}
}

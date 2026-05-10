using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubProcessMenuSystem
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameController _gameController;
	private GameSceneManager _gameSceneManager;
	private SaveLoadController _saveLoadController;
	private GameObject _gameObjectPlayerCamera;

	// Меню
	private GameObject _gameObjectBootstrapMenuSystem;
	public MenuManager MenuManager { get; private set; }

	// Главное меню
	private MainMenuReadNewsController _mainMenuReadNewsController;
	private GameObject _canvasMainMenuReadNews;
	private Button _buttonCloseMainMenuReadNews;

	// Меню подтверждения
	private PauseMenuConfirmActionController _pauseMenuConfirmActionController;
	private GameObject _canvasMenuConfirmAction;
	private GameObject _buttonConfirmAction;
	private GameObject _buttonCancelAction;
	private GameObject _textConfirmActionMessage;

	// Меню паузы
	private PauseMenuController _pauseMenuController;
	private GameObject _canvasPauseMenu;
	private GameObject[] _buttonsPauseMenu;

	// Подменю сохранения
	private PauseSubMenuSaveController _pauseSubMenuSaveController;
	private GameObject _canvasPauseSubMenuSave;
	private GameObject[] _buttonsRewriteGameFile;
	private GameObject[] _buttonsDeleteGameFile;
	private GameObject _buttonCreateNewGameFile;
	private GameObject _buttonClosePauseSubMenuSave;

	// Подменю загрузки
	private PauseSubMenuLoadController _pauseSubMenuLoadController;
	private GameObject _canvasPauseSubMenuLoad;
	private GameObject[] _buttonsLoadGameFile;
	private GameObject _buttonClosePauseSubMenuLoad;

	// Подменю внешности
	private PauseSubMenuAppearanceController _pauseSubMenuAppearanceController;
	private GameObject _canvasPauseSubMenuAppearance;
	private GameObject _buttonClosePauseSubMenuAppearance;

	// Подменю настроек
	private PauseSubMenuSettingsController _pauseSubMenuSettingsController;
	private GameObject _canvasPauseSubMenuSettings;
	private PauseSubMenuSettingsPlayerPrefs _pauseSubMenuSettingsPlayerPrefs;
	private GameObject[] _buttonsChangeFPS;
	private GameObject _sliderChangeFOV;
	private GameObject _textFOV;
	private GameObject[] _buttonsChangeLanguage;
	private GameObject[] _inputFieldsKeyRebinds;
	private GameObject _buttonSaveGameSettings;
	private GameObject _buttonClosePauseSubMenuSettings;
	private GameObject _buttonResetGameSettings;

	// Меню катсцены
	private GameObject _canvasMenuCutscene;
	private CutsceneMenuController _cutsceneMenuController;

	public BootstrapSubProcessMenuSystem(Bootstrap bootstrap, IInputDevice inputDevice, GameController gameController,
		GameSceneManager gameSceneManager, SaveLoadController saveLoadController,
		GameObject playerCameraGameObject,
		GameObject canvasMainMenuReadNews,
		GameObject canvasPauseMenu,
		GameObject canvasMenuConfirmAction,
		GameObject canvasPauseSubMenuSave,
		GameObject canvasPauseSubMenuLoad,
		GameObject canvasPauseSubMenuAppearance,
		GameObject canvasPauseSubMenuSettings,
		GameObject canvasMenuCutscene)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_gameController = gameController;
		_gameSceneManager = gameSceneManager;
		_saveLoadController = saveLoadController;
		_gameObjectPlayerCamera = playerCameraGameObject;
		_canvasMainMenuReadNews = canvasMainMenuReadNews;
		_canvasPauseMenu = canvasPauseMenu;
		_canvasMenuConfirmAction = canvasMenuConfirmAction;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;
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
		_pauseMenuConfirmActionController = _gameObjectBootstrapMenuSystem.AddComponent<PauseMenuConfirmActionController>();
		_mainMenuReadNewsController = _gameObjectBootstrapMenuSystem.AddComponent<MainMenuReadNewsController>();
		_cutsceneMenuController = _gameObjectBootstrapMenuSystem.AddComponent<CutsceneMenuController>();

		_buttonCloseMainMenuReadNews = _canvasMainMenuReadNews.transform.Find("ButtonCloseMainMenuReadNews").GetComponent<Button>();
		_mainMenuReadNewsController.Initialize(_inputDevice, _canvasMainMenuReadNews, _buttonCloseMainMenuReadNews);

		_buttonConfirmAction = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "ButtonConfirmAction");
		_buttonCancelAction = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "ButtonCancelAction");
		_textConfirmActionMessage = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "TextConfirmationMessage");

		_buttonsPauseMenu = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuResume"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuSave"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuLoad"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuAppearance"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuSettings"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuExitToMainMenu")
		};

		_buttonsRewriteGameFile = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile1"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile2"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile3"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile4"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonRewriteGameFile5")
		};

		_buttonClosePauseSubMenuSave = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonClosePauseSubMenuSave");

		_buttonsDeleteGameFile = new[]
{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile1"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile2"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile3"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile4"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonDeleteGameFile5")
		};

		_buttonsLoadGameFile = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile1"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile2"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile3"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile4"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonLoadGameFile5")
		};

		_buttonClosePauseSubMenuLoad = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "ButtonClosePauseSubMenuLoad");

		_buttonCreateNewGameFile = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "ButtonSaveNewGameFile");
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

		_buttonClosePauseSubMenuSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonClosePauseSubMenuSettings");
		_buttonClosePauseSubMenuAppearance = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuAppearance, "ButtonClosePauseSubMenuAppearance");
		_buttonSaveGameSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonSaveSettings");
		_buttonResetGameSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonResetSettings");

		MenuManager.Initialize(_inputDevice, _gameController, _gameSceneManager);
		_pauseMenuController.Initialize(_inputDevice, _gameController, _gameSceneManager, _saveLoadController, MenuManager, _canvasPauseMenu, _buttonsPauseMenu);
		_pauseSubMenuSaveController.Initialize(_inputDevice, MenuManager, _pauseMenuController, _saveLoadController, _canvasPauseSubMenuSave, _buttonsRewriteGameFile, _buttonsDeleteGameFile, _buttonClosePauseSubMenuSave, _buttonCreateNewGameFile);
		_pauseSubMenuLoadController.Initialize(_inputDevice, MenuManager, _pauseMenuController, _saveLoadController, _canvasPauseSubMenuLoad, _buttonsLoadGameFile, _buttonClosePauseSubMenuLoad);
		_pauseSubMenuAppearanceController.Initialize(_inputDevice, MenuManager, _pauseMenuController, _canvasPauseSubMenuAppearance, _buttonClosePauseSubMenuAppearance);
		_pauseSubMenuSettingsController.Initialize(_inputDevice, _bootstrap, _gameController, _gameObjectPlayerCamera, _textFOV, MenuManager, _pauseMenuController, _canvasPauseSubMenuSettings, _buttonClosePauseSubMenuSettings, _sliderChangeFOV, _buttonsChangeFPS, _buttonsChangeLanguage, _inputFieldsKeyRebinds, _pauseSubMenuSettingsPlayerPrefs, _buttonSaveGameSettings, _buttonResetGameSettings);

		_pauseMenuConfirmActionController.Initialize(MenuManager, _pauseMenuController, _canvasMenuConfirmAction, _buttonConfirmAction, _buttonCancelAction, _saveLoadController, _pauseSubMenuSaveController, _pauseSubMenuLoadController, _pauseSubMenuSettingsController, _textConfirmActionMessage);

		_cutsceneMenuController.Initialize(MenuManager, _gameSceneManager, _canvasMenuCutscene);

		ServiceLocator.Register("MenuManager", MenuManager);
		ServiceLocator.Register("PauseMenuController", _pauseMenuController);
		ServiceLocator.Register("MainMenuReadNews", _mainMenuReadNewsController);

		Debug.Log("MENU INITIALIZED");
		yield break;
	}
}

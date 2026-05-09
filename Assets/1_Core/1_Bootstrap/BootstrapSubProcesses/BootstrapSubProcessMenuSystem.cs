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
	private GameObject _playerCameraGameObject;

	// Меню
	private GameObject _menuManagerGameobject;
	public MenuManager MenuManager { get; private set; }

	// Главное меню
	private MainMenuReadNews _mainMenuReadNews;
	private GameObject _canvasMainMenuReadNews;
	private Button _buttonCloseMainMenuReadNews;

	// Меню подтверждения
	private PauseMenuConfirmActionController _pauseMenuConfirmActionController;
	private GameObject _canvasMenuConfirmAction;
	private GameObject _buttonConfirmAction;
	private GameObject _buttonCancelAction;
	private GameObject _textConfirmationMessage;

	// Меню паузы
	private PauseMenuController _pauseMenuController;
	private GameObject _canvasPauseMenu;
	private GameObject[] _buttonsPauseMenu;

	// Подменю сохранения
	private PauseSubMenuSaveController _pauseSubMenuSaveController;
	private GameObject _canvasPauseSubMenuSave;
	private GameObject[] _buttonsRewriteGameFile;
	private GameObject[] _buttonsDeleteGameFile;
	private GameObject _buttonSaveNewGameFile;
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
	private GameObject[] _KeyRebinds;
	private GameObject _buttonSaveSettings;
	private GameObject _buttonClosePauseSubMenuSettings;
	private GameObject _buttonResetSettings;

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
		_playerCameraGameObject = playerCameraGameObject;
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
		_menuManagerGameobject = new GameObject("MenuManager");

		MenuManager = _menuManagerGameobject.AddComponent<MenuManager>();
		_mainMenuReadNews = _menuManagerGameobject.AddComponent<MainMenuReadNews>();
		_pauseMenuController = _menuManagerGameobject.AddComponent<PauseMenuController>();
		_pauseSubMenuSaveController = _menuManagerGameobject.AddComponent<PauseSubMenuSaveController>();
		_pauseSubMenuLoadController = _menuManagerGameobject.AddComponent<PauseSubMenuLoadController>();
		_pauseSubMenuAppearanceController = _menuManagerGameobject.AddComponent<PauseSubMenuAppearanceController>();
		_pauseSubMenuSettingsPlayerPrefs = _menuManagerGameobject.AddComponent<PauseSubMenuSettingsPlayerPrefs>();
		_pauseSubMenuSettingsController = _menuManagerGameobject.AddComponent<PauseSubMenuSettingsController>();
		_pauseMenuConfirmActionController = _menuManagerGameobject.AddComponent<PauseMenuConfirmActionController>();
		_cutsceneMenuController = _menuManagerGameobject.AddComponent<CutsceneMenuController>();

		_buttonCloseMainMenuReadNews = _canvasMainMenuReadNews.transform.Find("ExitReadNews").GetComponent<Button>();
		_mainMenuReadNews.Initialize(_inputDevice, _canvasMainMenuReadNews, _buttonCloseMainMenuReadNews);

		_buttonConfirmAction = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "Confirm");
		_buttonCancelAction = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "Cancel");
		_textConfirmationMessage = _bootstrap.FindDeepGameObject(_canvasMenuConfirmAction, "Text");

		_buttonsPauseMenu = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "PauseMenu Resume Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "PauseMenu Save Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "PauseMenu Load Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "PauseMenu Appearance Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "PauseMenu Settings Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "PauseMenu Exit Button")
		};

		_buttonsRewriteGameFile = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "SaveSubMenu SAVE1 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "SaveSubMenu SAVE2 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "SaveSubMenu SAVE3 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "SaveSubMenu SAVE4 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "SaveSubMenu SAVE5 Button")
		};

		_buttonClosePauseSubMenuSave = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "SaveSubMenu close Button");

		_buttonsLoadGameFile = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "LoadSubMenu LOAD1 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "LoadSubMenu LOAD2 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "LoadSubMenu LOAD3 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "LoadSubMenu LOAD4 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "LoadSubMenu LOAD5 Button")
		};

		_buttonsDeleteGameFile = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "LoadSubMenu DELETE1 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "LoadSubMenu DELETE2 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "LoadSubMenu DELETE3 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "LoadSubMenu DELETE4 Button"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "LoadSubMenu DELETE5 Button")
		};

		_buttonClosePauseSubMenuLoad = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuLoad, "LoadSubMenu close Button");

		_buttonSaveNewGameFile = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSave, "SaveNewGame");
		_sliderChangeFOV = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "CameraFOVSlider");
		_textFOV = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "CameraFOVText");
		_buttonsChangeFPS = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Fps30"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Fps60"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Fps90"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "Fps144"),
		};

		_buttonsChangeLanguage = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "LanguageRussian"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "LanguageEnglish"),
		};

		_KeyRebinds = new GameObject[]
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

		_buttonClosePauseSubMenuSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "SettingsSubMenu close Button");
		_buttonClosePauseSubMenuAppearance = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuAppearance, "AppearanceSubMenu close Button");
		_buttonSaveSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "SaveSettings");
		_buttonResetSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ResetSettings");

		MenuManager.Initialize(_inputDevice, _gameController, _gameSceneManager);
		_pauseMenuController.Initialize(_inputDevice, _gameController, _gameSceneManager, _saveLoadController, MenuManager, _canvasPauseMenu, _buttonsPauseMenu);
		_pauseSubMenuSaveController.Initialize(_inputDevice, MenuManager, _pauseMenuController, _saveLoadController, _canvasPauseSubMenuSave, _buttonsRewriteGameFile, _buttonsDeleteGameFile, _buttonClosePauseSubMenuSave, _buttonSaveNewGameFile);
		_pauseSubMenuLoadController.Initialize(_inputDevice, MenuManager, _pauseMenuController, _saveLoadController, _canvasPauseSubMenuLoad, _buttonsLoadGameFile, _buttonClosePauseSubMenuLoad);
		_pauseSubMenuAppearanceController.Initialize(_inputDevice, MenuManager, _pauseMenuController, _canvasPauseSubMenuAppearance, _buttonClosePauseSubMenuAppearance);
		_pauseSubMenuSettingsController.Initialize(_inputDevice, _bootstrap, _gameController, _playerCameraGameObject, _textFOV, MenuManager, _pauseMenuController, _canvasPauseSubMenuSettings, _buttonClosePauseSubMenuSettings, _sliderChangeFOV, _buttonsChangeFPS, _buttonsChangeLanguage, _KeyRebinds, _pauseSubMenuSettingsPlayerPrefs, _buttonSaveSettings, _buttonResetSettings);

		_pauseMenuConfirmActionController.Initialize(MenuManager, _pauseMenuController, _canvasMenuConfirmAction, _buttonConfirmAction, _buttonCancelAction, _saveLoadController, _pauseSubMenuSaveController, _pauseSubMenuLoadController, _pauseSubMenuSettingsController, _textConfirmationMessage);

		_cutsceneMenuController.Initialize(MenuManager, _gameSceneManager, _canvasMenuCutscene);

		ServiceLocator.Register("MenuManager", MenuManager);
		ServiceLocator.Register("PauseMenuController", _pauseMenuController);
		ServiceLocator.Register("MainMenuReadNews", _mainMenuReadNews);

		Debug.Log("MENU INITIALIZED");
		yield break;
	}
}

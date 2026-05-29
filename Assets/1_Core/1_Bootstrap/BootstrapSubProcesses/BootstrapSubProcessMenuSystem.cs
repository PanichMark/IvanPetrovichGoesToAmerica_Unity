using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubProcessMenuSystem
{
	private Bootstrap _bootstrap;

	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private GameSceneManager _gameSceneManager;
	private SaveLoadController _saveLoadController;

	private GameObject _gameObjectBootstrapMenuSystem;
	public MenuManager MenuManager { get; private set; }

	public PauseMenuController PauseMenuController { get; private set; }
	private GameObject _canvasMenuBackground;
	private GameObject _canvasPauseMenu;
	private GameObject[] _buttonsPauseMenu;
	private GameObject _textCurrentMissionGoal;

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

	private PauseSubMenuTutorialController _pauseSubMenuTutorialController;
	private GameObject _canvasPauseSubMenuTutorial;
	private GameObject _buttonClosePauseSubMenuTutorial;
	private GameObject _buttonNextTutorial;
	private GameObject _buttonPreviousTutorial;
	private GameObject _tutorialNoteText;
	private GameObject _tutorialNoteImage;

	public PauseSubMenuSettingsController PauseSubMenuSettingsController { get; private set; }
	private GameObject _canvasPauseSubMenuSettings;
	private GameObject _subSettingsSectionGeneral;
	private GameObject _subSettingsSectionControls;
	private GameObject _subSettingsSectionGraphics;
	private GameObject _subSettingsSectionAudio;

	private GameObject _imageBackgroundSectionGeneral;
	private GameObject _imageBackgroundSectionControls;
	private GameObject _imageBackgroundSectionGraphics;
	private GameObject _imageBackgroundSectionAudio;

	private GameObject _buttonSaveGameSettings;
	private GameObject _buttonResetGameSettings;
	private GameObject _buttonClosePauseSubMenuSettings;

	public PauseSubMenuSettingsSectionGeneralController PauseSubMenuSettingsSectionGeneralController { get; private set; }
	private GameObject _buttonSubSettingsSectionGeneral;
	private GameObject _sliderChangeFOV;
	private GameObject _numberFOV;
	private GameObject[] _buttonsChangeFPS;

	public PauseSubMenuSettingsSectionControlsController PauseSubMenuSettingsSectionControlsController {  get; private set; }
	private GameObject _buttonSubSettingsSectionControls;
	private GameObject[] _inputFieldsKeyRebinds;

	public PauseSubMenuSettingsSectionGraphicsController PauseSubMenuSettingsSectionGraphicsController { get; private set; }
	private GameObject _buttonSubSettingsSectionGraphics;

	public PauseSubMenuSettingsSectionAudioController PauseSubMenuSettingsSectionAudioController { get; private set; }
	private GameObject _buttonSubSettingsSectionAudio;
	private GameObject[] _buttonsChangeLanguage;

	private PauseSubMenuSettingsPlayerPrefs _pauseSubMenuSettingsPlayerPrefs;

	private PauseMenuConfirmActionController _pauseMenuConfirmActionController;
	private GameObject _canvasMenuConfirmAction;
	private GameObject _buttonConfirmAction;
	private GameObject _buttonCancelAction;
	private GameObject _textConfirmActionMessage;

	private MainMenuReadNewsController _mainMenuReadNewsController;
	private GameObject _canvasMainMenuReadNews;
	private Button _buttonCloseMainMenuReadNews;
	private Button _buttonYouTube;
	private Button _buttonGitHub;

	private CutsceneMenuController _cutsceneMenuController;
	private GameObject _canvasMenuCutscene;

	public HUDhealthAndManaController HUDhealthAndManaController { get; private set; }
	private GameObject _canvasHUDhealthAndMana;
	private GameObject _healthBar;
	public Slider SliderHealthBar { get; private set; }
	public Button ButtonUseHealingItem {  get; private set; }
	public TextMeshProUGUI TextHealingItemNumber { get; private set; }
	public Slider SliderManaBar {  get; private set; }
	private GameObject _manaBar;
	public Button ButtonUseManaReplenishItem {  get; private set; }
	public TextMeshProUGUI TextManaReplenishItemNumber { get; private set; }

	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;
	public TMP_Text TextPlayerMoneyNumber { get; private set; }

	public PlayerResourcesAmmoManager PlayerResourcesAmmoManager { get; private set; }
	public HUDammoController HUDammoController { get; private set; }

	private GameObject _canvasHUDammo;
	public GameObject TextRightWeaponAmmoMagazineNumber { get; private set; }
	public GameObject TextRightWeaponAmmoReserveNumber { get; private set; }
	public GameObject RightWeaponAmmoSeparator { get; private set; }
	public GameObject TextLeftWeaponAmmoMagazineNumber { get; private set; }
	public GameObject TextLeftWeaponAmmoReserveNumber { get; private set; }
	public GameObject LeftWeaponAmmoSeparator { get; private set; }

	private GameObject _canvasMenuWeaponWheel;

	public BootstrapSubProcessMenuSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameSceneManager gameSceneManager,
		SaveLoadController saveLoadController,
		GameObject canvasMenuBackground,
		GameObject canvasPauseMenu,
		GameObject canvasPauseSubMenuSave,
		GameObject canvasPauseSubMenuLoad,
		GameObject canvasPauseSubMenuTutorial,
		GameObject canvasPauseSubMenuAppearance,
		GameObject canvasPauseSubMenuSettings,
		GameObject canvasMenuConfirmAction,
		GameObject canvasMainMenuReadNews,
		GameObject canvasMenuCutscene,
		GameObject canvasMenuWeaponWheel,
		GameObject canvasHUDhealthAndMana,
		GameObject canvasHUDammo)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_gameSceneManager = gameSceneManager;
		_saveLoadController = saveLoadController;
		_canvasMenuBackground = canvasMenuBackground;
		_canvasPauseMenu = canvasPauseMenu;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		_canvasPauseSubMenuTutorial = canvasPauseSubMenuTutorial;
		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;
		_canvasMenuConfirmAction = canvasMenuConfirmAction;
		_canvasMainMenuReadNews = canvasMainMenuReadNews;
		_canvasMenuCutscene = canvasMenuCutscene;
		_canvasMenuWeaponWheel = canvasMenuWeaponWheel;
		_canvasMenuWeaponWheel = canvasMenuWeaponWheel;
		_canvasHUDhealthAndMana = canvasHUDhealthAndMana;
		_canvasHUDammo = canvasHUDammo;
	}

	public IEnumerator InitializeMenuSystem()
	{
		_gameObjectBootstrapMenuSystem = new GameObject("Bootstrap_MenuSystem");

		MenuManager = _gameObjectBootstrapMenuSystem.AddComponent<MenuManager>();
		PauseMenuController = _gameObjectBootstrapMenuSystem.AddComponent<PauseMenuController>();
		_pauseSubMenuSaveController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSaveController>();
		_pauseSubMenuLoadController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuLoadController>();
		_pauseSubMenuTutorialController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuTutorialController>();
		_pauseSubMenuAppearanceController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuAppearanceController>();
		_pauseSubMenuSettingsPlayerPrefs = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsPlayerPrefs>();
		PauseSubMenuSettingsController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsController>();
		PauseSubMenuSettingsSectionGeneralController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsSectionGeneralController>();
		PauseSubMenuSettingsSectionControlsController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsSectionControlsController>();
		PauseSubMenuSettingsSectionGraphicsController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsSectionGraphicsController>();
		PauseSubMenuSettingsSectionAudioController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsSectionAudioController>();
		_pauseMenuConfirmActionController = _gameObjectBootstrapMenuSystem.AddComponent<PauseMenuConfirmActionController>();
		_mainMenuReadNewsController = _gameObjectBootstrapMenuSystem.AddComponent<MainMenuReadNewsController>();
		_cutsceneMenuController = _gameObjectBootstrapMenuSystem.AddComponent<CutsceneMenuController>();
		HUDhealthAndManaController = _gameObjectBootstrapMenuSystem.AddComponent<HUDhealthAndManaController>();
		HUDammoController = _gameObjectBootstrapMenuSystem.AddComponent<HUDammoController>();

		_textCurrentMissionGoal = _bootstrap.FindDeepGameObject(_canvasPauseMenu, "TextCurrentMissionGoal");

		SliderHealthBar = _bootstrap.FindDeepGameObject(_canvasHUDhealthAndMana, "SliderHealthBar").GetComponent<Slider>();
		ButtonUseHealingItem = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "ButtonUseHealingItem").GetComponent<Button>();
		TextHealingItemNumber = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "TextHealingItemNumber").GetComponent<TextMeshProUGUI>();
		SliderManaBar = _bootstrap.FindDeepGameObject(_canvasHUDhealthAndMana, "SliderManaBar").GetComponent<Slider>();
		ButtonUseManaReplenishItem = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "ButtonUseManaReplenishItem").GetComponent<Button>();
		TextManaReplenishItemNumber = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "TextManaReplenishItemNumber").GetComponent<TextMeshProUGUI>();
		TextPlayerMoneyNumber = _canvasPauseMenu.transform.Find("TextPlayerMoneyNumber").GetComponent<TMP_Text>();

		TextRightWeaponAmmoMagazineNumber = _canvasHUDammo.transform.Find("TextRightWeaponAmmoMagazineNumber").gameObject;
		TextRightWeaponAmmoReserveNumber = _canvasHUDammo.transform.Find("TextRightWeaponAmmoReserveNumber").gameObject;
		RightWeaponAmmoSeparator = _canvasHUDammo.transform.Find("RightWeaponAmmoSeparator").gameObject;
		TextLeftWeaponAmmoMagazineNumber = _canvasHUDammo.transform.Find("TextLeftWeaponAmmoMagazineNumber").gameObject;
		TextLeftWeaponAmmoReserveNumber = _canvasHUDammo.transform.Find("TextLeftWeaponAmmoReserveNumber").gameObject;
		LeftWeaponAmmoSeparator = _canvasHUDammo.transform.Find("LeftWeaponAmmoSeparator").gameObject;

		_buttonsPauseMenu = new[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuResume"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuSave"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuLoad"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuAppearance"),
			_bootstrap.FindDeepGameObject(_canvasPauseMenu, "ButtonPauseMenuTutorial"),
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
		_buttonYouTube = _canvasMainMenuReadNews.transform.Find("YouTube").GetComponent<Button>();
		_buttonGitHub = _canvasMainMenuReadNews.transform.Find("GitHub").GetComponent<Button>();

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

		_buttonClosePauseSubMenuTutorial = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuTutorial, "ButtonClosePauseSubMenuTutorial");
		_buttonNextTutorial = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuTutorial, "ButtonNextTutorial");
		_buttonPreviousTutorial = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuTutorial, "ButtonPreviousTutorial");
		_tutorialNoteText = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuTutorial, "TutorialNoteText");
		_tutorialNoteImage = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuTutorial, "TutorialNoteImage");

		_subSettingsSectionGeneral = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "SubSettingsSectionGeneral");
		_buttonSubSettingsSectionGeneral = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonSubSettingsGeneral");
		_sliderChangeFOV = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "SliderChangeFOV");
		_numberFOV = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "NumberFOV");
		_buttonsChangeFPS = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeFPS_30"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeFPS_60"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeFPS_90"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeFPS_144")
		};

		_subSettingsSectionControls = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "SubSettingsSectionControls");
		_buttonSubSettingsSectionControls = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonSubSettingsControls");
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

		_subSettingsSectionGraphics = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "SubSettingsSectionGraphics");
		_buttonSubSettingsSectionGraphics = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonSubSettingsGraphics");

		_subSettingsSectionAudio = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "SubSettingsSectionAudio");
		_buttonSubSettingsSectionAudio = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonSubSettingsAudio");
		_buttonsChangeLanguage = new GameObject[]
		{
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeLanguage_Russian"),
			_bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonChangeLanguage_English"),
		};

		_imageBackgroundSectionGeneral = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "BackgroundGeneral");
		_imageBackgroundSectionControls = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "BackgroundControls");
		_imageBackgroundSectionGraphics = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "BackgroundGraphics");
		_imageBackgroundSectionAudio = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "BackgroundAudio");


		_buttonSaveGameSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonSaveSettings");
		_buttonResetGameSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonResetSettings");
		_buttonClosePauseSubMenuSettings = _bootstrap.FindDeepGameObject(_canvasPauseSubMenuSettings, "ButtonClosePauseSubMenuSettings");

		_healthBar = _bootstrap.FindDeepGameObject(_canvasHUDhealthAndMana, "HealthBar");
		SliderHealthBar = _bootstrap.FindDeepGameObject(_canvasHUDhealthAndMana, "SliderHealthBar").GetComponent<Slider>();

		_manaBar = _bootstrap.FindDeepGameObject(_canvasHUDhealthAndMana, "ManaBar");
		SliderManaBar = _bootstrap.FindDeepGameObject(_canvasHUDhealthAndMana, "SliderManaBar").GetComponent<Slider>();

		MenuManager.Initialize(_gameController, _inputDevice, _gameSceneManager, _canvasMenuBackground);
		PauseMenuController.Initialize(_gameController, _inputDevice, _gameSceneManager, MenuManager, _canvasPauseMenu, _buttonsPauseMenu, _textCurrentMissionGoal, _saveLoadController);
		_pauseSubMenuSaveController.Initialize(_inputDevice, _saveLoadController, MenuManager, PauseMenuController, _canvasPauseSubMenuSave, _buttonCreateNewGameFile, _buttonsRewriteGameFile, _buttonsDeleteGameFile, _buttonClosePauseSubMenuSave);
		_pauseSubMenuLoadController.Initialize(_inputDevice, _saveLoadController, MenuManager, PauseMenuController, _canvasPauseSubMenuLoad, _buttonsLoadGameFile, _buttonClosePauseSubMenuLoad);
		_pauseSubMenuAppearanceController.Initialize(_inputDevice, MenuManager, PauseMenuController, _canvasPauseSubMenuAppearance, _buttonClosePauseSubMenuAppearance);
		_pauseSubMenuTutorialController.Initialize(MenuManager, PauseMenuController, _canvasPauseSubMenuTutorial, _buttonClosePauseSubMenuTutorial, _buttonNextTutorial, _buttonPreviousTutorial, _tutorialNoteText, _tutorialNoteImage, _bootstrap.ConfigPauseSubMenuTutorial.Notes);
		PauseSubMenuSettingsController.Initialize(PauseMenuController, _canvasPauseSubMenuSettings, _subSettingsSectionGeneral, _imageBackgroundSectionGeneral, _buttonSubSettingsSectionGeneral, _subSettingsSectionControls, _imageBackgroundSectionControls, _buttonSubSettingsSectionControls, _subSettingsSectionGraphics, _imageBackgroundSectionGraphics, _buttonSubSettingsSectionGraphics, _subSettingsSectionAudio, _imageBackgroundSectionAudio, _buttonSubSettingsSectionAudio, _buttonSaveGameSettings, _buttonResetGameSettings, _buttonClosePauseSubMenuSettings);
		PauseSubMenuSettingsSectionGeneralController.Initialize(_gameController, PauseMenuController, _numberFOV, _sliderChangeFOV, _buttonsChangeFPS);
		PauseSubMenuSettingsSectionControlsController.Initialize(_inputDevice, PauseMenuController, _inputFieldsKeyRebinds);
		PauseSubMenuSettingsSectionGraphicsController.Initialize();
		PauseSubMenuSettingsSectionAudioController.Initialize(_bootstrap, _localizationManager, PauseMenuController, _buttonsChangeLanguage);
		_pauseSubMenuSettingsPlayerPrefs.Initialize(_bootstrap, _inputDevice, PauseSubMenuSettingsController, PauseSubMenuSettingsSectionGeneralController, PauseSubMenuSettingsSectionControlsController, PauseSubMenuSettingsSectionGraphicsController, PauseSubMenuSettingsSectionAudioController);
		_pauseMenuConfirmActionController.Initialize(_gameSceneManager, _saveLoadController, MenuManager, PauseMenuController, _pauseSubMenuSaveController, _pauseSubMenuLoadController, PauseSubMenuSettingsController, PauseSubMenuSettingsSectionGeneralController, PauseSubMenuSettingsSectionControlsController, PauseSubMenuSettingsSectionGraphicsController, PauseSubMenuSettingsSectionAudioController,_canvasMenuConfirmAction, _buttonConfirmAction, _buttonCancelAction, _textConfirmActionMessage);
		_mainMenuReadNewsController.Initialize(_inputDevice, _canvasMainMenuReadNews, _buttonCloseMainMenuReadNews, _buttonYouTube , _buttonGitHub);
		_cutsceneMenuController.Initialize(_gameSceneManager, MenuManager, _canvasMenuCutscene);
		HUDhealthAndManaController.Initialize(_gameController, _bootstrapSubProcessSceneSystem.GameSceneManager, MenuManager, _canvasHUDhealthAndMana, _healthBar, _manaBar);

		ServiceLocator.Register("MenuManager", MenuManager);
		ServiceLocator.Register("PauseMenuController", PauseMenuController);
		ServiceLocator.Register("MainMenuReadNews", _mainMenuReadNewsController);

		Debug.Log("MENU INITIALIZED");
		yield break;
	}
}
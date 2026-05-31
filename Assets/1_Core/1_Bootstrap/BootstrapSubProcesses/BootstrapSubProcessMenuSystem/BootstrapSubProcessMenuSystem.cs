using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMenuSystem
{
	private Bootstrap _bootstrap;

	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;

	public ViewModelPauseMenu ViewModelPauseMenu {  get; private set; }
	private ViewModelPauseSubMenuSave _viewModelSave;
	private ViewModelPauseSubMenuLoad _viewModelLoad;
	private ViewModelPauseSubMenuAppearance _viewModelAppearance;
	private ViewModelPauseSubMenuTutorial _viewModelTutorial;
	private ViewModelPauseSubMenuSettings _viewModelSettings;
	private ViewModelPauseMenuConfirmAction _viewModelConfirmAction;
	private ViewModelMainMenuReadNews _viewModelReadNews;
	public ViewModelMenuWeaponWheel ViewModelWeaponWheel { get; private set; }
	public ViewModelHUDHealthAndMana ViewModelHUDHealthAndMana {  get; private set; }
	public ViewModelHUDAmmo ViewModelHUDAmmo {  get; private set; }

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

	private PauseSubMenuSaveController _pauseSubMenuSaveController;
	private GameObject _canvasPauseSubMenuSave;


	private PauseSubMenuLoadController _pauseSubMenuLoadController;
	private GameObject _canvasPauseSubMenuLoad;


	private PauseSubMenuAppearanceController _pauseSubMenuAppearanceController;
	private GameObject _canvasPauseSubMenuAppearance;


	private PauseSubMenuTutorialController _pauseSubMenuTutorialController;
	private GameObject _canvasPauseSubMenuTutorial;


	public PauseSubMenuSettingsController PauseSubMenuSettingsController { get; private set; }
	private GameObject _canvasPauseSubMenuSettings;

	public PauseSubMenuSettingsSectionGeneralController PauseSubMenuSettingsSectionGeneralController { get; private set; }


	public PauseSubMenuSettingsSectionControlsController PauseSubMenuSettingsSectionControlsController {  get; private set; }


	public PauseSubMenuSettingsSectionGraphicsController PauseSubMenuSettingsSectionGraphicsController { get; private set; }


	public PauseSubMenuSettingsSectionAudioController PauseSubMenuSettingsSectionAudioController { get; private set; }


	private PauseSubMenuSettingsPlayerPrefs _pauseSubMenuSettingsPlayerPrefs;

	private PauseMenuConfirmActionController _pauseMenuConfirmActionController;
	private GameObject _canvasMenuConfirmAction;
	

	private MainMenuReadNewsController _mainMenuReadNewsController;
	private GameObject _canvasMainMenuReadNews;


	private CutsceneMenuController _cutsceneMenuController;
	private GameObject _canvasMenuCutscene;

	public HUDhealthAndManaController HUDhealthAndManaController { get; private set; }
	private GameObject _canvasHUDhealthAndMana;

	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;

	public PlayerResourcesAmmoManager PlayerResourcesAmmoManager { get; private set; }
	public HUDammoController HUDammoController { get; private set; }

	private GameObject _canvasHUDammo;

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

		ViewModelPauseMenu = new ViewModelPauseMenu();
		ViewModelPauseMenu.Initialize(_bootstrap, _canvasPauseMenu);

		_viewModelSave = new ViewModelPauseSubMenuSave();
		_viewModelSave.Initialize(_bootstrap, _canvasPauseSubMenuSave);

		_viewModelLoad = new ViewModelPauseSubMenuLoad();
		_viewModelLoad.Initialize(_bootstrap, _canvasPauseSubMenuLoad);

		_viewModelAppearance = new ViewModelPauseSubMenuAppearance();
		_viewModelAppearance.Initialize(_bootstrap, _canvasPauseSubMenuAppearance);

		_viewModelTutorial = new ViewModelPauseSubMenuTutorial();
		_viewModelTutorial.Initialize(_bootstrap, _canvasPauseSubMenuTutorial);

		_viewModelSettings = new ViewModelPauseSubMenuSettings();
		_viewModelSettings.Initialize(_bootstrap, _canvasPauseSubMenuSettings);

		_viewModelConfirmAction = new ViewModelPauseMenuConfirmAction();
		_viewModelConfirmAction.Initialize(_bootstrap, _canvasMenuConfirmAction);

		_viewModelReadNews = new ViewModelMainMenuReadNews();
		_viewModelReadNews.Initialize(_bootstrap, _canvasMainMenuReadNews);

		ViewModelHUDHealthAndMana = new ViewModelHUDHealthAndMana();
		ViewModelHUDHealthAndMana.Initialize(_bootstrap, _canvasHUDhealthAndMana);

		ViewModelHUDAmmo = new ViewModelHUDAmmo();
		ViewModelHUDAmmo.Initialize(_bootstrap, _canvasHUDammo);

		ViewModelWeaponWheel = new ViewModelMenuWeaponWheel();
		ViewModelWeaponWheel.Initialize(_bootstrap, _canvasMenuWeaponWheel);

		MenuManager.Initialize(_gameController, _inputDevice, _gameSceneManager, _canvasMenuBackground);
		PauseMenuController.Initialize(_gameController, _inputDevice, _gameSceneManager, MenuManager, _canvasPauseMenu, ViewModelPauseMenu.ButtonsPauseMenu, ViewModelPauseMenu.TextCurrentMissionGoal, _saveLoadController);
		_pauseSubMenuSaveController.Initialize(_inputDevice, _saveLoadController, MenuManager, PauseMenuController, _canvasPauseSubMenuSave, _viewModelSave.ButtonCreateNewGameFile, _viewModelSave.ButtonsRewriteGameFile, _viewModelSave.ButtonsDeleteGameFile, _viewModelSave.ButtonClosePauseSubMenuSave);
		_pauseSubMenuLoadController.Initialize(_inputDevice, _saveLoadController, MenuManager, PauseMenuController, _canvasPauseSubMenuLoad, _viewModelLoad.ButtonsLoadGameFile, _viewModelLoad.ButtonClosePauseSubMenuLoad);
		_pauseSubMenuAppearanceController.Initialize(_inputDevice, MenuManager, PauseMenuController, _canvasPauseSubMenuAppearance, _viewModelAppearance.ButtonClosePauseSubMenuAppearance);
		_pauseSubMenuTutorialController.Initialize(MenuManager, PauseMenuController, _canvasPauseSubMenuTutorial, _viewModelTutorial.ButtonClosePauseSubMenuTutorial, _viewModelTutorial.ButtonNextTutorial, _viewModelTutorial.ButtonPreviousTutorial, _viewModelTutorial.TutorialNoteText, _viewModelTutorial.TutorialNoteImage, _bootstrap.ConfigPauseSubMenuTutorial.Notes);
		PauseSubMenuSettingsController.Initialize(PauseMenuController, _canvasPauseSubMenuSettings, _viewModelSettings.SubSettingsSectionGeneral, _viewModelSettings.ImageBackgroundSectionGeneral, _viewModelSettings.ButtonSubSettingsSectionGeneral, _viewModelSettings.SubSettingsSectionControls, _viewModelSettings.ImageBackgroundSectionControls, _viewModelSettings.ButtonSubSettingsSectionControls, _viewModelSettings.SubSettingsSectionGraphics, _viewModelSettings.ImageBackgroundSectionGraphics, _viewModelSettings.ButtonSubSettingsSectionGraphics, _viewModelSettings.SubSettingsSectionAudio, _viewModelSettings.ImageBackgroundSectionAudio, _viewModelSettings.ButtonSubSettingsSectionAudio, _viewModelSettings.ButtonSaveGameSettings, _viewModelSettings.ButtonResetGameSettings, _viewModelSettings.ButtonClosePauseSubMenuSettings);
		PauseSubMenuSettingsSectionGeneralController.Initialize(_gameController, PauseMenuController, _viewModelSettings.NumberFOV, _viewModelSettings.SliderChangeFOV, _viewModelSettings.ButtonsChangeFPS);
		PauseSubMenuSettingsSectionControlsController.Initialize(_inputDevice, PauseMenuController, _viewModelSettings.InputFieldsKeyRebinds);
		PauseSubMenuSettingsSectionGraphicsController.Initialize();
		PauseSubMenuSettingsSectionAudioController.Initialize(_bootstrap, _localizationManager, PauseMenuController, _viewModelSettings.ButtonsChangeLanguage);
		_pauseSubMenuSettingsPlayerPrefs.Initialize(_bootstrap, _inputDevice, PauseSubMenuSettingsController, PauseSubMenuSettingsSectionGeneralController, PauseSubMenuSettingsSectionControlsController, PauseSubMenuSettingsSectionGraphicsController, PauseSubMenuSettingsSectionAudioController);
		_pauseMenuConfirmActionController.Initialize(_gameSceneManager, _saveLoadController, MenuManager, PauseMenuController, _pauseSubMenuSaveController, _pauseSubMenuLoadController, PauseSubMenuSettingsController, PauseSubMenuSettingsSectionGeneralController, PauseSubMenuSettingsSectionControlsController, PauseSubMenuSettingsSectionGraphicsController, PauseSubMenuSettingsSectionAudioController,_canvasMenuConfirmAction, _viewModelConfirmAction.ButtonConfirmAction, _viewModelConfirmAction.ButtonCancelAction, _viewModelConfirmAction.TextConfirmActionMessage);
		_mainMenuReadNewsController.Initialize(_inputDevice, _canvasMainMenuReadNews, _viewModelReadNews.ButtonCloseMainMenuReadNews, _viewModelReadNews.ButtonYouTube, _viewModelReadNews.ButtonGitHub);
		_cutsceneMenuController.Initialize(_gameSceneManager, MenuManager, _canvasMenuCutscene);
		HUDhealthAndManaController.Initialize(_gameController, _bootstrapSubProcessSceneSystem.GameSceneManager, MenuManager, _canvasHUDhealthAndMana, ViewModelHUDHealthAndMana.HealthBar, ViewModelHUDHealthAndMana.ManaBar);

		ServiceLocator.Register("MenuManager", MenuManager);
		ServiceLocator.Register("PauseMenuController", PauseMenuController);
		ServiceLocator.Register("MainMenuReadNews", _mainMenuReadNewsController);

		Debug.Log("MENU INITIALIZED");
		yield break;
	}
}
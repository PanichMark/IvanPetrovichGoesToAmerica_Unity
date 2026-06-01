using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMenuSystem
{
	private Bootstrap _bootstrap;

	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;
	private GameObject _canvasHUDmission;
	public ViewModelPauseMenu ViewModelPauseMenu {  get; private set; }
	private ViewModelPauseSubMenuSave _viewModelPauseSubMenuSave;
	private ViewModelPauseSubMenuLoad _viewModelPauseSubMenuLoad;
	private ViewModelPauseSubMenuAppearance _viewModelPauseSubMenuAppearance;
	private ViewModelPauseSubMenuTutorial _viewModelPauseSubMenuTutorial;
	private ViewModelPauseSubMenuSettings _viewModelPauseSubMenuSettings;
	private ViewModelPauseMenuConfirmAction _viewModelPauseMenuConfirmAction;
	private ViewModelMainMenuReadNews _viewModelMainMenuReadNews;
	public ViewModelHUDMission ViewModelHUDMission { get; private set; }
	public ViewModelMenuWeaponWheel ViewModelWeaponWheel { get; private set; }
	public ViewModelHUDHealthAndMana ViewModelHUDhealthAndMana {  get; private set; }
	public ViewModelHUDAmmo ViewModelHUDAmmo {  get; private set; }

	public ViewModelHUDInteraction ViewModelHUDInteraction { get; private set; }
	public ViewModelMenuNote ViewModelMenuNote { get; private set; }
	public ViewModelMenuLockpickMechanical ViewModelMenuLockpickMechanical { get; private set; }
	public ViewModelMenuLockpickElectronic ViewModelMenuLockpickElectronic { get; private set; }
	public ViewModelMenuDialogue ViewModelMenuDialogue { get; private set; }

	private ViewModelPauseSubMenuSettingsSectionGeneral _viewModelPauseSubMenuSettingsSectionGeneral;
	private ViewModelPauseSubMenuSettingsSectionControls _viewModelPauseSubMenuSettingsSectionControls;
	private ViewModelPauseSubMenuSettingsSectionGraphics _viewModelPauseSubMenuSettingsSectionGraphics;
	private ViewModelPauseSubMenuSettingsSectionAudio _viewModelPauseSubMenuSettingsSectionAudio;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private GameSceneManager _gameSceneManager;
	private SaveLoadController _saveLoadController;

	private GameObject _gameObjectBootstrapMenuSystem;
	public MenuManager MenuManager { get; private set; }

	public PauseMenuController PauseMenuController { get; private set; }
	public GameObject CanvasHUDinteraction {  get; private set; }
	private GameObject _canvasMenuNote;
	private GameObject _canvasMenuLockpickMechanical;
	private GameObject _canvasMenuLockpickElectronic;
	private GameObject _canvasMenuDialogue;


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

	public GameObject CanvasHUDammo {  get; private set; }

	public GameObject CanvasMenuWeaponWheel {  get; private set; }

	public BootstrapSubProcessMenuSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessSaveLoadSystem bootstrapSubProcessSaveLoadSystem,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameObject canvasMenuBackground,
		GameObject canvasPauseMenu,
		GameObject canvasPauseSubMenuSave,
		GameObject canvasPauseSubMenuLoad,
		GameObject canvasPauseSubMenuAppearance,
		GameObject canvasPauseSubMenuTutorial,
		GameObject canvasPauseSubMenuSettings,
		GameObject canvasMenuConfirmAction,
		GameObject canvasMainMenuReadNews,
		GameObject canvasMenuWeaponWheel,
		GameObject canvasMenuCutscene,
		GameObject canvasHUDhealthAndMana,
		GameObject canvasHUDammo,
		GameObject canvasHUDinteraction,
		GameObject canvasHUDmission,
		GameObject canvasMenuNote,
		GameObject canvasMenuLockpickMechanical,
		GameObject canvasMenuLockpickElectronic,
		GameObject canvasMenuDialogue)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
		_saveLoadController = bootstrapSubProcessSaveLoadSystem.SaveLoadController;
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
		CanvasMenuWeaponWheel = canvasMenuWeaponWheel;
		_canvasHUDhealthAndMana = canvasHUDhealthAndMana;
		CanvasHUDammo = canvasHUDammo;
		_canvasHUDmission = canvasHUDmission;
		CanvasHUDinteraction = canvasHUDinteraction;
		_canvasMenuLockpickMechanical = canvasMenuLockpickMechanical;
		_canvasMenuLockpickElectronic = canvasMenuLockpickElectronic;
		_canvasMenuDialogue = canvasMenuDialogue;
		_canvasMenuNote = canvasMenuNote;
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

		ViewModelPauseMenu = new ViewModelPauseMenu(_bootstrap, _canvasPauseMenu);
		_viewModelPauseSubMenuSave = new ViewModelPauseSubMenuSave(_bootstrap, _canvasPauseSubMenuSave);
		_viewModelPauseSubMenuLoad = new ViewModelPauseSubMenuLoad(_bootstrap, _canvasPauseSubMenuLoad);
		_viewModelPauseSubMenuAppearance = new ViewModelPauseSubMenuAppearance(_bootstrap, _canvasPauseSubMenuAppearance);
		_viewModelPauseSubMenuTutorial = new ViewModelPauseSubMenuTutorial(_bootstrap, _canvasPauseSubMenuTutorial);
		_viewModelPauseSubMenuSettings = new ViewModelPauseSubMenuSettings(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseMenuConfirmAction = new ViewModelPauseMenuConfirmAction(_bootstrap, _canvasMenuConfirmAction);
		_viewModelMainMenuReadNews = new ViewModelMainMenuReadNews(_bootstrap, _canvasMainMenuReadNews);
		ViewModelHUDhealthAndMana = new ViewModelHUDHealthAndMana(_bootstrap, _canvasHUDhealthAndMana);
		ViewModelHUDAmmo = new ViewModelHUDAmmo(_bootstrap, CanvasHUDammo);
		ViewModelWeaponWheel = new ViewModelMenuWeaponWheel(_bootstrap, CanvasMenuWeaponWheel);
		ViewModelHUDMission = new ViewModelHUDMission(_bootstrap, _canvasHUDmission);

		ViewModelHUDInteraction = new ViewModelHUDInteraction(_bootstrap, CanvasHUDinteraction);
		ViewModelMenuNote = new ViewModelMenuNote(_bootstrap, _canvasMenuNote);
		ViewModelMenuLockpickMechanical = new ViewModelMenuLockpickMechanical(_bootstrap, _canvasMenuLockpickMechanical);
		ViewModelMenuLockpickElectronic = new ViewModelMenuLockpickElectronic(_bootstrap, _canvasMenuLockpickElectronic);
		ViewModelMenuDialogue = new ViewModelMenuDialogue(_bootstrap, _canvasMenuDialogue);

		_viewModelPauseSubMenuSettingsSectionGeneral = new ViewModelPauseSubMenuSettingsSectionGeneral(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsSectionControls = new ViewModelPauseSubMenuSettingsSectionControls(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsSectionGraphics = new ViewModelPauseSubMenuSettingsSectionGraphics(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsSectionAudio = new ViewModelPauseSubMenuSettingsSectionAudio(_bootstrap, _canvasPauseSubMenuSettings);




		MenuManager.Initialize(_gameController, _inputDevice, _gameSceneManager, _canvasMenuBackground);
		PauseMenuController.Initialize(_gameController, _inputDevice, _gameSceneManager, MenuManager, _canvasPauseMenu, ViewModelPauseMenu.ButtonsPauseMenu, ViewModelPauseMenu.TextCurrentMissionGoal, _saveLoadController);
		_pauseSubMenuSaveController.Initialize(_inputDevice, _saveLoadController, MenuManager, PauseMenuController, _canvasPauseSubMenuSave, _viewModelPauseSubMenuSave.ButtonCreateNewGameFile, _viewModelPauseSubMenuSave.ButtonsRewriteGameFile, _viewModelPauseSubMenuSave.ButtonsDeleteGameFile, _viewModelPauseSubMenuSave.ButtonClosePauseSubMenuSave);
		_pauseSubMenuLoadController.Initialize(_inputDevice, _saveLoadController, MenuManager, PauseMenuController, _canvasPauseSubMenuLoad, _viewModelPauseSubMenuLoad.ButtonsLoadGameFile, _viewModelPauseSubMenuLoad.ButtonClosePauseSubMenuLoad);
		_pauseSubMenuAppearanceController.Initialize(_inputDevice, MenuManager, PauseMenuController, _canvasPauseSubMenuAppearance, _viewModelPauseSubMenuAppearance.ButtonClosePauseSubMenuAppearance);
		_pauseSubMenuTutorialController.Initialize(MenuManager, PauseMenuController, _canvasPauseSubMenuTutorial, _viewModelPauseSubMenuTutorial.ButtonClosePauseSubMenuTutorial, _viewModelPauseSubMenuTutorial.ButtonNextTutorial, _viewModelPauseSubMenuTutorial.ButtonPreviousTutorial, _viewModelPauseSubMenuTutorial.TutorialNoteText, _viewModelPauseSubMenuTutorial.TutorialNoteImage, _bootstrap.ConfigPauseSubMenuTutorial.Notes);
		PauseSubMenuSettingsController.Initialize(PauseMenuController, _canvasPauseSubMenuSettings, _viewModelPauseSubMenuSettings.SubSettingsSectionGeneral, _viewModelPauseSubMenuSettings.ImageBackgroundSectionGeneral, _viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGeneral, _viewModelPauseSubMenuSettings.SubSettingsSectionControls, _viewModelPauseSubMenuSettings.ImageBackgroundSectionControls, _viewModelPauseSubMenuSettings.ButtonSubSettingsSectionControls, _viewModelPauseSubMenuSettings.SubSettingsSectionGraphics, _viewModelPauseSubMenuSettings.ImageBackgroundSectionGraphics, _viewModelPauseSubMenuSettings.ButtonSubSettingsSectionGraphics, _viewModelPauseSubMenuSettings.SubSettingsSectionAudio, _viewModelPauseSubMenuSettings.ImageBackgroundSectionAudio, _viewModelPauseSubMenuSettings.ButtonSubSettingsSectionAudio, _viewModelPauseSubMenuSettings.ButtonSaveGameSettings, _viewModelPauseSubMenuSettings.ButtonResetGameSettings, _viewModelPauseSubMenuSettings.ButtonClosePauseSubMenuSettings);
		PauseSubMenuSettingsSectionGeneralController.Initialize(_gameController, PauseMenuController, _viewModelPauseSubMenuSettingsSectionGeneral.NumberFOV, _viewModelPauseSubMenuSettingsSectionGeneral.SliderChangeFOV, _viewModelPauseSubMenuSettingsSectionGeneral.ButtonsChangeFPS);
		PauseSubMenuSettingsSectionControlsController.Initialize(_inputDevice, PauseMenuController, _viewModelPauseSubMenuSettingsSectionControls.InputFieldsKeyRebinds);
		PauseSubMenuSettingsSectionGraphicsController.Initialize();
		PauseSubMenuSettingsSectionAudioController.Initialize(_bootstrap, _localizationManager, PauseMenuController, _viewModelPauseSubMenuSettingsSectionAudio.ButtonsChangeLanguage);
		_pauseSubMenuSettingsPlayerPrefs.Initialize(_bootstrap, _inputDevice, PauseSubMenuSettingsController, PauseSubMenuSettingsSectionGeneralController, PauseSubMenuSettingsSectionControlsController, PauseSubMenuSettingsSectionGraphicsController, PauseSubMenuSettingsSectionAudioController);
		_pauseMenuConfirmActionController.Initialize(_gameSceneManager, _saveLoadController, MenuManager, PauseMenuController, _pauseSubMenuSaveController, _pauseSubMenuLoadController, PauseSubMenuSettingsController, PauseSubMenuSettingsSectionGeneralController, PauseSubMenuSettingsSectionControlsController, PauseSubMenuSettingsSectionGraphicsController, PauseSubMenuSettingsSectionAudioController,_canvasMenuConfirmAction, _viewModelPauseMenuConfirmAction.ButtonConfirmAction, _viewModelPauseMenuConfirmAction.ButtonCancelAction, _viewModelPauseMenuConfirmAction.TextConfirmActionMessage);
		_mainMenuReadNewsController.Initialize(_inputDevice, _canvasMainMenuReadNews, _viewModelMainMenuReadNews.ButtonCloseMainMenuReadNews, _viewModelMainMenuReadNews.ButtonYouTube, _viewModelMainMenuReadNews.ButtonGitHub);
		_cutsceneMenuController.Initialize(_gameSceneManager, MenuManager, _canvasMenuCutscene);
		HUDhealthAndManaController.Initialize(_gameController, _bootstrapSubProcessSceneSystem.GameSceneManager, MenuManager, _canvasHUDhealthAndMana, ViewModelHUDhealthAndMana.HealthBar, ViewModelHUDhealthAndMana.ManaBar);

		ServiceLocator.Register("MenuManager", MenuManager);
		ServiceLocator.Register("PauseMenuController", PauseMenuController);
		ServiceLocator.Register("MainMenuReadNews", _mainMenuReadNewsController);
		ServiceLocator.Register("TextChokeNPC", ViewModelHUDAmmo.TextChokeNPC);

		ServiceLocator.Register("CanvasMenuNote", _canvasMenuNote);
		ServiceLocator.Register("TextNote", ViewModelMenuNote.TextNote);
		ServiceLocator.Register("ImageNote", ViewModelMenuNote.ImageNote);
		ServiceLocator.Register("ImageNoteBlackBackground", ViewModelMenuNote.ImageNoteBlackBackground);
		ServiceLocator.Register("ButtonCloseReadNoteMenu", ViewModelMenuNote.ButtonCloseMenuNote);

		ServiceLocator.Register("CanvasMenuLockpickMechanical", _canvasMenuLockpickMechanical);
		ServiceLocator.Register("ButtonCloseLockpickMechanicalMenu", ViewModelMenuLockpickMechanical.ButtonCloseMenuLockpickMechanical);

		ServiceLocator.Register("CanvasMenuLockpickElectronic", _canvasMenuLockpickElectronic);
		ServiceLocator.Register("ButtonsLockElectronic", ViewModelMenuLockpickElectronic.ButtonsLockElectronic);
		ServiceLocator.Register("ButtonCloseLockpickElectronicMenu", ViewModelMenuLockpickElectronic.ButtonCloseMenuLockpickElectronic);

		ServiceLocator.Register("CanvasMenuDialogue", _canvasMenuDialogue);
		ServiceLocator.Register("TextPhraseLine", ViewModelHUDInteraction.TextPhraseLine);
		ServiceLocator.Register("TextDialogueLine", ViewModelMenuDialogue.TextDialogueLine);
		ServiceLocator.Register("ButtonDialogueYes", ViewModelMenuDialogue.ButtonDialogueYes);
		ServiceLocator.Register("ButtonDialogueNo", ViewModelMenuDialogue.ButtonDialogueNo);
		ServiceLocator.Register("TextDialogueYes", ViewModelMenuDialogue.TextDialogueYes);
		ServiceLocator.Register("TextDialogueNo", ViewModelMenuDialogue.TextDialogueNo);

		Debug.Log("MENU INITIALIZED");
		yield break;
	}
}
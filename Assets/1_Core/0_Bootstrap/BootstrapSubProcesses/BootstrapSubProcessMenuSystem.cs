using System.Collections;
using UnityEngine;

public class BootstrapSubProcessMenuSystem
{
	private Bootstrap _bootstrap;

	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
	private GameObject _canvasHUDmission;

	private MenuBackgroundController _menuBackgroundController;
	public ViewModelPauseMenu ViewModelPauseMenu {  get; private set; }
	private ViewModelPauseSubMenuSave _viewModelPauseSubMenuSave;
	private ViewModelPauseSubMenuLoad _viewModelPauseSubMenuLoad;
	private ViewModelPauseSubMenuAppearance _viewModelPauseSubMenuAppearance;
	private ViewModelPauseSubMenuTutorial _viewModelPauseSubMenuTutorial;
	private ViewModelPauseSubMenuSettings _viewModelPauseSubMenuSettings;
	private ViewModelPauseMenuConfirmAction _viewModelPauseMenuConfirmAction;
	private ViewModelMainMenuReadNews _viewModelMainMenuReadNews;
	private HUDmissionsController _HUDmissionsController;
	private GameObject _canvasMenuChooseFirstLanguage;
	public ViewModelHUDMission ViewModelHUDMission { get; private set; }
	public ViewModelMenuWeaponWheel ViewModelWeaponWheel { get; private set; }
	public ViewModelHUDHealthAndMana ViewModelHUDhealthAndMana {  get; private set; }
	public ViewModelHUDAmmo ViewModelHUDAmmo {  get; private set; }
	public ViewModelHUDInteraction ViewModelHUDInteraction { get; private set; }
	public ViewModelMenuNote ViewModelMenuNote { get; private set; }
	public ViewModelMenuLockpickMechanical ViewModelMenuLockpickMechanical { get; private set; }
	public ViewModelMenuLockpickElectronic ViewModelMenuLockpickElectronic { get; private set; }
	public ViewModelMenuDialogue ViewModelMenuDialogue { get; private set; }

	public ViewModelMenuCutscene ViewModelMenuCutscene { get; private set; }
	public ViewModelMenuChooseFirstLanguage ViewModelMenuChooseFirstLanguage { get; private set; }

	private ViewModelPauseSubMenuSettingsSectionGeneral _viewModelPauseSubMenuSettingsSectionGeneral;
	private ViewModelPauseSubMenuSettingsGameDifficultyController _viewModelPauseSubMenuSettingsGameDifficultyController;
	private ViewModelPauseSubMenuSettingsSectionControls _viewModelPauseSubMenuSettingsSectionControls;
	private ViewModelPauseSubMenuSettingsSectionGraphics _viewModelPauseSubMenuSettingsSectionGraphics;
	public ViewModelPauseSubMenuSettingsSectionAudio ViewModelPauseSubMenuSettingsSectionAudio { get; private set; }

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private GameScenesManager _gameSceneManager;
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

	private PauseSubMenuSettingsGameDifficultyController __pauseSubMenuSettingsGameDifficultyController;
	private GameObject _canvasPauseSubMenuSettingsGameDifficultyController;


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

	public PlayerResourcesAmmoManager PlayerResourcesAmmoManager { get; private set; }
	public HUDammoController HUDammoController { get; private set; }

	public GameObject CanvasHUDammo {  get; private set; }

	public GameObject CanvasMenuWeaponWheel {  get; private set; }

	public BootstrapSubProcessMenuSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessSaveLoadSystem bootstrapSubProcessSaveLoadSystem,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameObject canvasMenuChooseFirstLanguage,
		GameObject canvasMenuBackground,
		GameObject canvasPauseMenu,
		GameObject canvasPauseSubMenuSave,
		GameObject canvasPauseSubMenuLoad,
		GameObject canvasPauseSubMenuAppearance,
		GameObject canvasPauseSubMenuTutorial,
		GameObject canvasPauseSubMenuSettings,
		GameObject canvasPauseSubMenuSettingsGameDifficultyController,
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
		_canvasMenuChooseFirstLanguage = canvasMenuChooseFirstLanguage;
		_canvasMenuBackground = canvasMenuBackground;
		_canvasPauseMenu = canvasPauseMenu;
		_canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		_canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		_canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		_canvasPauseSubMenuTutorial = canvasPauseSubMenuTutorial;
		_canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;
		_canvasPauseSubMenuSettingsGameDifficultyController = canvasPauseSubMenuSettingsGameDifficultyController;
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
		_menuBackgroundController = _gameObjectBootstrapMenuSystem.AddComponent<MenuBackgroundController>();
		PauseMenuController = _gameObjectBootstrapMenuSystem.AddComponent<PauseMenuController>();
		_pauseSubMenuSaveController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSaveController>();
		_pauseSubMenuLoadController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuLoadController>();
		_pauseSubMenuTutorialController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuTutorialController>();
		//_pauseSubMenuAppearanceController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuAppearanceController>();
		_pauseSubMenuSettingsPlayerPrefs = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsPlayerPrefs>();
		PauseSubMenuSettingsController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsController>();
		PauseSubMenuSettingsSectionGeneralController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsSectionGeneralController>();
		__pauseSubMenuSettingsGameDifficultyController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsGameDifficultyController>();
		PauseSubMenuSettingsSectionControlsController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsSectionControlsController>();
		PauseSubMenuSettingsSectionGraphicsController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsSectionGraphicsController>();
		PauseSubMenuSettingsSectionAudioController = _gameObjectBootstrapMenuSystem.AddComponent<PauseSubMenuSettingsSectionAudioController>();
		_pauseMenuConfirmActionController = _gameObjectBootstrapMenuSystem.AddComponent<PauseMenuConfirmActionController>();
		_mainMenuReadNewsController = _gameObjectBootstrapMenuSystem.AddComponent<MainMenuReadNewsController>();
		_cutsceneMenuController = _gameObjectBootstrapMenuSystem.AddComponent<CutsceneMenuController>();
		HUDhealthAndManaController = _gameObjectBootstrapMenuSystem.AddComponent<HUDhealthAndManaController>();
		HUDammoController = _gameObjectBootstrapMenuSystem.AddComponent<HUDammoController>();
		_HUDmissionsController = _gameObjectBootstrapMenuSystem.AddComponent<HUDmissionsController>();

		ViewModelMenuChooseFirstLanguage = new ViewModelMenuChooseFirstLanguage(_bootstrap, _canvasMenuChooseFirstLanguage);

		ViewModelPauseMenu = new ViewModelPauseMenu(_bootstrap, _canvasPauseMenu);
		_viewModelPauseSubMenuSave = new ViewModelPauseSubMenuSave(_bootstrap, _canvasPauseSubMenuSave);
		_viewModelPauseSubMenuLoad = new ViewModelPauseSubMenuLoad(_bootstrap, _canvasPauseSubMenuLoad);
		//_viewModelPauseSubMenuAppearance = new ViewModelPauseSubMenuAppearance(_bootstrap, _canvasPauseSubMenuAppearance);
		_viewModelPauseSubMenuTutorial = new ViewModelPauseSubMenuTutorial(_bootstrap, _canvasPauseSubMenuTutorial);
		_viewModelPauseSubMenuSettings = new ViewModelPauseSubMenuSettings(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsGameDifficultyController = new ViewModelPauseSubMenuSettingsGameDifficultyController(_bootstrap, _canvasPauseSubMenuSettingsGameDifficultyController);
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
		ViewModelMenuCutscene = new ViewModelMenuCutscene(_bootstrap, _canvasMenuCutscene);

		_viewModelPauseSubMenuSettingsSectionGeneral = new ViewModelPauseSubMenuSettingsSectionGeneral(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsSectionControls = new ViewModelPauseSubMenuSettingsSectionControls(_bootstrap, _canvasPauseSubMenuSettings);
		_viewModelPauseSubMenuSettingsSectionGraphics = new ViewModelPauseSubMenuSettingsSectionGraphics(_bootstrap, _canvasPauseSubMenuSettings);
		ViewModelPauseSubMenuSettingsSectionAudio = new ViewModelPauseSubMenuSettingsSectionAudio(_bootstrap, _canvasPauseSubMenuSettings);

		MenuManager.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_gameSceneManager);

		_menuBackgroundController.Initialize(
			MenuManager,
			_canvasMenuBackground);

		PauseMenuController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_localizationManager,
			_gameSceneManager,
			MenuManager,
			_menuBackgroundController,
			_canvasPauseMenu,
			ViewModelPauseMenu);

		_pauseSubMenuSaveController.Initialize(
			_localizationManager,
			_saveLoadController,
			PauseMenuController,
			_canvasPauseSubMenuSave,
			_viewModelPauseSubMenuSave);

		_pauseSubMenuLoadController.Initialize(
			_localizationManager,
			_saveLoadController,
			PauseMenuController,
			_canvasPauseSubMenuLoad,
			_viewModelPauseSubMenuLoad);

		/*
		_pauseSubMenuAppearanceController.Initialize(
			PauseMenuController,
			_canvasPauseSubMenuAppearance,
			_viewModelPauseSubMenuAppearance);
		*/

		_pauseSubMenuTutorialController.Initialize(
			_localizationManager,
			PauseMenuController,
			_canvasPauseSubMenuTutorial,
			_viewModelPauseSubMenuTutorial);

		PauseSubMenuSettingsController.Initialize(
			_localizationManager,
			PauseMenuController,
			_canvasPauseSubMenuSettings,
			_viewModelPauseSubMenuSettings);

		PauseSubMenuSettingsSectionGeneralController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,	
			_localizationManager,
			MenuManager,
			PauseMenuController,
			PauseSubMenuSettingsController,
			_viewModelPauseSubMenuSettingsSectionGeneral);

		__pauseSubMenuSettingsGameDifficultyController.Initialize(
			_localizationManager,
			PauseSubMenuSettingsSectionGeneralController,
			_canvasPauseSubMenuSettingsGameDifficultyController,
			_viewModelPauseSubMenuSettingsGameDifficultyController
			);

		PauseSubMenuSettingsSectionControlsController.Initialize(
			_inputDevice,
			_localizationManager,
			PauseMenuController,
			_viewModelPauseSubMenuSettingsSectionControls);

		PauseSubMenuSettingsSectionGraphicsController.Initialize(
			_localizationManager,
			_viewModelPauseSubMenuSettingsSectionGraphics);

		PauseSubMenuSettingsSectionAudioController.Initialize(
			_bootstrap,
			_localizationManager,
			PauseMenuController,
			ViewModelPauseSubMenuSettingsSectionAudio);

		_pauseSubMenuSettingsPlayerPrefs.Initialize(
			_bootstrap,
			_inputDevice,
			PauseSubMenuSettingsSectionGeneralController,
			PauseSubMenuSettingsSectionControlsController,
			PauseSubMenuSettingsSectionGraphicsController,
			PauseSubMenuSettingsSectionAudioController);

		_pauseMenuConfirmActionController.Initialize(
			_localizationManager,
			_gameSceneManager,
			_saveLoadController,
			MenuManager,
			PauseMenuController,
			_pauseSubMenuSaveController,
			_pauseSubMenuLoadController,
			PauseSubMenuSettingsController,
			PauseSubMenuSettingsSectionGeneralController,
			PauseSubMenuSettingsSectionControlsController,
			PauseSubMenuSettingsSectionGraphicsController,
			PauseSubMenuSettingsSectionAudioController,
			_canvasMenuConfirmAction,
			_viewModelPauseMenuConfirmAction);

		_mainMenuReadNewsController.Initialize(
			_canvasMainMenuReadNews,
			_viewModelMainMenuReadNews);

		_cutsceneMenuController.Initialize(
			_gameSceneManager,
			MenuManager,
			_canvasMenuCutscene);

		HUDhealthAndManaController.Initialize(
			_gameController,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			MenuManager,
			PauseSubMenuSettingsSectionGeneralController,
			_canvasHUDhealthAndMana,
			ViewModelHUDhealthAndMana);

		_HUDmissionsController.Initialize(
			_gameController,
			_gameSceneManager,
			MenuManager,
			PauseSubMenuSettingsSectionGeneralController,
			_canvasHUDmission,
			ViewModelHUDMission);

		ServiceLocator.Register("MenuManager", MenuManager);
		ServiceLocator.Register("PauseMenuController", PauseMenuController);
		ServiceLocator.Register("PauseSubMenuSettingsController", PauseSubMenuSettingsController);
		ServiceLocator.Register("MainMenuReadNews", _mainMenuReadNewsController);
		ServiceLocator.Register("TextChokeNPC", ViewModelHUDAmmo.TextChokeNPC);

		ServiceLocator.Register("CanvasMenuNote", _canvasMenuNote);
		ServiceLocator.Register("TextNote", ViewModelMenuNote.TextNote);
		ServiceLocator.Register("ImageNote", ViewModelMenuNote.ImageNote);
		ServiceLocator.Register("ImageNoteBlackBackground", ViewModelMenuNote.ImageNoteBlackBackground);
		ServiceLocator.Register("ButtonCloseReadNoteMenu", ViewModelMenuNote.ButtonCloseMenuNote);
		ServiceLocator.Register("TextButtonCloseReadNoteMenu", ViewModelMenuNote.TextButtonCloseMenuNote);

		ServiceLocator.Register("CanvasMenuLockpickMechanical", _canvasMenuLockpickMechanical);
		ServiceLocator.Register("ButtonCloseLockpickMechanicalMenu", ViewModelMenuLockpickMechanical.ButtonCloseMenuLockpickMechanical);
		ServiceLocator.Register("TextButtonCloseLockpickMechanicalMenu", ViewModelMenuLockpickMechanical.TextButtonCloseMenuLockpickMechanical);
		ServiceLocator.Register("MoveLockMechanicalUp", ViewModelMenuLockpickMechanical.ButtonMoveLockMechanismUp);
		ServiceLocator.Register("MoveLockMechanicalDown", ViewModelMenuLockpickMechanical.ButtonMoveLockMechanismDown);
		ServiceLocator.Register("MoveLockMechanicalRight", ViewModelMenuLockpickMechanical.ButtonMoveLockMechanismRight);
		ServiceLocator.Register("MoveLockMechanicalLeft", ViewModelMenuLockpickMechanical.ButtonMoveLockMechanismLeft);

		ServiceLocator.Register("CanvasMenuLockpickElectronic", _canvasMenuLockpickElectronic);
		ServiceLocator.Register("ButtonsLockElectronic", ViewModelMenuLockpickElectronic.ButtonsLockElectronic);
		ServiceLocator.Register("ButtonCloseLockpickElectronicMenu", ViewModelMenuLockpickElectronic.ButtonCloseMenuLockpickElectronic);
		ServiceLocator.Register("TextButtonCloseLockpickElectronicMenu", ViewModelMenuLockpickElectronic.TextButtonCloseMenuLockpickElectronic);

		ServiceLocator.Register("MenuBackgroundController", _menuBackgroundController);

		ServiceLocator.Register("CanvasMenuDialogue", _canvasMenuDialogue);
		ServiceLocator.Register("TextPhraseLine", ViewModelHUDInteraction.TextPhraseLine);
		ServiceLocator.Register("TextDialogueLine", ViewModelMenuDialogue.TextDialogueLine);
		ServiceLocator.Register("ButtonDialogueYes", ViewModelMenuDialogue.ButtonDialogueYes);
		ServiceLocator.Register("ButtonDialogueNo", ViewModelMenuDialogue.ButtonDialogueNo);
		ServiceLocator.Register("TextDialogueYes", ViewModelMenuDialogue.TextDialogueYes);
		ServiceLocator.Register("TextDialogueNo", ViewModelMenuDialogue.TextDialogueNo);
		ServiceLocator.Register("TextCutsceneDialogue", ViewModelMenuCutscene.TextCutsceneDialogue);

		yield break;
	}
}
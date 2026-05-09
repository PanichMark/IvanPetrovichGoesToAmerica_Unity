using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubSystemMenu
{
	private Bootstrap bootstrap;
	private IInputDevice inputDevice;
	private GameController gameController;
	private GameSceneManager gameSceneManager;
	private SaveLoadController saveLoadController;
	private GameObject playerMainCameraGameObject;

	// Меню
	private GameObject menuManagerGameobject;
	public MenuManager menuManager { get; private set; }

	// Главное меню
	private MainMenuReadNews mainMenuReadNews;
	private GameObject canvasMainMenuReadNews;
	private Button buttonCloseMainMenuReadNews;

	// Меню подтверждения
	private PauseMenuConfirmActionController menuConfirmActionController;
	private GameObject canvasMenuConfirmAction;
	private GameObject buttonConfirmAction;
	private GameObject buttonCancelAction;
	private GameObject textShowConfirmationMessage;

	// Меню паузы
	private PauseMenuController pauseMenuController;
	private GameObject canvasPauseMenu;
	private GameObject[] buttonsPauseMenu;

	// Подменю сохранения
	private PauseSubMenuSaveController pauseSubMenuSaveController;
	private GameObject canvasPauseSubMenuSave;
	private GameObject[] buttonsRewriteGame;
	private GameObject[] buttonsDeleteGame;
	private GameObject buttonSaveNewGame;
	private GameObject buttonClosePauseSubMenuSave;

	// Подменю загрузки
	private PauseSubMenuLoadController pauseSubMenuLoadController;
	private GameObject canvasPauseSubMenuLoad;
	private GameObject[] buttonsLoadGame;
	private GameObject buttonClosePauseSubMenuLoad;

	// Подменю внешности
	private PauseSubMenuAppearanceController pauseSubMenuAppearanceController;
	private GameObject canvasPauseSubMenuAppearance;
	private GameObject buttonClosePauseSubMenuAppearance;

	// Подменю настроек
	private PauseSubMenuSettingsController pauseSubMenuSettingsController;
	private GameObject canvasPauseSubMenuSettings;
	private PauseSubMenuSettingsPlayerPrefs pauseSubMenuSettingsPlayerPrefs;
	private GameObject[] FPSbuttons;
	private GameObject FOVSlider;
	private GameObject fovDisplayText;
	private GameObject[] buttonsChangeLanguage;
	private GameObject[] KeyRebinds;
	private GameObject buttonSaveSettings;
	private GameObject buttonClosePauseSubMenuSettings;
	private GameObject buttonResetSettings;

	// Меню катсцены
	private GameObject canvasCutscene;
	private CutsceneMenuController cutsceneMenuController;

	public BootstrapSubSystemMenu(Bootstrap bootstrap, IInputDevice inputDevice, GameController gameController,
		GameSceneManager gameSceneManager, SaveLoadController saveLoadController,
		GameObject playerMainCameraGameObject,
		GameObject canvasMainMenuReadNews,
		GameObject canvasPauseMenu,
		GameObject canvasMenuConfirmAction,
		GameObject canvasPauseSubMenuSave,
		GameObject canvasPauseSubMenuLoad,
		GameObject canvasPauseSubMenuAppearance,
		GameObject canvasPauseSubMenuSettings,
		GameObject canvasCutscene)
	{
		this.bootstrap = bootstrap;
		this.inputDevice = inputDevice;
		this.gameController = gameController;
		this.gameSceneManager = gameSceneManager;
		this.saveLoadController = saveLoadController;
		this.playerMainCameraGameObject = playerMainCameraGameObject;
		this.canvasMainMenuReadNews = canvasMainMenuReadNews;
		this.canvasPauseMenu = canvasPauseMenu;
		this.canvasMenuConfirmAction = canvasMenuConfirmAction;
		this.canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		this.canvasPauseSubMenuLoad = canvasPauseSubMenuLoad;
		this.canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		this.canvasPauseSubMenuSettings = canvasPauseSubMenuSettings;
		this.canvasCutscene = canvasCutscene;
	}

	public IEnumerator InitializeMenuSystems()
	{
		menuManagerGameobject = new GameObject("MenuManager");

		menuManager = menuManagerGameobject.AddComponent<MenuManager>();
		mainMenuReadNews = menuManagerGameobject.AddComponent<MainMenuReadNews>();
		pauseMenuController = menuManagerGameobject.AddComponent<PauseMenuController>();
		pauseSubMenuSaveController = menuManagerGameobject.AddComponent<PauseSubMenuSaveController>();
		pauseSubMenuLoadController = menuManagerGameobject.AddComponent<PauseSubMenuLoadController>();
		pauseSubMenuAppearanceController = menuManagerGameobject.AddComponent<PauseSubMenuAppearanceController>();
		pauseSubMenuSettingsPlayerPrefs = menuManagerGameobject.AddComponent<PauseSubMenuSettingsPlayerPrefs>();
		pauseSubMenuSettingsController = menuManagerGameobject.AddComponent<PauseSubMenuSettingsController>();
		menuConfirmActionController = menuManagerGameobject.AddComponent<PauseMenuConfirmActionController>();
		cutsceneMenuController = menuManagerGameobject.AddComponent<CutsceneMenuController>();

		buttonCloseMainMenuReadNews = canvasMainMenuReadNews.transform.Find("ExitReadNews").GetComponent<Button>();
		mainMenuReadNews.Initialize(inputDevice, canvasMainMenuReadNews, buttonCloseMainMenuReadNews);

		buttonConfirmAction = bootstrap.FindDeepChildByName(canvasMenuConfirmAction, "Confirm");
		buttonCancelAction = bootstrap.FindDeepChildByName(canvasMenuConfirmAction, "Cancel");
		textShowConfirmationMessage = bootstrap.FindDeepChildByName(canvasMenuConfirmAction, "Text");

		buttonsPauseMenu = new[]
		{
			bootstrap.FindDeepChildByName(canvasPauseMenu, "PauseMenu Resume Button"),
			bootstrap.FindDeepChildByName(canvasPauseMenu, "PauseMenu Save Button"),
			bootstrap.FindDeepChildByName(canvasPauseMenu, "PauseMenu Load Button"),
			bootstrap.FindDeepChildByName(canvasPauseMenu, "PauseMenu Appearance Button"),
			bootstrap.FindDeepChildByName(canvasPauseMenu, "PauseMenu Settings Button"),
			bootstrap.FindDeepChildByName(canvasPauseMenu, "PauseMenu Exit Button")
		};

		buttonsRewriteGame = new[]
		{
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE1 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE2 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE3 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE4 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu SAVE5 Button")
		};

		buttonClosePauseSubMenuSave = bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "SaveSubMenu close Button");

		buttonsLoadGame = new[]
		{
			bootstrap.FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD1 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD2 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD3 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD4 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu LOAD5 Button")
		};

		buttonsDeleteGame = new[]
		{
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE1 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE2 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE3 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE4 Button"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "LoadSubMenu DELETE5 Button")
		};

		buttonClosePauseSubMenuLoad = bootstrap.FindDeepChildByName(canvasPauseSubMenuLoad, "LoadSubMenu close Button");

		buttonSaveNewGame = bootstrap.FindDeepChildByName(canvasPauseSubMenuSave, "SaveNewGame");
		FOVSlider = bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "CameraFOVSlider");
		fovDisplayText = bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "CameraFOVText");
		FPSbuttons = new GameObject[]
		{
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Fps30"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Fps60"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Fps90"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Fps144"),
		};

		buttonsChangeLanguage = new GameObject[]
		{
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "LanguageRussian"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "LanguageEnglish"),
		};

		KeyRebinds = new GameObject[]
		{
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "MoveForward"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "MoveBackward"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "MoveRight"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "MoveLeft"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Run"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Jump"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Crouch"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Interact"),

			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "ChangeCameraView"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "ChangeCameraShoulder"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "RightHandWeaponWheel"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "LeftHandWeaponWheel"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "RightHandWeaponAttack"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "LeftHandWeaponAttack"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "Reload"),
			bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "LegKick"),
		};

		buttonClosePauseSubMenuSettings = bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "SettingsSubMenu close Button");
		buttonClosePauseSubMenuAppearance = bootstrap.FindDeepChildByName(canvasPauseSubMenuAppearance, "AppearanceSubMenu close Button");
		buttonSaveSettings = bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "SaveSettings");
		buttonResetSettings = bootstrap.FindDeepChildByName(canvasPauseSubMenuSettings, "ResetSettings");

		menuManager.Initialize(inputDevice, gameController, gameSceneManager);
		pauseMenuController.Initialize(inputDevice, gameController, gameSceneManager, saveLoadController, menuManager, canvasPauseMenu, buttonsPauseMenu);
		pauseSubMenuSaveController.Initialize(inputDevice, menuManager, pauseMenuController, saveLoadController, canvasPauseSubMenuSave, buttonsRewriteGame, buttonsDeleteGame, buttonClosePauseSubMenuSave, buttonSaveNewGame);
		pauseSubMenuLoadController.Initialize(inputDevice, menuManager, pauseMenuController, saveLoadController, canvasPauseSubMenuLoad, buttonsLoadGame, buttonClosePauseSubMenuLoad);
		pauseSubMenuAppearanceController.Initialize(inputDevice, menuManager, pauseMenuController, canvasPauseSubMenuAppearance, buttonClosePauseSubMenuAppearance);
		pauseSubMenuSettingsController.Initialize(inputDevice, bootstrap, gameController, playerMainCameraGameObject, fovDisplayText, menuManager, pauseMenuController, canvasPauseSubMenuSettings, buttonClosePauseSubMenuSettings, FOVSlider, FPSbuttons, buttonsChangeLanguage, KeyRebinds, pauseSubMenuSettingsPlayerPrefs, buttonSaveSettings, buttonResetSettings);

		menuConfirmActionController.Initialize(menuManager, pauseMenuController, canvasMenuConfirmAction, buttonConfirmAction, buttonCancelAction, saveLoadController, pauseSubMenuSaveController, pauseSubMenuLoadController, pauseSubMenuSettingsController, textShowConfirmationMessage);

		cutsceneMenuController.Initialize(menuManager, gameSceneManager, canvasCutscene);

		ServiceLocator.Register("MenuManager", menuManager);
		ServiceLocator.Register("PauseMenuController", pauseMenuController);
		ServiceLocator.Register("MainMenuReadNews", mainMenuReadNews);

		Debug.Log("MENU INITIALIZED");
		yield break;
	}
}

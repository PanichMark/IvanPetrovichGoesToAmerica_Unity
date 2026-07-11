using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private GameSceneManager _gameSceneManager;
	private MenuManager _menuManager;
	private MenuBackgroundController _menuBackgroundController;

	private GameObject _canvasPauseMenu;
	private GameObject[] _buttonsPauseMenu;
	private Button[] _buttonsComponentsPauseMenu;
	private TextMeshProUGUI[] _textComponentsButtonsPauseMenu;
	private TextMeshProUGUI _textComponentsCurrentMissionGoal;
	private TextMeshProUGUI _textComponentsCurrentPlayerMoney;

	public bool IsPauseConfirmMenuOpened { get; private set; }

	public delegate void OpenPauseMenuEventHandler();
	public event OpenPauseMenuEventHandler OnOpenSaveSubMenu;
	public event OpenPauseMenuEventHandler OnOpenLoadSubMenu;
	public event OpenPauseMenuEventHandler OnOpenAppearanceSubMenu;
	public event OpenPauseMenuEventHandler OnOpenTutorialSubMenu;
	public event OpenPauseMenuEventHandler OnOpenSettingsSubMenu;
	public event OpenPauseMenuEventHandler OnCloseAnyPauseSubMenu;
	public event OpenPauseMenuEventHandler OnExitToMainMenu;
	public event OpenPauseMenuEventHandler OnOpenConfirmMenu;
	public event OpenPauseMenuEventHandler OnCloseConfirmMenu;

	public void Initialize(
		Bootstrap bootstrap,	
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameSceneManager gameSceneManager,
		MenuManager menuManager,
		MenuBackgroundController menuBackgroundController,
		GameObject canvasPauseMenu,
		ViewModelPauseMenu viewModelPauseMenu)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_menuBackgroundController = menuBackgroundController;
		_gameSceneManager = gameSceneManager;
		_menuManager = menuManager;

		_canvasPauseMenu = canvasPauseMenu;

		_buttonsPauseMenu = viewModelPauseMenu.ButtonsPauseMenu;
		_buttonsComponentsPauseMenu = new Button[viewModelPauseMenu.ButtonsPauseMenu.Length];
		for (int i = 0; i < viewModelPauseMenu.ButtonsPauseMenu.Length; i++)
		{
			_buttonsComponentsPauseMenu[i] = viewModelPauseMenu.ButtonsPauseMenu[i].GetComponent<Button>();
		}
		_buttonsComponentsPauseMenu[0].onClick.AddListener(_menuManager.ClosePauseMenu);
		_buttonsComponentsPauseMenu[1].onClick.AddListener(OpenSaveSubMenu);               
		_buttonsComponentsPauseMenu[2].onClick.AddListener(OpenLoadSubMenu);               
		_buttonsComponentsPauseMenu[3].onClick.AddListener(OpenAppearanceSubMenu);
		_buttonsComponentsPauseMenu[4].onClick.AddListener(OpenTutorialSubMenu);
		_buttonsComponentsPauseMenu[5].onClick.AddListener(OpenSettingsSubMenu);
		_buttonsComponentsPauseMenu[6].onClick.AddListener(ExitToMainMenu);
		_textComponentsButtonsPauseMenu = new TextMeshProUGUI[viewModelPauseMenu.TextButtonsPauseMenu.Length];
		for (int i = 0; i < viewModelPauseMenu.TextButtonsPauseMenu.Length; i++)
		{
			_textComponentsButtonsPauseMenu[i] = viewModelPauseMenu.TextButtonsPauseMenu[i].GetComponent<TextMeshProUGUI>();
		}

		_textComponentsCurrentMissionGoal = viewModelPauseMenu.TextCurrentMissionGoal.GetComponent<TextMeshProUGUI>();
		_textComponentsCurrentPlayerMoney = viewModelPauseMenu.TextCurrentPlayerMoney.GetComponent<TextMeshProUGUI>();

		_gameController.OnSaveGameAvailable += EnableSaveButton;
		_gameController.OnSaveGameUnavailable += DisableSaveButton;
		_gameController.OnPlayerLateDeath += HideDeathPauseMenuButtons;
		_gameController.OnPlayerRevive += ShowDeathPauseMenuButtons;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_gameSceneManager.OnBeginLoadingMainMenuScene += ClosePauseSubMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += ClosePauseSubMenu;

		_menuManager.OnOpenPauseMenu += ShowPauseMenu;
		_menuManager.OnClosePauseMenu += HidePauseMenu;

		_menuManager.OnCloseConfirmationOnExitToMainMenu += ClosePauseConfirmMenu;

		Debug.Log("PauseMenuController Initialized");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_inputDevice.GetKeyPauseMenu() && _menuManager.PauseMenuLevel.Count == 2 && !_gameController.IsMainMenuOpen && !IsPauseConfirmMenuOpened)
		{
			ClosePauseSubMenu();
		}
		if (_inputDevice.GetKeyPauseMenu() && _menuManager.PauseMenuLevel.Count == 3)
		{
			ClosePauseConfirmMenu();
		}
	}

	private void EnableSaveButton()
	{
		_buttonsComponentsPauseMenu[1].interactable = true;
		_textComponentsButtonsPauseMenu[1].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonSave");
	}

	private void DisableSaveButton()
	{
		_buttonsComponentsPauseMenu[1].interactable = false;
		_textComponentsButtonsPauseMenu[1].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonSave_UNAVAILABLE");
	}

	public void OpenPauseConfirmMenu()
	{
		IsPauseConfirmMenuOpened = true;
		_menuManager.PushPauseMenuLevel();
		OnOpenConfirmMenu?.Invoke();

		Debug.Log("ConfirmMenu opened");
	}

	public void ClosePauseConfirmMenu()
	{
		IsPauseConfirmMenuOpened = false;
		OnCloseConfirmMenu?.Invoke();
		if (_menuManager.PauseMenuLevel.Count > 0)
		{
			_menuManager.PopPauseMenuLevel();
		}

		Debug.Log("ConfirmMenu closed");
	}

	private void ShowDeathPauseMenuButtons()
	{
		_buttonsPauseMenu[0].SetActive(true);
		_buttonsPauseMenu[1].SetActive(true);
		_buttonsPauseMenu[3].SetActive(true);
	}

	private void HideDeathPauseMenuButtons()
	{
		_buttonsPauseMenu[0].SetActive(false);
		_buttonsPauseMenu[1].SetActive(false);
		_buttonsPauseMenu[3].SetActive(false);
	}

	public void ClosePauseSubMenu()
	{
		OnCloseAnyPauseSubMenu?.Invoke();
		if (_menuManager.PauseMenuLevel.Count > 0)
		{
			_menuManager.PopPauseMenuLevel();
		}

		if (_gameController.IsMainMenuOpen)
		{
			_menuManager.CloseAnyMenu();
			_menuBackgroundController.HideCanvasMenuBackground();
		}

		if (!_gameController.IsMainMenuOpen)
		{
			if (_gameController.IsPauseMenuAvailable || _gameController.IsPlayerDead)
			{
				ShowPauseMenu(); 
			}
		}
	}

	public void ShowPauseMenu()
	{
		_canvasPauseMenu.gameObject.SetActive(true);
	}
	public void HidePauseMenu()
	{
		_canvasPauseMenu.gameObject.SetActive(false);
	}

	public void OpenSaveSubMenu()
	{
		OnOpenSaveSubMenu?.Invoke();
		_menuManager.PushPauseMenuLevel();
		Debug.Log("SaveSubMenu opened");
		HidePauseMenu();
	}

	public void OpenLoadSubMenu()
	{
		OnOpenLoadSubMenu?.Invoke();
		_menuManager.PushPauseMenuLevel();
		Debug.Log("LoadSubMenu opened");
		HidePauseMenu();
	}

	public void OpenAppearanceSubMenu()
	{
		OnOpenAppearanceSubMenu?.Invoke();
		_menuManager.PushPauseMenuLevel();
		Debug.Log("AppearanceSubMenu opened");
		HidePauseMenu();
	}

	public void OpenTutorialSubMenu()
	{
		OnOpenTutorialSubMenu?.Invoke();
		_menuManager.PushPauseMenuLevel();
		Debug.Log("TutorialSubMenu opened");
		HidePauseMenu();
	}

	public void OpenSettingsSubMenu()
	{
		OnOpenSettingsSubMenu?.Invoke();
		_menuManager.PushPauseMenuLevel();
		Debug.Log("SettingsSubMenu opened");
		HidePauseMenu();
	}
	
	public void ExitToMainMenu()
	{
		OnExitToMainMenu?.Invoke();
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textComponentsButtonsPauseMenu[0].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonResume");

		if (_gameController.IsGameAbleToSave)
		{
			_textComponentsButtonsPauseMenu[1].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonSave");
		}
		else
		{
			_textComponentsButtonsPauseMenu[1].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonSave_UNAVAILABLE");
		}

		_textComponentsButtonsPauseMenu[2].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonLoad");
		_textComponentsButtonsPauseMenu[3].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonAppearance");
		_textComponentsButtonsPauseMenu[4].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonTutorial");
		_textComponentsButtonsPauseMenu[5].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonSettings");
		_textComponentsButtonsPauseMenu[6].text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_ButtonExitToMainMenu");

		_textComponentsCurrentMissionGoal.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_TextCurrentMissionGoal");
		_textComponentsCurrentPlayerMoney.text = _localizationManager.GetLocalizedString("UI_Menu_PauseMenu_TextCurrentPlayerMoney");
	}
}
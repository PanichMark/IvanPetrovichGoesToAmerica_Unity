using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	public bool IsPauseConfirmMenuOpened { get; private set; }
	public event OpenPauseMenuEventHandler OnOpenConfirmMenu;
	public event OpenPauseMenuEventHandler OnCloseConfirmMenu;
	private IInputDevice _inputDevice;
    private MenuManager _menuManager;
	private GameObject _pauseMenuCanvas;
	private SaveLoadController _saveLoadController;
	private bool _isInitialized = false;
	private GameController _gameController;
	private GameObject[] _buttonsPauseMenu;
	private GameSceneManager _gameSceneManager;
	public delegate void OpenPauseMenuEventHandler();
	public event OpenPauseMenuEventHandler OnOpenSaveSubMenu;
	public event OpenPauseMenuEventHandler OnOpenLoadSubMenu;
	public event OpenPauseMenuEventHandler OnOpenAppearanceSubMenu;
	public event OpenPauseMenuEventHandler OnOpenTutorialSubMenu;
	public event OpenPauseMenuEventHandler OnOpenSettingsSubMenu;
	public event OpenPauseMenuEventHandler OnExitToMainMenu;
	public event OpenPauseMenuEventHandler OnCloseAnyPauseSubMenu;

	public void Initialize(
		GameController gameController,
		IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		MenuManager menuManager,
		GameObject PauseMenuCanvas,
		GameObject[] buttonsPauseMenu,
		SaveLoadController saveLoadController)
	{
		_gameSceneManager = gameSceneManager;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_pauseMenuCanvas = PauseMenuCanvas;
		_buttonsPauseMenu = buttonsPauseMenu;
		_saveLoadController = saveLoadController;
		_buttonsPauseMenu[0].GetComponent<Button>().onClick.AddListener(_menuManager.ClosePauseMenu);     
		_buttonsPauseMenu[1].GetComponent<Button>().onClick.AddListener(OpenSaveSubMenu);               
		_buttonsPauseMenu[2].GetComponent<Button>().onClick.AddListener(OpenLoadSubMenu);               
		_buttonsPauseMenu[3].GetComponent<Button>().onClick.AddListener(OpenAppearanceSubMenu);
		_buttonsPauseMenu[4].GetComponent<Button>().onClick.AddListener(OpenTutorialSubMenu);
		_buttonsPauseMenu[5].GetComponent<Button>().onClick.AddListener(OpenSettingsSubMenu);
		_buttonsPauseMenu[6].GetComponent<Button>().onClick.AddListener(ExitToMainMenu);

		_menuManager.OnOpenPauseMenu += ShowPauseMenu;
		_menuManager.OnClosePauseMenu += HidePauseMenu;

		_gameSceneManager.OnBeginLoadingMainMenuScene += ClosePauseSubMenu;
		_gameSceneManager.OnBeginLoadingGameplayScene += ClosePauseSubMenu;
		_gameController.OnPlayerLateDeath += HideDeathPauseMenuButtons;
		_gameController.OnPlayerRevive += ShowDeathPauseMenuButtons;

		_menuManager.OnOpenConfirmationOnExitToMainMenu += DisableButtons;
		_menuManager.OnCloseConfirmationOnExitToMainMenu += ClosePauseConfirmMenu;
		_menuManager.OnCloseConfirmationOnExitToMainMenu += EnableButtons;

		_isInitialized = true;

		Debug.Log("PauseMenu Initialized");
	}

	private void Update()
	{
		if (!_isInitialized)
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

	private void DisableButtons()
	{
		for (int i = 0; i < _buttonsPauseMenu.Length; i++)
		{
			_buttonsPauseMenu[i].GetComponent<Button>().interactable = false;
		}
	}

	private void EnableButtons()
	{
		for (int i = 0; i < _buttonsPauseMenu.Length; i++)
		{
			if (i == 3)
				continue; // SKIP! - Appearance SubMenu

			_buttonsPauseMenu[i].GetComponent<Button>().interactable = true;
		}
	}

	public void OpenPauseConfirmMenu()
	{
		IsPauseConfirmMenuOpened = true;
		_menuManager.PauseMenuLevel.Push(3);
		OnOpenConfirmMenu?.Invoke();

		Debug.Log("ConfirmMenu opened");
	}

	public void ClosePauseConfirmMenu()
	{
		IsPauseConfirmMenuOpened = false;
		OnCloseConfirmMenu?.Invoke();
		if (_menuManager.PauseMenuLevel.Count > 0)
		{ 
			_menuManager.PauseMenuLevel?.Pop();
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
			_menuManager.PauseMenuLevel.Pop();
		}

		if (_gameController.IsMainMenuOpen)
		{
			_menuManager.CloseAnyMenu();
			_menuManager.HideCanvasMenuBackground();
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
		_pauseMenuCanvas.gameObject.SetActive(true);
	}
	public void HidePauseMenu()
	{
		_pauseMenuCanvas.gameObject.SetActive(false);
	}

	public void OpenSaveSubMenu()
	{
		OnOpenSaveSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("SaveSubMenu opened");
		HidePauseMenu();
	}

	public void OpenLoadSubMenu()
	{
		OnOpenLoadSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("LoadSubMenu opened");
		HidePauseMenu();
	}

	public void OpenAppearanceSubMenu()
	{
		OnOpenAppearanceSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("AppearanceSubMenu opened");
		HidePauseMenu();
	}

	public void OpenTutorialSubMenu()
	{
		OnOpenTutorialSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("TutorialSubMenu opened");
		HidePauseMenu();
	}

	public void OpenSettingsSubMenu()
	{
		OnOpenSettingsSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("SettingsSubMenu opened");
		HidePauseMenu();
	}
	
	public void ExitToMainMenu()
	{
		OnExitToMainMenu?.Invoke();
	}
}
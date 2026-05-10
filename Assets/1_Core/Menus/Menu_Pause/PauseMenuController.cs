using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	private bool _isAnySubMenuOpened;
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
	public event OpenPauseMenuEventHandler OnClosePauseMenu;
	public event OpenPauseMenuEventHandler OnOpenSaveSubMenu;
	public event OpenPauseMenuEventHandler OnOpenLoadSubMenu;
	public event OpenPauseMenuEventHandler OnOpenImagesSubMenu;
	public event OpenPauseMenuEventHandler OnOpenSettingsSubMenu;
	public event OpenPauseMenuEventHandler OnExitToMainMenu;
	public event OpenPauseMenuEventHandler OnClosePauseSubMenu;
	public event OpenPauseMenuEventHandler OnCloseAnySubMenuForMain;

	public void Initialize( IInputDevice inputDevice, GameController gameController, GameSceneManager gameSceneManager, SaveLoadController saveLoadController, MenuManager menuManager, GameObject PauseMenuCanvas, GameObject[] buttonsPauseMenu)
	{
		_gameSceneManager = gameSceneManager;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_pauseMenuCanvas = PauseMenuCanvas;
		_buttonsPauseMenu = buttonsPauseMenu;
		_saveLoadController = saveLoadController;
		_buttonsPauseMenu[0].GetComponent<Button>().onClick.AddListener(this._menuManager.ClosePauseMenu);     
		_buttonsPauseMenu[1].GetComponent<Button>().onClick.AddListener(OpenSaveSubMenu);               
		_buttonsPauseMenu[2].GetComponent<Button>().onClick.AddListener(OpenLoadSubMenu);               
		_buttonsPauseMenu[3].GetComponent<Button>().onClick.AddListener(OpenAppearanceSubMenu);              
		_buttonsPauseMenu[4].GetComponent<Button>().onClick.AddListener(OpenSettingsSubMenu);
		_buttonsPauseMenu[5].GetComponent<Button>().onClick.AddListener(ExitToMainMenu);

		_menuManager.OnOpenPauseMenu += ShowPauseMenu;
		_menuManager.OnClosePauseMenu += HidePauseMenu;

		_isInitialized = true;
		_gameSceneManager.OnBeginLoadMainMenuScene += ClosePauseSubMenu;
		_gameSceneManager.OnBeginLoadGameplayScene += ClosePauseSubMenu;
		_gameController.OnPlayerDeath += HideDeathPauseMenuButtons;
		_gameController.OnPlayerRevive += ShowDeathPauseMenuButtons;

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
		if (_inputDevice.GetKeyPauseMenu() && _gameController.IsMainMenuOpen && _menuManager.PauseMenuLevel.Count == 2)
		{
			ClosePauseConfirmMenu();
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
		_isAnySubMenuOpened = false;
		OnClosePauseSubMenu?.Invoke();
		if (_menuManager.PauseMenuLevel.Count > 0)
		{
			_menuManager.PauseMenuLevel.Pop();
		}

		if (_gameController.IsMainMenuOpen)
		{
			_menuManager.CloseAnyMenu();
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
		_isAnySubMenuOpened = true;
		OnOpenSaveSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("SaveSubMenu opened");
		HidePauseMenu();
	}

	public void OpenLoadSubMenu()
	{
		_isAnySubMenuOpened = true;
		OnOpenLoadSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("LoadSubMenu opened");
		HidePauseMenu();
	}
	public void OpenAppearanceSubMenu()
	{
		_isAnySubMenuOpened = true;
		OnOpenImagesSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("ImagesSubMenu opened");
		HidePauseMenu();
	}
	public void OpenSettingsSubMenu()
	{
		_isAnySubMenuOpened = true;
		OnOpenSettingsSubMenu?.Invoke();
		_menuManager.PauseMenuLevel.Push(2);
		Debug.Log("SettingsSubMenu opened");
		HidePauseMenu();
	}
	
	public void ExitToMainMenu()
	{
		OnExitToMainMenu?.Invoke();
		Debug.Log("MAIN MENU EXIT");
		ClosePauseSubMenu();
		_menuManager.ClosePauseMenu();
		StartCoroutine(_gameSceneManager.LoadMainMenuScene());
	}
}
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	private bool isAnySubMenuOpened;
	public bool IsPauseConfirmMenuOpened { get; private set; }
	public event OpenPauseMenuEventHandler OnOpenConfirmMenu;
	public event OpenPauseMenuEventHandler OnCloseConfirmMenu;
	private IInputDevice inputDevice;
    private MenuManager menuManager;
	private GameObject PauseMenuCanvas;
	private SaveLoadController saveLoadController;
	private GameObject[] buttonsPauseMenu;
	private GameSceneManager gameSceneManager;
	public delegate void OpenPauseMenuEventHandler();
	public event OpenPauseMenuEventHandler OnClosePauseMenu;
	public event OpenPauseMenuEventHandler OnOpenSaveSubMenu;
	public event OpenPauseMenuEventHandler OnOpenLoadSubMenu;
	public event OpenPauseMenuEventHandler OnOpenImagesSubMenu;
	public event OpenPauseMenuEventHandler OnOpenSettingsSubMenu;
	public event OpenPauseMenuEventHandler OnPlanningToExitToMainMenu;
	public event OpenPauseMenuEventHandler OnExitToMainMenu;
	public event OpenPauseMenuEventHandler OnClosePauseSubMenu;
	public event OpenPauseMenuEventHandler OnCloseAnySubMenuForMain;
	private GameController gameController;
	public void Initialize( IInputDevice inputDevice, GameController gameController, GameSceneManager gameSceneManager, SaveLoadController saveLoadController, MenuManager menuManager, GameObject PauseMenuCanvas, GameObject[] buttonsPauseMenu)
	{
		this.gameSceneManager = gameSceneManager;
		this.gameController = gameController;
		this.inputDevice = inputDevice;
		this.menuManager = menuManager;
		this.PauseMenuCanvas = PauseMenuCanvas;
		this.buttonsPauseMenu = buttonsPauseMenu;
		this.saveLoadController = saveLoadController;
		this.buttonsPauseMenu[0].GetComponent<Button>().onClick.AddListener(this.menuManager.ClosePauseMenu);     
		this.buttonsPauseMenu[1].GetComponent<Button>().onClick.AddListener(OpenSaveSubMenu);               
		this.buttonsPauseMenu[2].GetComponent<Button>().onClick.AddListener(OpenLoadSubMenu);               
		this.buttonsPauseMenu[3].GetComponent<Button>().onClick.AddListener(OpenAppearanceSubMenu);              
		this.buttonsPauseMenu[4].GetComponent<Button>().onClick.AddListener(OpenSettingsSubMenu);         
		this.buttonsPauseMenu[5].GetComponent<Button>().onClick.AddListener(() => OnPlanningToExitToMainMenu());

		this.menuManager.OnOpenPauseMenu += ShowPauseMenu;
		this.menuManager.OnClosePauseMenu += HidePauseMenu;

		_isInitialized = true;
		this.gameSceneManager.OnBeginLoadMainMenuScene += ClosePauseSubMenu;
		this.gameSceneManager.OnBeginLoadGameplayScene += ClosePauseSubMenu;
		this.gameController.OnPlayerDeath += HideDeathPauseMenuButtons;
		this.gameController.OnPlayerRevive += ShowDeathPauseMenuButtons;
		Debug.Log("PauseMenu Initialized");
	}
	private bool _isInitialized = false;

	private void Update()
	{
		//Debug.Log(gameController.IsPlayerDead);
		//Debug.Log(gameController.IsPauseMenuAvailable);

		if (!_isInitialized)
			return;

		// Проверка условия перехода назад по меню
		if (inputDevice.GetKeyPauseMenu() && menuManager.PauseMenuLevel.Count == 2 && !gameController.IsMainMenuOpen && !IsPauseConfirmMenuOpened)
		{
			ClosePauseSubMenu();
		}
		if (inputDevice.GetKeyPauseMenu() && menuManager.PauseMenuLevel.Count == 3)
		{
			//Debug.Log("Main!");
			ClosePauseConfirmMenu();
		}
	
		if (inputDevice.GetKeyPauseMenu() && menuManager.PauseMenuLevel.Count == 2 && !isAnySubMenuOpened)
		{
			ClosePauseConfirmMenu();
		}

	}
	public void OpenPauseConfirmMenu()
	{
		IsPauseConfirmMenuOpened = true;
		menuManager.PauseMenuLevel.Push(3); // Теперь уровень 3
		OnOpenConfirmMenu?.Invoke();
		//OpenAnyMenu();
		//gameController.MakePlayerNonControllable();
		//Time.timeScale = 0f;
		Debug.Log("ConfirmMenu opened");
	}

	public void ClosePauseConfirmMenu()
	{
		IsPauseConfirmMenuOpened = false;
		menuManager.PauseMenuLevel.Pop(); // Уменьшаем с 3 до 2
		OnCloseConfirmMenu?.Invoke();

		//Debug.Log("is FALSE");
		Debug.Log("ConfirmMenu closed");
	}
	private void ShowDeathPauseMenuButtons()
	{
		buttonsPauseMenu[0].SetActive(true);
		buttonsPauseMenu[1].SetActive(true);
		buttonsPauseMenu[3].SetActive(true);
	}

	private void HideDeathPauseMenuButtons()
	{
		buttonsPauseMenu[0].SetActive(false);
		buttonsPauseMenu[1].SetActive(false);
		buttonsPauseMenu[3].SetActive(false);
	}

	public void ClosePauseSubMenu()
	{
		isAnySubMenuOpened = false;
		OnClosePauseSubMenu?.Invoke();
		if (menuManager.PauseMenuLevel.Count > 0)
		{
			menuManager.PauseMenuLevel.Pop();
			//Debug.Log("LMAO!");
		}

		if (gameController.IsMainMenuOpen)
		{
			menuManager.CloseAnyMenu();
		}

		if (!gameController.IsMainMenuOpen)
		{
			if (gameController.IsPauseMenuAvailable || gameController.IsPlayerDead)
			{
				//Debug.Log("BRUH!!!");
				ShowPauseMenu(); // Показываем главное меню паузы снова
				
			}
		}
	}

	public void ShowPauseMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(true);
	}
	public void HidePauseMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(false);
	}
	
	

	public void OpenSaveSubMenu()
	{
		isAnySubMenuOpened = true;
		OnOpenSaveSubMenu?.Invoke();
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("SaveSubMenu opened");
		HidePauseMenu();
	}

	public void OpenLoadSubMenu()
	{
		isAnySubMenuOpened = true;
		OnOpenLoadSubMenu?.Invoke();
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("LoadSubMenu opened");
		HidePauseMenu();
	}
	public void OpenAppearanceSubMenu()
	{
		isAnySubMenuOpened = true;
		OnOpenImagesSubMenu?.Invoke();
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("ImagesSubMenu opened");
		HidePauseMenu();
	}
	public void OpenSettingsSubMenu()
	{
		isAnySubMenuOpened = true;
		OnOpenSettingsSubMenu?.Invoke();
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("SettingsSubMenu opened");
		HidePauseMenu();
	}
	
	public void ExitToMainMenu()
	{
		OnExitToMainMenu?.Invoke();
		Debug.Log("MAIN MENU EXIT");
		ClosePauseSubMenu();
		menuManager.ClosePauseMenu();
		StartCoroutine(gameSceneManager.LoadMainMenuScene());
	}
}
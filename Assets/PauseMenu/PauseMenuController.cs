using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private GameObject PauseMenuCanvas;
    private MenuManager menuManager;
	private GameObject ButtonsImagesSubMenu;
	public delegate void OpenPauseSubMenuEventHandler();
	public event OpenPauseSubMenuEventHandler OnOpenSaveSubMenu;
	public event OpenPauseSubMenuEventHandler OnOpenLoadSubMenu;
	public event OpenPauseSubMenuEventHandler OnOpenImagesSubMenu;
	public event OpenPauseSubMenuEventHandler OnOpenSettingsSubMenu;
	public event OpenPauseSubMenuEventHandler OnCloseSubMenu;
	public void Initialize( IInputDevice inputDevice, MenuManager menuManager, GameObject PauseMenuCanvas, GameObject buttonImages)
	{
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.PauseMenuCanvas = PauseMenuCanvas;
		this.ButtonsImagesSubMenu = buttonImages;
		
		menuManager.OnOpenPauseMenu += ShowPauseMenu;
		menuManager.OnClosePauseMenu += HidePauseMenu;
		Button imagesSubMenuButton = ButtonsImagesSubMenu.GetComponent<Button>();
		imagesSubMenuButton.onClick.AddListener(OpenImagesSubMenu);

		/*
		ResumeGameButton.onClick.AddListener(HidePauseMenu);
		OpenSaveSubMenuButton.onClick.AddListener(OpenSaveSubMenu);
		OpenLoadSubMenuButton.onClick.AddListener(OpenLoadSubMenu);
		OpenSettingsSubMenuButton.onClick.AddListener(OpenSettingsSubMenu);
		ExitToMainMenuButton.onClick.AddListener(ExitToMainMenu);
		*/
		_isInitialized = true;
		Debug.Log("PauseMenuController Initialized");
	}
	private bool _isInitialized = false;
	public void OpenImagesSubMenu()
	{
		OnOpenImagesSubMenu?.Invoke();
		menuManager.menuLevelStack.Push(2);
		Debug.Log("ImagesSubMenu opened");
		HidePauseMenu();
	}
	private void Update()
	{
		if (!_isInitialized)
			return;

		// Проверка условия перехода назад по меню
		if (inputDevice.GetKeyPauseMenu() && menuManager.menuLevelStack.Count == 2)
		{
			OnCloseSubMenu?.Invoke();
			menuManager.menuLevelStack.Pop(); // Убираем верхний элемент (субменю)
			ShowPauseMenu(); // Показываем главное меню паузы снова
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
		OnOpenSaveSubMenu?.Invoke();
		menuManager.menuLevelStack.Push(2);
		Debug.Log("SaveSubMenu opened");
		HidePauseMenu();
	}

	public void OpenLoadSubMenu()
	{
		OnOpenLoadSubMenu?.Invoke();
		menuManager.menuLevelStack.Push(2);
		Debug.Log("LoadSubMenu opened");
		HidePauseMenu();
	}

	public void OpenSettingsSubMenu()
	{
		OnOpenSettingsSubMenu?.Invoke();
		menuManager.menuLevelStack.Push(2);
		Debug.Log("SettingsSubMenu opened");
		HidePauseMenu();
	}
	
	public void ExitToMainMenu()
	{
		Debug.Log("MAIN MENU EXIT");
	}
}
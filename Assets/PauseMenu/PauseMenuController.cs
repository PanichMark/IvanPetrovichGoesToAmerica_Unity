using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	private IInputDevice inputDevice;
    private MenuManager menuManager;
	private GameObject PauseMenuCanvas;

	private GameObject buttonClosePauseMenuGameObject;
	private GameObject buttonOpenPauseSubMenuLoadGameObject;
	private GameObject buttonOpenPauseSubMenuImagesGameObject;
	private GameObject buttonOpenPauseSubMenuSaveGameObject;
	private GameObject buttonOpenPauseSubMenuSettingsGameObject;
	private GameObject buttonExitToMainMenuGameObject;

	public delegate void OpenPauseMenuEventHandler();
	public event OpenPauseMenuEventHandler OnClosePauseMenu;
	public event OpenPauseMenuEventHandler OnOpenSaveSubMenu;
	public event OpenPauseMenuEventHandler OnOpenLoadSubMenu;
	public event OpenPauseMenuEventHandler OnOpenImagesSubMenu;
	public event OpenPauseMenuEventHandler OnOpenSettingsSubMenu;
	public event OpenPauseMenuEventHandler OnExitToMainMenu;
	public event OpenPauseMenuEventHandler OnCloseSubMenu;
	public void Initialize( IInputDevice inputDevice, MenuManager menuManager, GameObject PauseMenuCanvas,
		GameObject buttonClosePauseMenu, GameObject buttonOpenPauseSubMenuSave,
		GameObject buttonOpenPauseSubMenuLoad, GameObject buttonOpenPauseSubMenuImages,
		GameObject buttonOpenPauseSubMenuSettings, GameObject buttonExitToMainMenu)
	{
		this.inputDevice = inputDevice;
		this.menuManager = menuManager;
		this.PauseMenuCanvas = PauseMenuCanvas;

		this.buttonClosePauseMenuGameObject = buttonClosePauseMenu;
		this.buttonOpenPauseSubMenuSaveGameObject = buttonOpenPauseSubMenuSave;
		this.buttonOpenPauseSubMenuLoadGameObject = buttonOpenPauseSubMenuLoad;
		this.buttonOpenPauseSubMenuImagesGameObject = buttonOpenPauseSubMenuImages;
		this.buttonOpenPauseSubMenuSettingsGameObject = buttonOpenPauseSubMenuSettings;
		this.buttonExitToMainMenuGameObject = buttonExitToMainMenu;



		Button buttonClosePauseMenuComponent = this.buttonClosePauseMenuGameObject.GetComponent<Button>();
		buttonClosePauseMenuComponent.onClick.AddListener(menuManager.ClosePauseMenu);

		Button buttonOpenPauseSubMenuSaveComponent = this.buttonOpenPauseSubMenuSaveGameObject.GetComponent<Button>();
		buttonOpenPauseSubMenuSaveComponent.onClick.AddListener(OpenSaveSubMenu);
		
		Button buttonOpenPauseSubMenuLoadComponent = this.buttonOpenPauseSubMenuLoadGameObject.GetComponent<Button>();
		buttonOpenPauseSubMenuLoadComponent.onClick.AddListener(OpenLoadSubMenu);

		Button buttonOpenPauseSubMenuImagesComponent = this.buttonOpenPauseSubMenuImagesGameObject.GetComponent<Button>();
		buttonOpenPauseSubMenuImagesComponent.onClick.AddListener(OpenImagesSubMenu);

		Button buttonOpenPauseSubMenuSettingsComponent = this.buttonOpenPauseSubMenuSettingsGameObject.GetComponent<Button>();
		buttonOpenPauseSubMenuSettingsComponent.onClick.AddListener(OpenSettingsSubMenu);

		Button buttonExitToMainMenuComponent = this.buttonExitToMainMenuGameObject.GetComponent<Button>();
		buttonExitToMainMenuComponent.onClick.AddListener(ExitToMainMenu);

		menuManager.OnOpenPauseMenu += ShowPauseMenu;
		menuManager.OnClosePauseMenu += HidePauseMenu;

		_isInitialized = true;

		Debug.Log("PauseMenuController Initialized");
	}
	private bool _isInitialized = false;

	private void Update()
	{
		if (!_isInitialized)
			return;

		// Проверка условия перехода назад по меню
		if (inputDevice.GetKeyPauseMenu() && menuManager.PauseMenuLevel.Count == 2)
		{
			OnCloseSubMenu?.Invoke();
			menuManager.PauseMenuLevel.Pop(); // Убираем верхний элемент (субменю)
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
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("SaveSubMenu opened");
		HidePauseMenu();
	}

	public void OpenLoadSubMenu()
	{
		OnOpenLoadSubMenu?.Invoke();
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("LoadSubMenu opened");
		HidePauseMenu();
	}
	public void OpenImagesSubMenu()
	{
		OnOpenImagesSubMenu?.Invoke();
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("ImagesSubMenu opened");
		HidePauseMenu();
	}
	public void OpenSettingsSubMenu()
	{
		OnOpenSettingsSubMenu?.Invoke();
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("SettingsSubMenu opened");
		HidePauseMenu();
	}
	
	public void ExitToMainMenu()
	{
		Debug.Log("MAIN MENU EXIT");
	}
}
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private GameObject PauseMenuCanvas;
    private MenuManager menuManager;
	private GameObject buttonOpenPauseSubMenuImagesGameObject;
	private GameObject buttonOpenPauseSubMenuSettingsGameObject;
	private GameObject buttonOpenPauseSubMenuLoadGameObject;
	private GameObject buttonOpenPauseSubMenuSaveGameObject;
	public delegate void OpenPauseSubMenuEventHandler();
	public event OpenPauseSubMenuEventHandler OnOpenSaveSubMenu;
	public event OpenPauseSubMenuEventHandler OnOpenLoadSubMenu;
	public event OpenPauseSubMenuEventHandler OnOpenImagesSubMenu;
	public event OpenPauseSubMenuEventHandler OnOpenSettingsSubMenu;
	public event OpenPauseSubMenuEventHandler OnCloseSubMenu;
	public void Initialize( IInputDevice inputDevice, MenuManager menuManager, GameObject PauseMenuCanvas, GameObject buttonOpenPauseSubMenuImages)
	{
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.PauseMenuCanvas = PauseMenuCanvas;
		this.buttonOpenPauseSubMenuImagesGameObject = buttonOpenPauseSubMenuImages;
		
		menuManager.OnOpenPauseMenu += ShowPauseMenu;
		menuManager.OnClosePauseMenu += HidePauseMenu;
		Button buttonOpenPauseSubMenuImagesComponent = this.buttonOpenPauseSubMenuImagesGameObject.GetComponent<Button>();
		buttonOpenPauseSubMenuImagesComponent.onClick.AddListener(OpenImagesSubMenu);

		/*
		Button buttonOpenPauseSubMenuSettingsComponent = this.buttonOpenPauseSubMenuSettingsGameObject.GetComponent<Button>();
		buttonOpenPauseSubMenuSettingsComponent.onClick.AddListener(OpenSettingsSubMenu);

		Button buttonOpenPauseSubMenuSaveComponent = this.buttonOpenPauseSubMenuSaveGameObject.GetComponent<Button>();
		buttonOpenPauseSubMenuSaveComponent.onClick.AddListener(OpenSaveSubMenu);

		Button buttonOpenPauseSubMenuLoadComponent = this.buttonOpenPauseSubMenuLoadGameObject.GetComponent<Button>();
		buttonOpenPauseSubMenuLoadComponent.onClick.AddListener(OpenLoadSubMenu);
		*/
		_isInitialized = true;
		Debug.Log("PauseMenuController Initialized");
	}
	private bool _isInitialized = false;
	public void OpenImagesSubMenu()
	{
		OnOpenImagesSubMenu?.Invoke();
		menuManager.PauseMenuLevel.Push(2);
		Debug.Log("ImagesSubMenu opened");
		HidePauseMenu();
	}
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
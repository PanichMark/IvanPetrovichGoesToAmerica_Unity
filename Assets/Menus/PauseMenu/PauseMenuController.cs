using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private GameObject PauseMenuCanvas;
    private MenuManager menuManager;
	private GameObject ButtonsImagesSubMenu;
	public delegate void OpenPauseSubMenuEventHandler();
	public event OpenPauseSubMenuEventHandler OnOpenPauseSubMenu;
	public event OpenPauseSubMenuEventHandler OnClosePauseSubMenu;
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
		Debug.Log("PauseMenuController Initialized");
	}

	public void OpenImagesSubMenu()
	{
		//PauseMenuCanvas.gameObject.SetActive(false);
		//Debug.Log("PauseMenu closed");
		OnOpenPauseSubMenu.Invoke();
		menuManager.menuLevelStack.Push(2); // Субменю открыто поверх главного меню
								//imagesSubMenuController.ImagesSubMenuCanvas.gameObject.SetActive(true);
		Debug.Log("ImagesSubMenu opened");
		HidePauseMenu();
	}

	// Функция, вызываемая при событии "открыть меню паузы"
	public void ShowPauseMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(true); // Покажем меню паузы
	}
	public void HidePauseMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(false); // Скрываем меню паузы
		
		
		
	}
	
	public void OpenSaveSubMenu()
	{
		//PauseMenuCanvas.gameObject.SetActive(false);
	//	Debug.Log("PauseMenu closed");

		//saveSubMenuController.SaveSubMenuCanvas.gameObject.SetActive(true);
		Debug.Log("SaveSubMenu opened");
	}

	public void OpenLoadSubMenu()
	{
		//PauseMenuCanvas.gameObject.SetActive(false);
		//Debug.Log("PauseMenu closed");

		//loadSubMenuController.LoadSubMenuCanvas.gameObject.SetActive(true);
		//loadSubMenuController.RefreshLoadButtonLabels(); // Всегда обновляем информацию при открытии меню
		Debug.Log("LoadSubMenu opened");
	}

	
	public void OpenSettingsSubMenu()
	{
		//PauseMenuCanvas.gameObject.SetActive(false);
		//Debug.Log("PauseMenu closed");

		//settingsSubMenuController.SettingsSubMenuCanvas.gameObject.SetActive(true);
		Debug.Log("SettingsSubMenu opened");
	}
	
	public void ExitToMainMenu()
	{
		Debug.Log("MAIN MENU EXIT");
	}

	void Start()
	{


		/*
		ResumeGameButton.onClick.AddListener(HidePauseMenu);
		OpenSaveSubMenuButton.onClick.AddListener(OpenSaveSubMenu);
		OpenLoadSubMenuButton.onClick.AddListener(OpenLoadSubMenu);
		OpenImagesSubMenuButton.onClick.AddListener(OpenImagesSubMenu);
		OpenSettingsSubMenuButton.onClick.AddListener(OpenSettingsSubMenu);
		ExitToMainMenuButton.onClick.AddListener(ExitToMainMenu);
		*/

	}
}



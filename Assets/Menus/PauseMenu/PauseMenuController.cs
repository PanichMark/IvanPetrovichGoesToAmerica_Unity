using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private GameObject PauseMenuCanvas;
    private MenuManager menuManager;

	public void Initialize(MenuManager menuManager, GameObject PauseMenuCanvas)
	{
		this.menuManager = menuManager;
		this.inputDevice = menuManager.inputDevice;
		this.PauseMenuCanvas = PauseMenuCanvas;
		
		menuManager.OnOpenPauseMenu += ShowPauseMenu;
		menuManager.OnClosePauseMenu += HidePauseMenu;
		Debug.Log("PauseMenuController Initialized");
	}




	

	public Button ResumeGameButton;
    public Button OpenSaveSubMenuButton;
	public Button OpenLoadSubMenuButton;
	public Button OpenImagesSubMenuButton;
	public Button OpenSettingsSubMenuButton;
	public Button ExitToMainMenuButton;
	
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


	// Функция, вызываемая при событии "открыть меню паузы"
	private void ShowPauseMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(true); // Покажем меню паузы
	}
	private void HidePauseMenu()
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

	public void OpenImagesSubMenu()
	{
		//PauseMenuCanvas.gameObject.SetActive(false);
		//Debug.Log("PauseMenu closed");

		//imagesSubMenuController.ImagesSubMenuCanvas.gameObject.SetActive(true);
		Debug.Log("ImagesSubMenu opened");
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
}



using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    //PlayerInputsList playerInputsList;
    MenuManager menuManager;

	SaveSubMenuController saveSubMenuController;
	LoadSubMenuController loadSubMenuController;
	ImagesSubMenuController imagesSubMenuController;
	SettingsSubMenuController settingsSubMenuController;

	public Canvas PauseMenuCanvas;

	public Button ResumeGameButton;
    public Button OpenSaveSubMenuButton;
	public Button OpenLoadSubMenuButton;
	public Button OpenImagesSubMenuButton;
	public Button OpenSettingsSubMenuButton;
	public Button ExitToMainMenuButton;
	
	void Start()
    {
		menuManager = GetComponent<MenuManager>();

		saveSubMenuController = GetComponent<SaveSubMenuController>();
		loadSubMenuController = GetComponent<LoadSubMenuController>();
		imagesSubMenuController = GetComponent <ImagesSubMenuController>();
		settingsSubMenuController = GetComponent<SettingsSubMenuController>();

		//ResumeGameButton.onClick.AddListener(menuManager.ClosePauseMenu);
		OpenSaveSubMenuButton.onClick.AddListener(OpenSaveSubMenu);
		OpenLoadSubMenuButton.onClick.AddListener(OpenLoadSubMenu);
		OpenImagesSubMenuButton.onClick.AddListener(OpenImagesSubMenu);
		OpenSettingsSubMenuButton.onClick.AddListener(OpenSettingsSubMenu);
		ExitToMainMenuButton.onClick.AddListener(ExitToMainMenu);
	}

	public void OpenSaveSubMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(false);
		Debug.Log("PauseMenu closed");

		saveSubMenuController.SaveSubMenuCanvas.gameObject.SetActive(true);
		Debug.Log("SaveSubMenu opened");
	}

	public void OpenLoadSubMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(false);
		Debug.Log("PauseMenu closed");

		loadSubMenuController.LoadSubMenuCanvas.gameObject.SetActive(true);
		loadSubMenuController.RefreshLoadButtonLabels(); // Всегда обновляем информацию при открытии меню
		Debug.Log("LoadSubMenu opened");
	}

	public void OpenImagesSubMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(false);
		Debug.Log("PauseMenu closed");

		imagesSubMenuController.ImagesSubMenuCanvas.gameObject.SetActive(true);
		Debug.Log("ImagesSubMenu opened");
	}

	public void OpenSettingsSubMenu()
	{
		PauseMenuCanvas.gameObject.SetActive(false);
		Debug.Log("PauseMenu closed");

		settingsSubMenuController.SettingsSubMenuCanvas.gameObject.SetActive(true);
		Debug.Log("SettingsSubMenu opened");
	}

	public void ExitToMainMenu()
	{
		Debug.Log("MAIN MENU EXIT");
	}
}


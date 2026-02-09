using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSaveController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	PauseMenuController pauseMenuController;
	private bool isPauseSubMenuSaveOpened;
	private GameObject canvasPauseSubMenuSave;
	private SaveLoadController saveLoadController;
	//private Button CloseSaveSubMenuButton;
	private GameObject buttonClosePauseSubMenuSave;
	private GameObject[] buttonsSaveGame;

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, SaveLoadController saveLoadController, GameObject canvasPauseSubMenuSave, GameObject[] buttonsSaveGame, GameObject buttonClosePauseSubMenuSave)

	{
		this.buttonClosePauseSubMenuSave = buttonClosePauseSubMenuSave;
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		this.buttonsSaveGame = buttonsSaveGame;
		this.saveLoadController = saveLoadController;
		this.pauseMenuController.OnOpenSaveSubMenu += ShowSaveSubMenuCanvas;
		this.pauseMenuController.OnClosePauseSubMenu += HideSaveSubMenuCanvas;
		/*
		CloseSaveSubMenuButton.onClick.AddListener(HideImagesSubMenuCanvas);
		*/

		

		this.buttonsSaveGame[0].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.SaveGame(1));
		this.buttonsSaveGame[1].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.SaveGame(2));
		this.buttonsSaveGame[2].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.SaveGame(3));
		this.buttonsSaveGame[3].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.SaveGame(4));
		this.buttonsSaveGame[4].GetComponent<Button>().onClick.AddListener(() => this.saveLoadController.SaveGame(5));

		this.buttonClosePauseSubMenuSave.GetComponent<Button>().onClick.AddListener(() => this.pauseMenuController.ClosePauseSubMenu());

		Debug.Log("SaveSubMenu Initialized");
	}




	private void ShowSaveSubMenuCanvas()
	{
		isPauseSubMenuSaveOpened = true;
		canvasPauseSubMenuSave.gameObject.SetActive(true);
	}
	private void HideSaveSubMenuCanvas()
	{
		if (isPauseSubMenuSaveOpened)
		{
			isPauseSubMenuSaveOpened = false;
			canvasPauseSubMenuSave.gameObject.SetActive(false);
			Debug.Log("SaveSubMenu closed");
		}
	}
}



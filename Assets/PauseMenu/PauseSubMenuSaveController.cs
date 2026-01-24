using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSaveController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	PauseMenuController pauseMenuController;

	private GameObject canvasPauseSubMenuSave;
	private DataPersistenceManager dataPersistenceManager;
	private Button CloseSaveSubMenuButton;

	private Button SaveGame1Button;
	private Button SaveGame2Button;
	private Button SaveGame3Button;
	private Button SaveGame4Button;
	private Button SaveGame5Button;

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, GameObject canvasPauseSubMenuSave)

	{
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.canvasPauseSubMenuSave = canvasPauseSubMenuSave;
		pauseMenuController.OnOpenSaveSubMenu += ShowSaveSubMenuCanvas;
		pauseMenuController.OnCloseSubMenu += HideSaveSubMenuCanvas;
		/*
		CloseSaveSubMenuButton.onClick.AddListener(HideImagesSubMenuCanvas);

		SaveGame1Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(1));
		SaveGame2Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(2));
		SaveGame3Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(3));
		SaveGame4Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(4));
		SaveGame5Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(5));
		*/
		Debug.Log("SaveSubMenu Initialized");
	}




	private void ShowSaveSubMenuCanvas()
	{
		canvasPauseSubMenuSave.gameObject.SetActive(true);
	}
	private void HideSaveSubMenuCanvas()
	{
		canvasPauseSubMenuSave.gameObject.SetActive(false);
		Debug.Log("SaveSubMenu closed");
	}
}



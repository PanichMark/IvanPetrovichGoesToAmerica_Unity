using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSaveController : MonoBehaviour
{
	
	PauseMenuController pauseMenuController;

	private Canvas canvasPauseSubMenuSave;
	private DataPersistenceManager dataPersistenceManager;
	private Button CloseSaveSubMenuButton;

	private Button SaveGame1Button;
	private Button SaveGame2Button;
	private Button SaveGame3Button;
	private Button SaveGame4Button;
	private Button SaveGame5Button;
	void Start()
    {


		CloseSaveSubMenuButton.onClick.AddListener(CloseSaveSubMenu);

		SaveGame1Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(1));
		SaveGame2Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(2));
		SaveGame3Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(3));
		SaveGame4Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(4));
		SaveGame5Button.onClick.AddListener(() => dataPersistenceManager.SaveGame(5));

	}


	public void CloseSaveSubMenu()
	{
		canvasPauseSubMenuSave.gameObject.SetActive(false);
		Debug.Log("SaveSubMenu closed");
	}
}



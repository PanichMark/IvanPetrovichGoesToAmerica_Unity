using UnityEngine;
using UnityEngine.UI;

public class ImagesSubMenuController : MonoBehaviour
{
	
	private IInputDevice inputDevice;
	private MenuManager menuManager;

	private GameObject ImagesSubMenuCanvas;
	private PauseMenuController pauseMenuController;

	//private Button CloseImagesSubMenuButton;
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, GameObject ImagesSubMenuCanvas)

	{
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.ImagesSubMenuCanvas = ImagesSubMenuCanvas;
		// Подписываемся на события главного меню паузы
		pauseMenuController.OnOpenPauseSubMenu += OpenImagesSubMenu;
		menuManager.OnClosePauseSubMenu += CloseImagesSubMenu;
		Debug.Log("ImagesSubMenu Initialized");
	}

	void Start()
	{
		

		//CloseImagesSubMenuButton.onClick.AddListener(CloseImagesSubMenu);
	}

	/*
	private void Update()
	{
		if (InputManager.Instance.GetKeyPauseMenu() && ImagesSubMenuCanvas.gameObject.activeInHierarchy)
		{
			CloseImagesSubMenu();
		}
	}
	*/
	private void OpenImagesSubMenu()
	{
		ImagesSubMenuCanvas.gameObject.SetActive(true);
		//pauseMenuController.HidePauseMenu();
	
		//pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true);


	}
	private void CloseImagesSubMenu()
	{
		ImagesSubMenuCanvas.gameObject.SetActive(false);
		//pauseMenuController.ShowPauseMenu();
		menuManager.menuLevelStack.Pop();
		//pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true);

		Debug.Log("ImagesSubMenu closed");
	}
}



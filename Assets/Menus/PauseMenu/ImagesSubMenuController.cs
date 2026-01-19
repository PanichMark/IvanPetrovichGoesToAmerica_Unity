using UnityEngine;
using UnityEngine.UI;

public class ImagesSubMenuController : MonoBehaviour
{
	
	private IInputDevice inputDevice;
	private PauseMenuController pauseMenuController;

	private GameObject ImagesSubMenuCanvas;

	//private Button CloseImagesSubMenuButton;
	public void Initialize(IInputDevice inputDevice, PauseMenuController pauseMenuController, GameObject ImagesSubMenuCanvas)

	{
		this.pauseMenuController = pauseMenuController;
		this.inputDevice = inputDevice;
		this.ImagesSubMenuCanvas = ImagesSubMenuCanvas;
		// Подписываемся на события главного меню паузы
		pauseMenuController.OnOpenPauseSubMenu += OpenImagesSubMenu;
		pauseMenuController.OnClosePauseSubMenu += CloseImagesSubMenu;
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
		pauseMenuController.HidePauseMenu();

		//pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true);

		Debug.Log("ImagesSubMenu Opened");
	}
	private void CloseImagesSubMenu()
	{
		ImagesSubMenuCanvas.gameObject.SetActive(false);
		pauseMenuController.ShowPauseMenu();

		//pauseMenuController.PauseMenuCanvas.gameObject.SetActive(true);

		Debug.Log("ImagesSubMenu closed");
	}
}



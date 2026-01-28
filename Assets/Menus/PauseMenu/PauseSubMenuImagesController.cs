using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuImagesController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;

	private GameObject canvasPauseSubMenuImages;
	private PauseMenuController pauseMenuController;

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, GameObject canvasPauseSubMenuImages)

	{
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.canvasPauseSubMenuImages = canvasPauseSubMenuImages;
		this.pauseMenuController.OnOpenImagesSubMenu += ShowImagesSubMenuCanvas;
		this.pauseMenuController.OnClosePauseSubMenu += HideImagesSubMenuCanvas;
		Debug.Log("ImagesSubMenu Initialized");
	}

	private void ShowImagesSubMenuCanvas()
	{
		canvasPauseSubMenuImages.gameObject.SetActive(true);
	}
	private void HideImagesSubMenuCanvas()
	{
		canvasPauseSubMenuImages.gameObject.SetActive(false);
		Debug.Log("ImagesSubMenu closed");
	}
}
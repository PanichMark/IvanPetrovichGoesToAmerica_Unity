using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuImagesController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;

	private GameObject ImagesSubMenuCanvas;
	private PauseMenuController pauseMenuController;

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, GameObject ImagesSubMenuCanvas)

	{
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.ImagesSubMenuCanvas = ImagesSubMenuCanvas;
		pauseMenuController.OnOpenImagesSubMenu += ShowImagesSubMenuCanvas;
		pauseMenuController.OnCloseSubMenu += HideImagesSubMenuCanvas;
		Debug.Log("ImagesSubMenu Initialized");
	}

	private void ShowImagesSubMenuCanvas()
	{
		ImagesSubMenuCanvas.gameObject.SetActive(true);
	}
	private void HideImagesSubMenuCanvas()
	{
		ImagesSubMenuCanvas.gameObject.SetActive(false);
		menuManager.menuLevelStack.Pop();
		Debug.Log("ImagesSubMenu closed");
	}
}
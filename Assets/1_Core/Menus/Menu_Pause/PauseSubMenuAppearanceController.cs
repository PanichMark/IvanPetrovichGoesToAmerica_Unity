using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuAppearanceController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;

	private bool isPauseSubMenuAppearanceOpened;
	private GameObject canvasPauseSubMenuAppearance;
	private PauseMenuController pauseMenuController;
	private GameObject buttonClosePauseSubMenuAppearance;
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, GameObject canvasPauseSubMenuAppearance, GameObject buttonClosePauseSubMenuAppearance)

	{
		this.buttonClosePauseSubMenuAppearance = buttonClosePauseSubMenuAppearance;
		this.pauseMenuController = pauseMenuController;
		this.menuManager = menuManager;
		this.inputDevice = inputDevice;
		this.canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		this.pauseMenuController.OnOpenImagesSubMenu += ShowAppearanceSubMenuCanvas;
		this.pauseMenuController.OnClosePauseSubMenu += HideAppearanceSubMenuCanvas;

		this.buttonClosePauseSubMenuAppearance.GetComponent<Button>().onClick.AddListener(() => this.pauseMenuController.ClosePauseSubMenu());


		Debug.Log("ImagesSubMenu Initialized");
	}

	private void ShowAppearanceSubMenuCanvas()
	{
		isPauseSubMenuAppearanceOpened = true;
		canvasPauseSubMenuAppearance.gameObject.SetActive(true);
	}
	private void HideAppearanceSubMenuCanvas()
	{
		if (isPauseSubMenuAppearanceOpened)
		{
			isPauseSubMenuAppearanceOpened = false;
			canvasPauseSubMenuAppearance.gameObject.SetActive(false);
			Debug.Log("AppearanceSubMenu closed");
		}
	}
}
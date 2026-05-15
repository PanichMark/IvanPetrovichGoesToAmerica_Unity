using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuAppearanceController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;

	private bool _isPauseSubMenuAppearanceOpened;
	private GameObject _canvasPauseSubMenuAppearance;
	private PauseMenuController _pauseMenuController;
	private GameObject _buttonClosePauseSubMenuAppearance;
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PauseMenuController pauseMenuController, GameObject canvasPauseSubMenuAppearance, GameObject buttonClosePauseSubMenuAppearance)
	{
		_buttonClosePauseSubMenuAppearance = buttonClosePauseSubMenuAppearance;
		_pauseMenuController = pauseMenuController;
		_menuManager = menuManager;
		_inputDevice = inputDevice;
		_canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		_pauseMenuController.OnOpenImagesSubMenu += ShowAppearanceSubMenuCanvas;
		_pauseMenuController.OnClosePauseSubMenu += HideAppearanceSubMenuCanvas;

		_buttonClosePauseSubMenuAppearance.GetComponent<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());

		Debug.Log("ImagesSubMenu Initialized");
	}

	private void ShowAppearanceSubMenuCanvas()
	{
		_isPauseSubMenuAppearanceOpened = true;
		_canvasPauseSubMenuAppearance.gameObject.SetActive(true);
	}

	private void HideAppearanceSubMenuCanvas()
	{
		if (_isPauseSubMenuAppearanceOpened)
		{
			_isPauseSubMenuAppearanceOpened = false;
			_canvasPauseSubMenuAppearance.gameObject.SetActive(false);
			Debug.Log("AppearanceSubMenu closed");
		}
	}
}
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuTutorialController : MonoBehaviour
{
	private GameObject _canvasPauseSubMenuTutorial;
	private GameObject _buttonClosePauseSubMenuTutorial;
	private GameObject _buttonNextTutorial;
	private GameObject _buttonPreviousTutorial;

	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PauseMenuController _pauseMenuController;
	private bool _isPauseSubMenuTutorialOpened;

	public void Initialize(
		MenuManager menuManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuTutorial,
		GameObject buttonClosePauseSubMenuTutorial,
		GameObject buttonNextTutorial,
		GameObject buttonPreviousTutorial)
	{
		_menuManager = menuManager;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuTutorial = canvasPauseSubMenuTutorial;
		_buttonClosePauseSubMenuTutorial = buttonClosePauseSubMenuTutorial;
		_buttonNextTutorial = buttonNextTutorial;
		_buttonPreviousTutorial = buttonPreviousTutorial;

		_pauseMenuController.OnOpenAppearanceSubMenu += ShowAppearanceSubMenuCanvas;
		_pauseMenuController.OnClosePauseSubMenu += HideAppearanceSubMenuCanvas;

		_buttonClosePauseSubMenuTutorial.GetComponent<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());


		Debug.Log("TutorialSubMenu Initialized");
	}

	private void ShowAppearanceSubMenuCanvas()
	{
		_isPauseSubMenuTutorialOpened = true;
		_canvasPauseSubMenuTutorial.gameObject.SetActive(true);
	}

	private void HideAppearanceSubMenuCanvas()
	{
		if (_isPauseSubMenuTutorialOpened)
		{
			_isPauseSubMenuTutorialOpened = false;
			_canvasPauseSubMenuTutorial.gameObject.SetActive(false);
			Debug.Log("TutorialSubMenu closed");
		}
	}
}

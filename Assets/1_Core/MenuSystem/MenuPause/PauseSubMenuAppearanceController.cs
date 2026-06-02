using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuAppearanceController : MonoBehaviour
{
	private bool _isPauseSubMenuAppearanceOpened;
	private GameObject _canvasPauseSubMenuAppearance;
	private PauseMenuController _pauseMenuController;
	private GameObject _buttonClosePauseSubMenuAppearance;
	public void Initialize(PauseMenuController pauseMenuController, GameObject canvasPauseSubMenuAppearance, ViewModelPauseSubMenuAppearance viewModelPauseSubMenuAppearance)
	{
		_buttonClosePauseSubMenuAppearance = viewModelPauseSubMenuAppearance.ButtonClosePauseSubMenuAppearance;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuAppearance = canvasPauseSubMenuAppearance;
		_pauseMenuController.OnOpenAppearanceSubMenu += ShowAppearanceSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideAppearanceSubMenuCanvas;

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
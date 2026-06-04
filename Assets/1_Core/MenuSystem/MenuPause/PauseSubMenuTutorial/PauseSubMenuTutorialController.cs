using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuTutorialController : MonoBehaviour
{
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;

	private GameObject _canvasPauseSubMenuTutorial;

	private ViewModelPauseSubMenuTutorial _viewModelPauseSubMenuTutorial;

	private List<InteractionObjectNoteData> _tutorialsList = new List<InteractionObjectNoteData>();

	private GameObject _imageTutorial;
	private Image _imageComponentTutorial;
	private GameObject _textTutorial;
	private TextMeshProUGUI _textComponentTutorial;

	private GameObject _buttonNextTutorial;
	private GameObject _buttonPreviousTutorial;

	private GameObject _buttonClosePauseSubMenuTutorial;
	private TextMeshProUGUI _textButtonComponentClosePauseSubMenuTutorial;

	private bool _isPauseSubMenuTutorialOpened;

	private int _currentNoteIndex = 0;

	private bool _isInitialized;

	public void Initialize(
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuTutorial,
		ViewModelPauseSubMenuTutorial viewModelPauseSubMenuTutorial,
		List<InteractionObjectNoteData> tutorialList)
	{
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuTutorial = canvasPauseSubMenuTutorial;
		_viewModelPauseSubMenuTutorial = viewModelPauseSubMenuTutorial;
		_tutorialsList = tutorialList;

		_textTutorial = _viewModelPauseSubMenuTutorial.TextTutorial;
		_textComponentTutorial = _textTutorial.GetComponent<TextMeshProUGUI>();
		_imageTutorial = _viewModelPauseSubMenuTutorial.ImageTutorial;
		_imageComponentTutorial = _imageTutorial.GetComponent<Image>();

		_buttonNextTutorial = _viewModelPauseSubMenuTutorial.ButtonNextTutorial;
		_buttonNextTutorial.GetComponent<Button>().onClick.AddListener(() => NextTutorial());
		_buttonPreviousTutorial = _viewModelPauseSubMenuTutorial.ButtonPreviousTutorial;
		_buttonPreviousTutorial.GetComponent<Button>().onClick.AddListener(() => PreviousTutorial());

		_buttonClosePauseSubMenuTutorial = _viewModelPauseSubMenuTutorial.ButtonClosePauseSubMenuTutorial;
		_buttonClosePauseSubMenuTutorial.GetComponent<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());
		_textButtonComponentClosePauseSubMenuTutorial = _viewModelPauseSubMenuTutorial.TextButtonClosePauseSubMenuTutorial.GetComponent<TextMeshProUGUI>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseMenuController.OnOpenTutorialSubMenu += ShowTutorialSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideTutorialSubMenuCanvas;

		_isInitialized = true;

		Debug.Log("PauseSubMenuTutorialController Initialized");
	}

	private void Update()
	{
		if (!_isInitialized)
			return;

		if (_isPauseSubMenuTutorialOpened)
		{
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				NextTutorial();
			}

			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				PreviousTutorial();
			}
		}
	}

	private void ShowTutorialSubMenuCanvas()
	{
		_isPauseSubMenuTutorialOpened = true;
		_canvasPauseSubMenuTutorial.SetActive(true);

		if (_tutorialsList.Count > 0)
		{
			_currentNoteIndex = 0;
			UpdateUIWithCurrentNote();
		}
	}

	private void HideTutorialSubMenuCanvas()
	{
		if (_isPauseSubMenuTutorialOpened)
		{
			_isPauseSubMenuTutorialOpened = false;
			_canvasPauseSubMenuTutorial.SetActive(false);
			Debug.Log("TutorialSubMenu closed");
		}
	}

	private void NextTutorial()
	{
		_currentNoteIndex = (_currentNoteIndex + 1) % _tutorialsList.Count;

		UpdateUIWithCurrentNote();
	}

	private void PreviousTutorial()
	{
		_currentNoteIndex = (_currentNoteIndex - 1 + _tutorialsList.Count) % _tutorialsList.Count;

		UpdateUIWithCurrentNote();
	}

	private void UpdateUIWithCurrentNote()
	{
		InteractionObjectNoteData data = _tutorialsList[_currentNoteIndex];

		Debug.Log($"Showing TutorialNote #{_currentNoteIndex + 1}");

		string textToShow = _localizationManager.GetLanguageSuffix(data);

		_textComponentTutorial.text = textToShow;

		Sprite spriteToShow = data.NoteImage;
		_imageComponentTutorial.sprite = spriteToShow;

		if (spriteToShow != null)
		{
			_imageComponentTutorial.sprite = spriteToShow;
			_imageTutorial.SetActive(true);
		}
		else
		{
			_imageTutorial.SetActive(false);
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textButtonComponentClosePauseSubMenuTutorial.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuTutorial_ButtonClosePauseSubMenuTutorial");
	}
}
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuTutorialController : MonoBehaviour
{
	private GameObject _canvasPauseSubMenuTutorial;
	private GameObject _buttonClosePauseSubMenuTutorial;
	private GameObject _buttonNextTutorial;
	private GameObject _buttonPreviousTutorial;

	private GameObject _tutorialNoteText;
	private GameObject _tutorialNoteImage;

	private MenuManager _menuManager;
	private PauseMenuController _pauseMenuController;
	private bool _isPauseSubMenuTutorialOpened;

	private List<InteractionObjectNoteData> _tutorialNotes = new List<InteractionObjectNoteData>();
	private int _currentNoteIndex = 0;

	private TextMeshProUGUI _textComponent;
	private Image _imageComponent;

	public void Initialize(
		MenuManager menuManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuTutorial,
		GameObject buttonClosePauseSubMenuTutorial,
		GameObject buttonNextTutorial,
		GameObject buttonPreviousTutorial,
		GameObject tutorialNoteText,
		GameObject tutorialNoteImage,
		List<InteractionObjectNoteData> tutorialNotes)
	{
		_menuManager = menuManager;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuTutorial = canvasPauseSubMenuTutorial;
		_buttonClosePauseSubMenuTutorial = buttonClosePauseSubMenuTutorial;
		_buttonNextTutorial = buttonNextTutorial;
		_buttonPreviousTutorial = buttonPreviousTutorial;

		_tutorialNoteText = tutorialNoteText;
		_tutorialNoteImage = tutorialNoteImage;

		_textComponent = _tutorialNoteText.GetComponent<TextMeshProUGUI>();
		_imageComponent = _tutorialNoteImage.GetComponent<Image>();

		_tutorialNotes = tutorialNotes;

		_pauseMenuController.OnOpenTutorialSubMenu += ShowAppearanceSubMenuCanvas;
		_pauseMenuController.OnCloseAnyPauseSubMenu += HideAppearanceSubMenuCanvas;

		_buttonClosePauseSubMenuTutorial.GetComponent<Button>().onClick.AddListener(() => _pauseMenuController.ClosePauseSubMenu());

		_buttonNextTutorial.GetComponent<Button>().onClick.AddListener(() => OnNextTutorial());
		_buttonPreviousTutorial.GetComponent<Button>().onClick.AddListener(() => OnPreviousTutorial());

		Debug.Log("TutorialSubMenu Initialized");
	}

	private void ShowAppearanceSubMenuCanvas()
	{
		_isPauseSubMenuTutorialOpened = true;
		_canvasPauseSubMenuTutorial.SetActive(true);

		if (_tutorialNotes.Count > 0)
		{
			_currentNoteIndex = 0;
			UpdateUIWithCurrentNote();
		}
	}

	private void HideAppearanceSubMenuCanvas()
	{
		if (_isPauseSubMenuTutorialOpened)
		{
			_isPauseSubMenuTutorialOpened = false;
			_canvasPauseSubMenuTutorial.SetActive(false);
			Debug.Log("TutorialSubMenu closed");
		}
	}

	private void OnNextTutorial()
	{
		CloseCurrentDisplay();

		_currentNoteIndex = (_currentNoteIndex + 1) % _tutorialNotes.Count;

		UpdateUIWithCurrentNote();
	}

	private void OnPreviousTutorial()
	{
		CloseCurrentDisplay();

		_currentNoteIndex = (_currentNoteIndex - 1 + _tutorialNotes.Count) % _tutorialNotes.Count;

		UpdateUIWithCurrentNote();
	}

	private void UpdateUIWithCurrentNote()
	{
		InteractionObjectNoteData data = _tutorialNotes[_currentNoteIndex];

		Debug.Log($"Showing TutorialNote #{_currentNoteIndex + 1}");

		string textToShow = data.NoteText_RU.text;
		
		_textComponent.text = textToShow;

		if (_imageComponent != null)
		{
			Sprite spriteToShow = data.NoteImage;
			_imageComponent.sprite = spriteToShow;
			if (spriteToShow != null)
			{
				_imageComponent.sprite = spriteToShow;
				_tutorialNoteImage.SetActive(true);
			}
			else
			{
				_tutorialNoteImage.SetActive(false);
			}
		}

		if (_textComponent != null)
			_textComponent.gameObject.SetActive(true);
	}

	private void CloseCurrentDisplay()
	{
		if (_textComponent != null)
			_textComponent.gameObject.SetActive(false);

		if (_imageComponent != null)
			_tutorialNoteImage.SetActive(false);
	}
}
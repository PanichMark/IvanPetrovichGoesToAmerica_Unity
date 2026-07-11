using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PauseSubMenuSettingsGameDifficultyController : MonoBehaviour
{
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private LocalizationManager _localizationManager;

	private GameObject _canvasGameDifficulty;

	private List<InteractionObjectNoteData> _difficultiesList = new List<InteractionObjectNoteData>();

	private GameObject _imageContainer;
	private Image _imageComponent;
	private GameObject _textContainer;
	private TextMeshProUGUI _textComponent;

	private GameObject _buttonNext;
	private GameObject _buttonPrevious;
	private GameObject _buttonClose;
	private TextMeshProUGUI _textButtonComponentClose;

	private bool _isOpened;
	private int _currentIndex = 0;
	private bool _isInitialized;

	public void Initialize(
		LocalizationManager localizationManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		GameObject canvasGameDifficulty,
		ViewModelPauseSubMenuSettingsGameDifficultyController viewModelPauseSubMenuSettingsGameDifficultyController)
	{
		_localizationManager = localizationManager;	
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasGameDifficulty = canvasGameDifficulty;

		_difficultiesList = ((GameDifficultiesList)Resources.Load("GameDifficultiesList")).Notes;

		_textContainer = viewModelPauseSubMenuSettingsGameDifficultyController.TextGameDifficulty;
		_textComponent = _textContainer.GetComponent<TextMeshProUGUI>();
		_imageContainer = viewModelPauseSubMenuSettingsGameDifficultyController.ImageGameDifficulty;
		_imageComponent = _imageContainer.GetComponent<Image>();

		_buttonNext = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonNextGameDifficulty;
		_buttonNext.GetComponent<Button>().onClick.AddListener(() => NextDifficulty());
		_buttonPrevious = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonPreviousGameDifficulty;
		_buttonPrevious.GetComponent<Button>().onClick.AddListener(() => PreviousDifficulty());

		_buttonClose = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonCloseSettingsGameDifficulty;
		_buttonClose.GetComponent<Button>().onClick.AddListener(() => _pauseSubMenuSettingsSectionGeneralController.CloseSubMenuChooseGameDifficulty());
		_textButtonComponentClose = viewModelPauseSubMenuSettingsGameDifficultyController.TextButtonCloseSettingsGameDifficulty.GetComponent<TextMeshProUGUI>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseSubMenuSettingsSectionGeneralController.OnOpenSubMenuGameDifficulty += ShowMenuGameDifficulty;
		_pauseSubMenuSettingsSectionGeneralController.OnCloseSubMenuGameDifficulty += HideMenuGameDifficulty;

		_isInitialized = true;
	}

	private void Update()
	{
		if (!_isInitialized || !_isOpened) return;

		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			NextDifficulty();
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			PreviousDifficulty();
		}
	}

	private void ShowMenuGameDifficulty()
	{
		_isOpened = true;
		_canvasGameDifficulty.SetActive(true);

		if (_difficultiesList.Count > 0)
		{
			_currentIndex = 0;
			UpdateUIDifficulty();
		}
	}

	private void HideMenuGameDifficulty()
	{
		if (_isOpened)
		{
			_isOpened = false;
			_canvasGameDifficulty.SetActive(false);
		}
	}

	private void NextDifficulty()
	{
		_currentIndex = (_currentIndex + 1) % _difficultiesList.Count;
		UpdateUIDifficulty();
	}

	private void PreviousDifficulty()
	{
		_currentIndex = (_currentIndex - 1 + _difficultiesList.Count) % _difficultiesList.Count;
		UpdateUIDifficulty();
	}

	private void UpdateUIDifficulty()
	{
		InteractionObjectNoteData data = _difficultiesList[_currentIndex];
		string textToShow = _localizationManager.GetLanguageSuffix(data);
		_textComponent.text = textToShow;

		Sprite spriteToShow = data.NoteImage;
		_imageComponent.sprite = spriteToShow;
		_imageContainer.SetActive(spriteToShow != null);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		_textButtonComponentClose.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenuSettingsGameDifficultyController_ButtonClose");
		UpdateUIDifficulty();
	}
}
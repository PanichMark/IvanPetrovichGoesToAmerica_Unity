using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuSettingsGameDifficultyController : MonoBehaviour
{
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private LocalizationManager _localizationManager;

	private GameObject _canvasGameDifficulty;

	private GameDifficultiesList _difficultiesList;

	private GameObject _imageGameDifficulty;
	private Image _imageComponentGameDifficulty;
	private GameObject _textGameDifficultyHeader;
	private TextMeshProUGUI _textComponentGameDifficultyHeader;
	private GameObject _textGameDifficultyDescription;
	private TextMeshProUGUI _textComponentGameDifficultyDescription;

	private GameObject _buttonNextGameDifficulty;
	private GameObject _buttonPreviousGameDifficulty;
	private GameObject _buttonCloseSettingsGameDifficulty;
	private TextMeshProUGUI _textComponentButtonCloseSettingsGameDifficulty;

	private GameObject _difficultyNotAvailavle;
	private GameObject _textDifficultyNotAvailavle;
	private TextMeshProUGUI _textComponentDifficultyNotAvailable;

	public bool IsChooseGameDifficultyMenuOpened { get; private set; }
	private int _currentIndex = 0;
	private bool _isInitialized;

	public void Initialize(
		LocalizationManager localizationManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		GameDifficultiesList difficultiesList,
		GameObject canvasGameDifficulty,
		ViewModelPauseSubMenuSettingsGameDifficultyController viewModelPauseSubMenuSettingsGameDifficultyController)
	{
		_localizationManager = localizationManager;	
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasGameDifficulty = canvasGameDifficulty;

		_difficultiesList = difficultiesList;

		_textGameDifficultyHeader = viewModelPauseSubMenuSettingsGameDifficultyController.TextGameDifficultyHeader;
		_textComponentGameDifficultyHeader = _textGameDifficultyHeader.GetComponent<TextMeshProUGUI>();
		_textGameDifficultyDescription = viewModelPauseSubMenuSettingsGameDifficultyController.TextGameDifficultyDescription;
		_textComponentGameDifficultyDescription = _textGameDifficultyDescription.GetComponent<TextMeshProUGUI>();

		_imageGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.ImageGameDifficulty;
		_imageComponentGameDifficulty = _imageGameDifficulty.GetComponent<Image>();

		Sprite spriteToShow = _difficultiesList.Notes[_currentIndex].NoteImage;
		_imageComponentGameDifficulty.sprite = spriteToShow;
		_imageGameDifficulty.SetActive(spriteToShow != null);

		_buttonNextGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonNextGameDifficulty;
		_buttonNextGameDifficulty.GetComponent<Button>().onClick.AddListener(() => NextDifficulty());
		_buttonPreviousGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonPreviousGameDifficulty;
		_buttonPreviousGameDifficulty.GetComponent<Button>().onClick.AddListener(() => PreviousDifficulty());

		_buttonCloseSettingsGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.ButtonCloseSettingsGameDifficulty;
		_buttonCloseSettingsGameDifficulty.GetComponent<Button>().onClick.AddListener(() => _pauseSubMenuSettingsSectionGeneralController.CloseSubMenuChooseGameDifficulty());
		_textComponentButtonCloseSettingsGameDifficulty = viewModelPauseSubMenuSettingsGameDifficultyController.TextButtonCloseSettingsGameDifficulty.GetComponent<TextMeshProUGUI>();

		_difficultyNotAvailavle = viewModelPauseSubMenuSettingsGameDifficultyController.DifficultyNotAvailable;
		_textDifficultyNotAvailavle = viewModelPauseSubMenuSettingsGameDifficultyController.TextDifficultyNotAvailable;
		_textComponentDifficultyNotAvailable = viewModelPauseSubMenuSettingsGameDifficultyController.TextDifficultyNotAvailable.GetComponent<TextMeshProUGUI>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_pauseSubMenuSettingsSectionGeneralController.OnOpenSubMenuGameDifficulty += ShowMenuGameDifficulty;
		_pauseSubMenuSettingsSectionGeneralController.OnCloseSubMenuGameDifficulty += HideMenuGameDifficulty;

		_isInitialized = true;
	}

	private void Update()
	{
		if (!_isInitialized || !IsChooseGameDifficultyMenuOpened) return;

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
		IsChooseGameDifficultyMenuOpened = true;
		_canvasGameDifficulty.SetActive(true);

		_currentIndex = 1;
		UpdateGameDifficultyUI();
	}

	public void HideMenuGameDifficulty()
	{
		if (IsChooseGameDifficultyMenuOpened)
		{
			IsChooseGameDifficultyMenuOpened = false;
			_canvasGameDifficulty.SetActive(false);
		}
	}

	private void NextDifficulty()
	{
		_currentIndex = (_currentIndex + 1) % _difficultiesList.Notes.Count;
		UpdateGameDifficultyUI();
	}

	private void PreviousDifficulty()
	{
		_currentIndex = (_currentIndex - 1 + _difficultiesList.Notes.Count) % _difficultiesList.Notes.Count;
		UpdateGameDifficultyUI();
	}

	private void UpdateGameDifficultyUI()
	{
		if (_currentIndex == 1)
		{
			 _difficultyNotAvailavle.SetActive(false);
			_imageComponentGameDifficulty.color = Color.white;
		}
		else
		{
			_difficultyNotAvailavle.SetActive(true);
			_imageComponentGameDifficulty.color = Color.grey;
		}

		_textComponentGameDifficultyHeader.text = _localizationManager.GetLocalizedString($"UI_Menu_PauseSubMenu_Settings_SectionGeneral_GameDifficulty{_currentIndex + 1}");

		InteractionObjectNoteData data = _difficultiesList.Notes[_currentIndex];
		string textToShow = _localizationManager.GetNoteLanguageSuffix(data);
		_textComponentGameDifficultyDescription.text = textToShow;

		Sprite spriteToShow = data.NoteImage;
		_imageComponentGameDifficulty.sprite = spriteToShow;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;
		_textComponentButtonCloseSettingsGameDifficulty.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_GameDifficulty_ButtonClose");
		_textComponentDifficultyNotAvailable.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Settings_SectionGeneral_GameDifficulty_THIS-GAME-DIFFICULTY-IS-NOT-AVAILABLE-IN-DEMO");

		UpdateGameDifficultyUI();
	}
}
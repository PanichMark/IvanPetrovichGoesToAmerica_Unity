using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseSubMenuTutorialController : MonoBehaviour
{
	private LocalizationManager _localizationManager;
	private PauseMenuController _pauseMenuController;
	private IInputDevice _inputDevice;
	private GameObject _canvasPauseSubMenuTutorial;

	private ViewModelPauseSubMenuTutorial _viewModelPauseSubMenuTutorial;

	private GameTutorialsList _tutorialsList;

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
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		PauseMenuController pauseMenuController,
		GameObject canvasPauseSubMenuTutorial,
		GameTutorialsList tutorialsList,
		ViewModelPauseSubMenuTutorial viewModelPauseSubMenuTutorial)
	{
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;
		_canvasPauseSubMenuTutorial = canvasPauseSubMenuTutorial;
		_viewModelPauseSubMenuTutorial = viewModelPauseSubMenuTutorial;

		_tutorialsList = tutorialsList;

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

		if (_tutorialsList.Notes.Count > 0)
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
		_currentNoteIndex = (_currentNoteIndex + 1) % _tutorialsList.Notes.Count;

		UpdateUIWithCurrentNote();
	}

	private void PreviousTutorial()
	{
		_currentNoteIndex = (_currentNoteIndex - 1 + _tutorialsList.Notes.Count) % _tutorialsList.Notes.Count;

		UpdateUIWithCurrentNote();
	}

	private void UpdateUIWithCurrentNote()
	{
		InteractionObjectNoteData data = _tutorialsList.Notes[_currentNoteIndex];

		Debug.Log($"Showing TutorialNote #{_currentNoteIndex + 1}");

		// 1. Получаем сырой текст (например: "Двигайтесь на кнопки {MoveForward}")
		string rawTextToShow = _localizationManager.GetNoteLanguageSuffix(data);

		// 2. ОБЯЗАТЕЛЬНО пропускаем его через ReplaceActionTags и сохраняем результат
		string finalText = ReplaceActionTags(rawTextToShow);

		// 3. Отдаем ВЕСЬ отформатированный текст компоненту TextMeshPro
		_textComponentTutorial.text = finalText;

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

	private string ReplaceActionTags(string input)
	{
		// Ищем всё, что находится внутри фигурных скобок {здесь}
		System.Text.RegularExpressions.Regex tagRegex = new(@"\{([^}]+)\}");

		return tagRegex.Replace(input, match =>
		{
			string actionStringFromFile = match.Groups[1].Value;

			// Пытаемся превратить строку из файла (например, "Run") в наш Enum
			if (System.Enum.TryParse(typeof(InputControlsEnum), actionStringFromFile, out object parsedEnum))
			{
				InputControlsEnum actionEnum = (InputControlsEnum)parsedEnum;

				// Спрашиваем у инпута актуальное имя кнопки
				string keyName = _inputDevice.GetNameOfKey(actionEnum);

				// Возвращаем готовую строку с кастомным цветом
				return _inputDevice.GetNameOfKey(actionEnum);
			}
			else
			{
				Debug.LogWarning($"В туториале найден неизвестный тег {{{actionStringFromFile}}}. Проверьте .txt файл.");
				return match.Value;
			}
		});
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		_textButtonComponentClosePauseSubMenuTutorial.text = _localizationManager.GetLocalizedString("UI_Menu_PauseSubMenu_Tutorial_ButtonClosePauseSubMenuTutorial");
	}
}
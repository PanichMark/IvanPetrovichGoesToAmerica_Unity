using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class InteractionObjectNote : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;
	[SerializeField] private string _interactionObjectNameUI;
	private IInputDevice _inputDevice;
	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(_interactionObjectNameUI)}";
	private TextMeshProUGUI _textButtonExit;
	[SerializeField] private InteractionObjectNoteData _noteData;

	private MenuManager _menuManager;
	private bool _isReading;
	private LocalizationManager _localizationManager;
	public event IInteractable.InteractableObjectHandler OnInteract;
	public string InteractionHintMessageMain => $"{_interactionHintMessageAction} {InteractionObjectNameUI}?";
	public string InteractionHintMessageFail => null;

	private GameObject _canvasNoteMenu;
	private Button _buttonExitNoteMenu;
	private string _interactionHintMessageAction;
	public string InteractionHintMessageAction => _interactionHintMessageAction;

	private RectTransform _imageRectTransform;
	private RectTransform _textRectTransform;
	private Image _textBackground;
	private RectTransform _textBackgroundTransform;
	private TextMeshProUGUI _textComponent;
	private Image _imageComponent;

	[SerializeField] private InteractionObjectNote _noteToOpenAfter;

	public bool IsInteractionHintMessageFailActive => false;
	private GameScenesManager _gameSceneManager;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		_textButtonExit = ServiceLocator.Resolve<GameObject>("TextButtonCloseReadNoteMenu").GetComponent<TextMeshProUGUI>();

		_inputDevice = ServiceLocator.Resolve<IInputDevice>("InputDevice");

		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_buttonExitNoteMenu = ServiceLocator.Resolve<GameObject>("ButtonCloseReadNoteMenu").GetComponent<Button>();
		_imageComponent = ServiceLocator.Resolve<GameObject>("ImageNote").GetComponent<Image>();

		_textBackground = ServiceLocator.Resolve<GameObject>("ImageNoteBlackBackground").GetComponent<Image>();
		_canvasNoteMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuNote");

		_gameSceneManager = ServiceLocator.Resolve<GameScenesManager>("GameSceneManager");
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseAndDeactivate;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseAndDeactivate;

		_textComponent = ServiceLocator.Resolve<GameObject>("TextNote").GetComponent<TextMeshProUGUI>();
		_textRectTransform = _textComponent.gameObject.GetComponent<RectTransform>();
		
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_imageRectTransform = _imageComponent.gameObject.GetComponent<RectTransform>();
		_textBackgroundTransform = _textBackground.gameObject.GetComponent<RectTransform>();

		_buttonExitNoteMenu.onClick.AddListener(CloseAndDeactivate);
	

		_menuManager.OnOpenPauseMenu += HideNoteCanvas;
		_menuManager.OnClosePauseMenu += ShowNoteCanvas;
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		if (_isReading)
		{
			_localizationManager = localizationManager;

			if (!_noteData.IsNoteToGlanceAt)
			{
				_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Read")}";
				_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_Text")}";

			}
			else
			{
				_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GlanceAt")}";
				_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_NoText")}";
			}

			_textComponent.text = _localizationManager.GetNoteLanguageSuffix(_noteData);
		}
	}

	private void HideNoteCanvas()
	{
		if (_isReading)
		{
			_canvasNoteMenu.SetActive(false);
		}
	}

	private void ShowNoteCanvas()
	{
		if (_isReading)
		{
			_canvasNoteMenu.SetActive(true);
		}
	}

	public void Interact()
	{
		Debug.Log("BRUH!");

		_menuManager.OpenInteractionMenu();
		_isReading = true;

		_canvasNoteMenu.SetActive(true);

		_imageComponent.gameObject.SetActive(true);
		_imageComponent.sprite = _noteData.NoteImage;

		if (!_noteData.IsNoteToGlanceAt)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Read")}";
			_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_Text")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GlanceAt")}";
			_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_NoText")}";
		}

		_textComponent.text = ReplaceActionTags(_localizationManager.GetNoteLanguageSuffix(_noteData));

		_imageRectTransform.anchoredPosition = _noteData.NotePosition.ImagePosition;
		_imageRectTransform.localEulerAngles = new Vector3(0f, 0f, _noteData.NotePosition.ImageRotation.x);
		_imageRectTransform.sizeDelta = new Vector2(_noteData.NotePosition.ImageWidth, _noteData.NotePosition.ImageHeight);

		_textRectTransform.anchoredPosition = _noteData.NotePosition.TextPosition;
		_textRectTransform.localEulerAngles = new Vector3(0f, 0f, _noteData.NotePosition.TextRotation.x);
		_textRectTransform.sizeDelta = new Vector2(_noteData.NotePosition.TextWidth, _noteData.NotePosition.TextHeight);

		_textBackgroundTransform.anchoredPosition = _noteData.NotePosition.TextPosition;
		_textBackgroundTransform.localEulerAngles = new Vector3(0f, 0f, _noteData.NotePosition.TextRotation.x);
		_textBackgroundTransform.sizeDelta = new Vector2(_noteData.NotePosition.TextWidth, _noteData.NotePosition.TextHeight);

		gameObject.tag = "Untagged";
	}

	public void InteractCutscene()
	{
		Interact();
	}

	private string ReplaceActionTags(string input)
	{
		if (_inputDevice == null || string.IsNullOrEmpty(input))
		{
			return input;
		}

		System.Text.RegularExpressions.Regex tagRegex = new(@"\{([^}]+)\}");

		return tagRegex.Replace(input, match =>
		{
			string actionStringFromFile = match.Groups[1].Value;

			// Пытаемся превратить строку из файла (например, "Run") в наш Enum
			if (System.Enum.TryParse(typeof(InputControlsEnum), actionStringFromFile, out object parsedEnum))
			{
				InputControlsEnum actionEnum = (InputControlsEnum)parsedEnum;

				// Спрашиваем у инпута актуальное имя кнопки с учетом раскладки/геймпада
				return _inputDevice.GetNameOfKey(actionEnum);
			}
			else
			{
				Debug.LogWarning($"В заметке '{_interactionObjectNameUI}' найден неизвестный тег {{{actionStringFromFile}}}. Проверьте .txt файл.");
				return match.Value;
			}
		});
	}

	private void CloseAndDeactivate()
	{
		if (_isReading)
		{
			_isReading = false;

			_textComponent.text = string.Empty;
				
			_imageComponent.sprite = null;
	
			_canvasNoteMenu.SetActive(false);
		
			_menuManager.CloseInteractionMenu();

			gameObject.tag = "Interactable";

			if (_noteToOpenAfter != null)
			{
				StartCoroutine(DelayedOpenNextNote());
			}
		}
	}

	private IEnumerator DelayedOpenNextNote()
	{
		yield return new WaitForSecondsRealtime(0.01f);

		_noteToOpenAfter.Interact();
	}
}
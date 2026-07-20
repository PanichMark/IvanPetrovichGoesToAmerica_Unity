using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionObjectNote : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;
	[SerializeField] private string _interactionObjectNameUI;
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

	public bool IsInteractionHintMessageFailActive => false;
	private GameScenesManager _gameSceneManager;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		_textButtonExit = ServiceLocator.Resolve<GameObject>("TextButtonCloseReadNoteMenu").GetComponent<TextMeshProUGUI>();

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

			if (!_noteData.IsLittleText)
			{
				_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Read")}";
				_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_Text")}";

			}
			else
			{
				_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GlanceAt")}";
				_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_NoText")}";
			}

			_textComponent.text = _localizationManager.GetLanguageSuffix(_noteData);
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
		_menuManager.OpenInteractionMenu();
		_isReading = true;

		_canvasNoteMenu.SetActive(true);

		_imageComponent.gameObject.SetActive(true);
		_imageComponent.sprite = _noteData.NoteImage;

		if (!_noteData.IsLittleText)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Read")}";
			_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_Text")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GlanceAt")}";
			_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_NoText")}";
		}

		_textComponent.text = _localizationManager.GetLanguageSuffix(_noteData);

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
		}
	}
}
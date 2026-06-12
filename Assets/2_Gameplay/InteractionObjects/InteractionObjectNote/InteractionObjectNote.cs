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
	[SerializeField] private InteractionObjectNotePosition _notePosition;
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
	private TextMeshProUGUI _textComponent;
	private Image _imageComponent;

	public bool IsInteractionHintMessageFailActive => false;
	private GameSceneManager _gameSceneManager;

	private void Start()
	{

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		//_interactionObjectNameUI = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_Money")}!";
		_textButtonExit = ServiceLocator.Resolve<GameObject>("TextButtonCloseReadNoteMenu").GetComponent<TextMeshProUGUI>();

		// Остальная инициализация остается прежней
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_buttonExitNoteMenu = ServiceLocator.Resolve<GameObject>("ButtonCloseReadNoteMenu").GetComponent<Button>();
		_imageComponent = ServiceLocator.Resolve<GameObject>("ImageNote").GetComponent<Image>();

		_textBackground = ServiceLocator.Resolve<GameObject>("ImageNoteBlackBackground").GetComponent<Image>();
		_canvasNoteMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuNote");

		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseAndDeactivate;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseAndDeactivate;

	
		_textComponent = ServiceLocator.Resolve<GameObject>("TextNote").GetComponent<TextMeshProUGUI>();
		_textRectTransform = _textComponent.gameObject.GetComponent<RectTransform>();
		

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_imageRectTransform = _imageComponent.gameObject.GetComponent<RectTransform>();
		
		_buttonExitNoteMenu.onClick.AddListener(CloseAndDeactivate);
		_menuManager.OnOpenPauseMenu += HideNoteCanvas;
		_menuManager.OnClosePauseMenu += ShowNoteCanvas;
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (_notePosition.IsThereText)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Read")}";
			_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_Text")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GlanceAt")}";
			_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_NoText")}";
		}

		if (_notePosition.IsThereText)
		{
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

		// Используем данные из ScriptableObject напрямую
		_imageComponent.gameObject.SetActive(true);
		_imageComponent.sprite = _noteData.NoteImage;

		if (_notePosition.IsThereText)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Read")}";
			_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_Text")}";

			_textBackground.gameObject.SetActive(true);
			_textComponent.text = _localizationManager.GetLanguageSuffix(_noteData);
		}
		else
		{
			_textBackground.gameObject.SetActive(false);

			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GlanceAt")}";
			_textButtonExit.text = $"{_localizationManager.GetLocalizedString("UI_Menu_InteractionMenu_Note_ButtonCloseNoteMenu_NoText")}";
		}

		_imageRectTransform.anchoredPosition = _notePosition.TextPosition;
		_imageRectTransform.localEulerAngles = _notePosition.TextRotation;

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

			if (_notePosition.IsThereText)
			{

				// Деактивируем объекты
				_textBackground.gameObject.SetActive(false);

				// Очищаем текст и изображение (не обязательно, но полезно для чистоты)
				_textComponent.text = string.Empty;
				
			}

			_imageComponent.sprite = null;
			// Закрываем меню и сбрасываем состояние кнопки (если нужно)
			_canvasNoteMenu.SetActive(false);
			_menuManager.CloseInteractionMenu();

			gameObject.tag = "Interactable";
		}
	}
}
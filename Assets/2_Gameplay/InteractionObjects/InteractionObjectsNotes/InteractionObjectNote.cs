using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionObjectNote : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	[SerializeField] private string _interactionObjectNameUI;
	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(_interactionObjectNameUI)}";

	[SerializeField] private InteractionObjectNoteData _noteData;
	[SerializeField] private InteractionObjectNotePosition _notePosition;
	private MenuManager _menuManager;
	private bool _isReading;
	private LocalizationManager _localizationManager;
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

		if (_notePosition.IsThereText)
		{
			_textComponent = ServiceLocator.Resolve<TextMeshProUGUI>("TextNote");
			_textRectTransform = _textComponent.gameObject.GetComponent<RectTransform>();

			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Read")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GlanceAt")}";
		}

		// Остальная инициализация остается прежней
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_buttonExitNoteMenu = ServiceLocator.Resolve<Button>("ButtonCloseReadNoteMenu");
		_imageComponent = ServiceLocator.Resolve<Image>("ImageNote");

		_textBackground = ServiceLocator.Resolve<Image>("ImageNoteBlackBackground");
		_canvasNoteMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuNote");

		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseAndDeactivate;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseAndDeactivate;

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_imageRectTransform = _imageComponent.gameObject.GetComponent<RectTransform>();
		
		_buttonExitNoteMenu.onClick.AddListener(CloseAndDeactivate);
		_menuManager.OnOpenPauseMenu += HideNoteCanvas;
		_menuManager.OnClosePauseMenu += ShowNoteCanvas;
	}

	public void ChangeLanguage()
	{
		if (_notePosition.IsThereText)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Read")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_GlanceAt")}";
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
		if (_noteData == null) return; // Защита от NullReference

		_menuManager.OpenInteractionMenu();
		_isReading = true;

		_canvasNoteMenu.SetActive(true);

		// Используем данные из ScriptableObject напрямую
		_imageComponent.gameObject.SetActive(true);
		_imageComponent.sprite = _noteData.NoteImage;

		if (_notePosition.IsThereText)
		{
			_textBackground.gameObject.SetActive(true);
			_textComponent.text = _localizationManager.GetLanguageSuffix(_noteData);
		}
		else
		{
			_textBackground.gameObject.SetActive(false);
		}



			// Позиция и поворот для случая с текстом (как в оригинале)
			_imageRectTransform.anchoredPosition = _notePosition.TextPosition;
		_imageRectTransform.localEulerAngles = _notePosition.TextRotation;

		// Убедимся, что обработчик добавлен только один раз (опционально)


		gameObject.tag = "Untagged";
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
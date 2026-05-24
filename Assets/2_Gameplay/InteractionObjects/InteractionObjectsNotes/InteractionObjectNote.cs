using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionObjectNote : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	[SerializeField] private string _interactionObjectNameUI;
	public string InteractionObjectNameUI => _interactionObjectNameUI;

	// Заменяем поля на ссылку на ScriptableObject
	[SerializeField] private InteractionObjectNoteData _noteData;
	[SerializeField] private InteractionObjectNotePosition _notePosition;
	private MenuManager _menuManager;
	private bool _isReading;
	private LocalizationManager _localizationManager;
	public string InteractionHintMessageMain => $"Прочитать {InteractionObjectNameUI}";
	public string InteractionHintMessageFail => null;

	private GameObject _canvasNoteMenu;
	private Button _buttonExitNoteMenu;

	public string InteractionHintMessageAction { get; protected set; }

	private RectTransform _imageRectTransform;
	private RectTransform _textRectTransform;
	private Image _textBackground;
	private TextMeshProUGUI _textComponent;
	private Image _imageComponent;

	public bool IsInteractionHintMessageFailActive => false;
	private GameSceneManager _gameSceneManager;

	private void Start()
	{
		if(_notePosition.IsThereText)
		{
			_textComponent = ServiceLocator.Resolve<TextMeshProUGUI>("TextNote");
			_textRectTransform = _textComponent.gameObject.GetComponent<RectTransform>();
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

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_imageRectTransform = _imageComponent.gameObject.GetComponent<RectTransform>();
		
		_buttonExitNoteMenu.onClick.AddListener(CloseAndDeactivate);
		_menuManager.OnOpenPauseMenu += HideNoteCanvas;
		_menuManager.OnClosePauseMenu += ShowNoteCanvas;
	}

	public void ChangeLanguage()
	{
		// Метод можно оставить пустым или обновить данные, если LocalizationManager создается заново.
		// В текущей реализации данные берутся из noteData, который не зависит от локализации напрямую.
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
			TextAsset localizedTextFile = _noteData.NoteText_RU;

			if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
			{
				localizedTextFile = _noteData.NoteText_RU;
			}
			if (_localizationManager.CurrentLanguage == LanguagesEnum.English)
			{
				localizedTextFile = _noteData.NoteText_EN;
			}

			_textBackground.gameObject.SetActive(true);
			_textComponent.text = localizedTextFile.text;
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
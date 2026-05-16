using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionObjectNote : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	[SerializeField] private string _interactionObjectNameUI;
	public string InteractionObjectNameUI => _interactionObjectNameUI;

	// Заменяем поля на ссылку на ScriptableObject
	[SerializeField] private InteractionObjectNoteData noteData;

	private MenuManager _menuManager;
	private bool _isReading;

	public string InteractionHintMessageMain => $"Прочитать {InteractionObjectNameUI}";
	public string InteractionHintMessageFail => null;

	private GameObject _canvasNoteMenu;
	private Button _buttonExitNoteMenu;

	public string InteractionHintMessageAction { get; protected set; }
	private bool _isThereText;

	private RectTransform _imageRectTransform;
	private Image _backgroundBack;
	private TextMeshProUGUI _descriptionText;
	private Image _imageComponent;

	public bool IsInteractionHintMessageFailActive => false;
	private GameSceneManager _gameSceneManager;

	private void Start()
	{
		// Проверка наличия данных через ScriptableObject
		if (noteData == null)
		{
			Debug.LogError("TutorialNoteData не назначен в инспекторе для объекта " + gameObject.name);
			_isThereText = false;
			return; // Останавливаем инициализацию, если данных нет
		}

		_isThereText = (noteData.NoteText_RU != null && noteData.NoteText_EN != null);

		// Остальная инициализация остается прежней
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_buttonExitNoteMenu = ServiceLocator.Resolve<Button>("ButtonExitReadNoteMenu");
		_imageComponent = ServiceLocator.Resolve<Image>("ImageNote");
		_descriptionText = ServiceLocator.Resolve<TextMeshProUGUI>("TextNote");
		_backgroundBack = ServiceLocator.Resolve<Image>("ImageNoteBlackBackground");
		_canvasNoteMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuNote");

		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseAndDeactivate;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseAndDeactivate;

		var _localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_localizationManager.OnLanguageChangeEvent += ChangeLanguage;

		_imageRectTransform = _imageComponent.gameObject.GetComponent<RectTransform>();

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
		if (noteData == null) return; // Защита от NullReference

		_menuManager.OpenInteractionMenu();
		_isReading = true;

		_canvasNoteMenu.SetActive(true);

		// Используем данные из ScriptableObject напрямую
		_imageComponent.gameObject.SetActive(true);
		_imageComponent.sprite = noteData.NoteImage;

		if (_isThereText)
		{
			TextAsset localizedTextFile;
			var _localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

			localizedTextFile = (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
				? noteData.NoteText_RU
				: noteData.NoteText_EN;

			_backgroundBack.gameObject.SetActive(true);
			_descriptionText.text = localizedTextFile.text;

			// Позиция и поворот для случая с текстом (как в оригинале)
			_imageRectTransform.anchoredPosition = new Vector2(-184, -48);
			_imageRectTransform.localEulerAngles = new Vector3(0, 0, 11.5f);

		}
		else
		{
			// Позиция и поворот для случая без текста (как в оригинале)
			_imageRectTransform.anchoredPosition = new Vector2(0, 0);
			_imageRectTransform.localEulerAngles = new Vector3(0, 0, 0);
		}

		// Убедимся, что обработчик добавлен только один раз (опционально)
		//_buttonExitNoteMenu.onClick.AddListener(CloseAndDeactivate); 

		gameObject.tag = "Untagged";
	}

	private void CloseAndDeactivate()
	{
		if (_isReading)
		{
			_isReading = false;

			// Деактивируем объекты
			_backgroundBack.gameObject.SetActive(false);

			// Очищаем текст и изображение (не обязательно, но полезно для чистоты)
			_descriptionText.text = string.Empty;
			_imageComponent.sprite = null;

			// Закрываем меню и сбрасываем состояние кнопки (если нужно)
			_canvasNoteMenu.SetActive(false);
			_menuManager.CloseInteractionMenu();

			gameObject.tag = "Interactable";

			// Отписываемся от событий при выходе (опционально, для чистоты)
			if (_gameSceneManager != null)
			{
				_gameSceneManager.OnBeginLoadingMainMenuScene -= CloseAndDeactivate;
				_gameSceneManager.OnBeginLoadingGameplayScene -= CloseAndDeactivate;
				var _localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
				if (_localizationManager != null)
					_localizationManager.OnLanguageChangeEvent -= ChangeLanguage;

				if (_menuManager != null)
				{
					_menuManager.OnOpenPauseMenu -= HideNoteCanvas;
					_menuManager.OnClosePauseMenu -= ShowNoteCanvas;
				}
			}
		}
	}
}
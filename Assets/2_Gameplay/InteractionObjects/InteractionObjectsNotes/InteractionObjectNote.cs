using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionObjectNote : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	[SerializeField] private string _interactionObjectNameUI;

	private MenuManager _menuManager;
	private bool _IsReading;
	public string InteractionObjectNameUI => _interactionObjectNameUI;

	public string InteractionHintMessageMain => $"Прочитать {InteractionObjectNameUI}";
	[SerializeField] private TextAsset _textFile_RU;   // Русская версия текста
	[SerializeField] private TextAsset _textFile_EN;   // Английская версия текста

	private LocalizationManager _localizationManager;
	public string InteractionHintMessageAdditional => null;
	private GameObject _canvasNoteMenu;
	private Button _buttonExitNoteMenu;

	[SerializeField] private Sprite _image;

	public string InteractionHintAction { get; protected set; }
	private bool _isThereText;


	private RectTransform _imageRectTransform;
	private Image _backgroundBack;

	private TextMeshProUGUI _descriptionText;

	private Image _imageComponent;

	public bool IsInteractionHintMessageAdditionalActive => false;
	private GameSceneManager _gameSceneManager;
	private void Start()
	{
		// Разрешаем объекты по строке-ключу
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_buttonExitNoteMenu = ServiceLocator.Resolve<Button>("ButtonExitReadNoteMenu");     // Предполагаемый ключ
		_imageComponent = ServiceLocator.Resolve<Image>("ImageNote"); // Предполагаемый ключ
		_descriptionText = ServiceLocator.Resolve<TextMeshProUGUI>("TextNote");
		_backgroundBack = ServiceLocator.Resolve<Image>("ImageNoteBlackBackground");
		_canvasNoteMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuNote");
		//Debug.Log(ReadStructure);
		//menuManager.OnCloseReadNoteMenu += CloseAndDeactivate;
		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_gameSceneManager.OnBeginLoadMainMenuScene += CloseAndDeactivate;
		_gameSceneManager.OnBeginLoadGameplayScene += CloseAndDeactivate;
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		//saveLoadController.OnSafeFileLoad += CloseAndDeactivate;


		_imageRectTransform = _imageComponent.gameObject.GetComponent<RectTransform>();
		_localizationManager.OnLanguageChangeEvent += ChangeLanguage;



		_menuManager.OnOpenPauseMenu += HideNoteCanvas;
		_menuManager.OnClosePauseMenu += ShowNoteCanvas;

		if (_textFile_RU == null || _textFile_EN == null)
		{
			_isThereText = false;
		}
		else _isThereText = true;
		
	}

	public void ChangeLanguage()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
	}

	private void HideNoteCanvas()
	{
		if (_IsReading)
		{
			_canvasNoteMenu.SetActive(false);
		}
	}

	private void ShowNoteCanvas()
	{
		if (_IsReading)
		{
			_canvasNoteMenu.SetActive(true);
		}
	}

	public void Interact()
	{
		//Debug.Log(isThereText);
		_menuManager.OpenInteractionMenu();
		_IsReading = true;

		_canvasNoteMenu.SetActive(true);

		_imageComponent.gameObject.SetActive(true);
		_imageComponent.sprite = _image;

	

		

		
		// Включаем отображение текста из выбранного файла
		// Определяем, какой текстовый файл использовать в зависимости от текущего языка
		if (_isThereText)
		{
			TextAsset localizedTextFile;

			if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
			{
				localizedTextFile = _textFile_RU;
			}
			else
			{
				localizedTextFile = _textFile_EN;
			}

			_backgroundBack.gameObject.SetActive(true);
			_descriptionText.text = localizedTextFile.text;

			// Устанавливаем новую позицию
			_imageRectTransform.anchoredPosition = new Vector2(-184, -48);

			// Устанавливаем поворот по оси Z (в градусах)
			_imageRectTransform.localEulerAngles = new Vector3(0, 0, 11.5f);

		}
		else
		{
			// Устанавливаем новую позицию
			_imageRectTransform.anchoredPosition = new Vector2(0,0);

			// Устанавливаем поворот по оси Z (в градусах)
			_imageRectTransform.localEulerAngles = new Vector3(0, 0, 0);
		}

			// Берём текст из правильного файла


			// Заполняем текстовую область соответствующим текстом

			// Подписываемся на событие OnClick кнопки ExitButton
			_buttonExitNoteMenu.GetComponent<Button>().onClick.AddListener(CloseAndDeactivate);
		//ExitButton.gameObject.SetActive(true);

		gameObject.tag = "Untagged";
	}

	// Новый метод для закрытия меню и деактивации элементов
	private void CloseAndDeactivate()
	{
		if (_IsReading)
		{
			_IsReading = false;
			// Деактивируем объекты
			_backgroundBack.gameObject.SetActive(false);
			//ExitButton.gameObject.SetActive(false);
			_imageComponent.sprite = null;
			_descriptionText.text = null;
			//Закрываем меню
			_canvasNoteMenu.SetActive(false);
			_menuManager.CloseInteractionMenu();

			gameObject.tag = "Interactable";
		}
	}
}

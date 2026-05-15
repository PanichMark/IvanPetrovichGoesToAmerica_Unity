using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionObjectNote : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	[SerializeField] private string _interactionObjectNameUI;

	private MenuManager _menuManager;
	private bool _isReading;
	public string InteractionObjectNameUI => _interactionObjectNameUI;

	public string InteractionHintMessageMain => $"Прочитать {InteractionObjectNameUI}";
	[SerializeField] private TextAsset _textFile_RU;   // Русская версия текста
	[SerializeField] private TextAsset _textFile_EN;   // Английская версия текста

	private LocalizationManager _localizationManager;
	public string InteractionHintMessageFail => null;
	private GameObject _canvasNoteMenu;
	private Button _buttonExitNoteMenu;

	[SerializeField] private Sprite _image;

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
		_menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		_buttonExitNoteMenu = ServiceLocator.Resolve<Button>("ButtonExitReadNoteMenu");    
		_imageComponent = ServiceLocator.Resolve<Image>("ImageNote"); 
		_descriptionText = ServiceLocator.Resolve<TextMeshProUGUI>("TextNote");
		_backgroundBack = ServiceLocator.Resolve<Image>("ImageNoteBlackBackground");
		_canvasNoteMenu = ServiceLocator.Resolve<GameObject>("CanvasMenuNote");

		_gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		_gameSceneManager.OnBeginLoadingMainMenuScene += CloseAndDeactivate;
		_gameSceneManager.OnBeginLoadingGameplayScene += CloseAndDeactivate;
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

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
		_imageComponent.sprite = _image;

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

			_buttonExitNoteMenu.GetComponent<Button>().onClick.AddListener(CloseAndDeactivate);
		//ExitButton.gameObject.SetActive(true);

		gameObject.tag = "Untagged";
	}

	// Новый метод для закрытия меню и деактивации элементов
	private void CloseAndDeactivate()
	{
		if (_isReading)
		{
			_isReading = false;
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
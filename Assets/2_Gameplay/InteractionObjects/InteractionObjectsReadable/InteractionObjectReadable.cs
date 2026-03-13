using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionObjectReadable : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	[SerializeField] private string interactionObjectNameUI;

	private MenuManager menuManager;
	private bool IsReading;
	public string InteractionObjectNameUI => interactionObjectNameUI;

	public string InteractionHintMessageMain => $"Прочитать {InteractionObjectNameUI}";
	[SerializeField] private TextAsset textFile_RU;   // Русская версия текста
	[SerializeField] private TextAsset textFile_EN;   // Английская версия текста

	private LocalizationManager localizationManager;
	public string InteractionHintMessageAdditional => null;
	private GameObject canvasReadNoteMenu;
	private Button buttonExitReadNoteMenu;

	[SerializeField] private Sprite Image;

	public string InteractionHintAction { get; protected set; }
	private SaveLoadController saveLoadController;
	

	

	private Image ReadStructure;

	private TextMeshProUGUI descriptionText;

	private Image ImageComponent;

	public bool IsInteractionHintMessageAdditionalActive => false;
	private GameSceneManager gameSceneManager;
	private void Awake()
	{
		// Разрешаем объекты по строке-ключу
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		buttonExitReadNoteMenu = ServiceLocator.Resolve<Button>("ExitReadNote");     // Предполагаемый ключ
		ImageComponent = ServiceLocator.Resolve<Image>("ImageNewspaper"); // Предполагаемый ключ
		descriptionText = ServiceLocator.Resolve<TextMeshProUGUI>("ReadableText");
		ReadStructure = ServiceLocator.Resolve<Image>("BackgroundBlack");
		canvasReadNoteMenu = ServiceLocator.Resolve<GameObject>("CanvasReadNoteMenu");
		saveLoadController = ServiceLocator.Resolve<SaveLoadController>("SaveLoadController");
		//Debug.Log(ReadStructure);
		//menuManager.OnCloseReadNoteMenu += CloseAndDeactivate;
		gameSceneManager = ServiceLocator.Resolve<GameSceneManager>("GameSceneManager");
		gameSceneManager.OnBeginLoadMainMenuScene += CloseAndDeactivate;
		gameSceneManager.OnBeginLoadGameplayScene += CloseAndDeactivate;
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		//saveLoadController.OnSafeFileLoad += CloseAndDeactivate;
		localizationManager.OnLanguageChangeEvent += ChangeLanguage;

		menuManager.OnOpenPauseMenu += HideReadNoteCanvas;
		menuManager.OnClosePauseMenu += ShowReadNoteCanvas;

	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
	}

	private void HideReadNoteCanvas()
	{
		if (IsReading)
		{
			canvasReadNoteMenu.SetActive(false);
		}
	}

	private void ShowReadNoteCanvas()
	{
		if (IsReading)
		{
			canvasReadNoteMenu.SetActive(true);
		}
	}

	public void Interact()
	{
		menuManager.OpenInteractionMenu();
		IsReading = true;
		ReadStructure.gameObject.SetActive(true);

		
		ImageComponent.gameObject.SetActive(true);	
		ImageComponent.sprite = Image;

		TextAsset localizedTextFile;
		// Включаем отображение текста из выбранного файла
		// Определяем, какой текстовый файл использовать в зависимости от текущего языка
		if (localizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			localizedTextFile = textFile_RU;
		}
		else
		{
			localizedTextFile = textFile_EN;
		}

		// Берём текст из правильного файла


		// Заполняем текстовую область соответствующим текстом
		descriptionText.text = localizedTextFile.text;

		canvasReadNoteMenu.SetActive(true);
		// Подписываемся на событие OnClick кнопки ExitButton
		buttonExitReadNoteMenu.GetComponent<Button>().onClick.AddListener(CloseAndDeactivate);
		//ExitButton.gameObject.SetActive(true);

		gameObject.tag = "Untagged";
	}

	// Новый метод для закрытия меню и деактивации элементов
	private void CloseAndDeactivate()
	{
		if (IsReading)
		{
			IsReading = false;
			// Деактивируем объекты

			//ExitButton.gameObject.SetActive(false);
			ImageComponent.sprite = null;
			descriptionText.text = null;
			//Закрываем меню
			canvasReadNoteMenu.SetActive(false);
			menuManager.CloseInteractionMenu();

			gameObject.tag = "Interactable";
		}
	}
}

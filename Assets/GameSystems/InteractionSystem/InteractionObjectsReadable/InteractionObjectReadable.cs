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

	public string InteractionHintMessageAdditional => null;
	private GameObject canvasReadNoteMenu;
	private Button buttonExitReadNoteMenu;

	[SerializeField] private Sprite Image;

	public string InteractionHintAction { get; protected set; }
	private SaveLoadController saveLoadController;
	

	[SerializeField] private TextAsset textFile; // Поле для выбора текстового файла

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
		gameSceneManager.OnLoadMainMenuScene += CloseAndDeactivate;
		gameSceneManager.OnLoadGameplayScene += CloseAndDeactivate;

		//saveLoadController.OnSafeFileLoad += CloseAndDeactivate;


		menuManager.OnOpenPauseMenu += HideReadNoteCanvas;
		menuManager.OnClosePauseMenu += ShowReadNoteCanvas;

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
		menuManager.OpenReadNoteMenu();
		IsReading = true;
		ReadStructure.gameObject.SetActive(true);

		
		ImageComponent.gameObject.SetActive(true);	
		ImageComponent.sprite = Image;

		// Включаем отображение текста из выбранного файла
		
		
	    descriptionText.text = textFile.text;

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
			menuManager.CloseReadNoteMenu();

			gameObject.tag = "Interactable";
		}
	}
}

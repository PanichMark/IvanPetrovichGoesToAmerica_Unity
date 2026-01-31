using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionObjectReadable : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	[SerializeField] private string interactionObjectNameUI;

	private MenuManager menuManager;

	public string InteractionObjectNameUI => interactionObjectNameUI;

	public string MainInteractionHint => $"Прочитать {InteractionObjectNameUI}";

	public string AdditionalInteractionHint => null;
	private GameObject canvasReadNoteMenu;
	private Button buttonExitReadNoteMenu;

	[SerializeField] private Sprite Image;

	

	

	[SerializeField] private TextAsset textFile; // Поле для выбора текстового файла

	private Image ReadStructure;

	private TextMeshProUGUI descriptionText;

	private Image ImageComponent;

	public bool IsAdditionalInteractionHintActive => false;

	private void Awake()
	{
		// Разрешаем объекты по строке-ключу
		menuManager = ServiceLocator.Resolve<MenuManager>("MenuManager");
		buttonExitReadNoteMenu = ServiceLocator.Resolve<Button>("ExitReadNote");     // Предполагаемый ключ
		ImageComponent = ServiceLocator.Resolve<Image>("ImageNewspaper"); // Предполагаемый ключ
		descriptionText = ServiceLocator.Resolve<TextMeshProUGUI>("ReadableText");
		ReadStructure = ServiceLocator.Resolve<Image>("BackgroundBlack");
		canvasReadNoteMenu = ServiceLocator.Resolve<GameObject>("CanvasReadNoteMenu");
		//Debug.Log(ReadStructure);
		menuManager.OnCloseReadNoteMenu += CloseAndDeactivate;

	}

	
	public void Interact()
	{
		menuManager.OpenInteractionMenu();

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

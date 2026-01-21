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

	private Button ExitButton;

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
		ExitButton = ServiceLocator.Resolve<Button>("ExitInteraction");     // Предполагаемый ключ
		ImageComponent = ServiceLocator.Resolve<Image>("ImageNewspaper"); // Предполагаемый ключ
		descriptionText = ServiceLocator.Resolve<TextMeshProUGUI>("ReadableText");
		ReadStructure = ServiceLocator.Resolve<Image>("BackgroundBlack");
		//Debug.Log(ReadStructure);
		
	}
	public void Interact()
	{
		menuManager.OpenInteractionMenu();

		ReadStructure.gameObject.SetActive(true);

		
		ImageComponent.gameObject.SetActive(true);	
		ImageComponent.sprite = Image;

		// Включаем отображение текста из выбранного файла
		
		
	    descriptionText.text = textFile.text;
		

		// Подписываемся на событие OnClick кнопки ExitButton
		ExitButton.GetComponent<Button>().onClick.AddListener(CloseAndDeactivate);
		ExitButton.gameObject.SetActive(true);

		gameObject.tag = "Untagged";
	}

	// Новый метод для закрытия меню и деактивации элементов
	private void CloseAndDeactivate()
	{
		// Деактивируем объекты

		ExitButton.gameObject.SetActive(false);
		ImageComponent.sprite = null;
		descriptionText.text = null;
		ImageComponent.gameObject.SetActive(false);
		ReadStructure.gameObject.SetActive(false);
		//Закрываем меню
		menuManager.CloseInteractionMenu();

		gameObject.tag = "Interactable";
	}
}

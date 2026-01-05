using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadableObject : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => null;

	[SerializeField] private string interactionObjectNameUI;

	[SerializeField] private MenuManager menuManager;

	public string InteractionObjectNameUI => interactionObjectNameUI;

	public string MainInteractionHint => $"Прочитать {InteractionObjectNameUI}";

	public string AdditionalInteractionHint => null;

	[SerializeField] private GameObject ExitButton;

	[SerializeField] private Sprite Image;

	[SerializeField] private GameObject ImageRead;

	[SerializeField] private GameObject DescriptionTextGameobject; // Игровой объект с компонентом TextMeshProUGUI

	[SerializeField] private TextAsset textFile; // Поле для выбора текстового файла

	[SerializeField] private GameObject ReadStructure;

	private TextMeshProUGUI descriptionText;

	private Image ImageComponent;

	public bool IsAdditionalInteractionHintActive => false;

	public void Interact()
	{
		menuManager.OpenInteractionMenu();

		ReadStructure.SetActive(true);
		

		ImageComponent = ImageRead.GetComponent<Image>();
		ImageComponent.sprite = Image;

		// Включаем отображение текста из выбранного файла
		
		descriptionText = DescriptionTextGameobject.GetComponent<TextMeshProUGUI>();
		descriptionText.text = textFile.text;
		

		// Подписываемся на событие OnClick кнопки ExitButton
		ExitButton.GetComponent<Button>().onClick.AddListener(CloseAndDeactivate);
	}

	// Новый метод для закрытия меню и деактивации элементов
	private void CloseAndDeactivate()
	{
		// Деактивируем объекты
		

		ImageComponent.sprite = null;
		descriptionText.text = null;

		ReadStructure.SetActive(false);
		// Закрываем меню
		menuManager.CloseInteractionMenu();
	}
}
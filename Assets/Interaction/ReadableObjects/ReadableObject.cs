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

	[SerializeField] private GameObject DescriptionText; // Игровой объект с компонентом TextMeshProUGUI

	[SerializeField] private TextAsset textFile; // Поле для выбора текстового файла

	private Image ImageComponent;

	public bool IsAdditionalInteractionHintActive => false;

	public void Interact()
	{
		menuManager.OpenInteractionMenu();

		ExitButton.SetActive(true);
		ImageRead.SetActive(true);
		DescriptionText.SetActive(true); // Включаем отображение текста

		ImageComponent = ImageRead.GetComponent<Image>();
		ImageComponent.sprite = Image;

		// Включаем отображение текста из выбранного файла
		if (textFile != null && !string.IsNullOrEmpty(textFile.text))
		{
			var descriptionText = DescriptionText.GetComponent<TextMeshProUGUI>();
			descriptionText.text = textFile.text;
		}
		else
		{
			Debug.LogWarning("Текстовый файл не выбран или пуст.");
		}

		// Подписываемся на событие OnClick кнопки ExitButton
		ExitButton.GetComponent<Button>().onClick.AddListener(CloseAndDeactivate);
	}

	// Новый метод для закрытия меню и деактивации элементов
	private void CloseAndDeactivate()
	{
		// Деактивируем объекты
		ExitButton.SetActive(false);
		ImageRead.SetActive(false);
		DescriptionText.SetActive(false); // Скрываем текст

		// Закрываем меню
		menuManager.CloseInteractionMenu();
	}
}
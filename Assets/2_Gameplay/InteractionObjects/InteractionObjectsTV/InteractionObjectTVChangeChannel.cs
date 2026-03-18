using UnityEngine;

public class InteractionObjectTVButtonChannel : MonoBehaviour, IInteractable
{
	[SerializeField] private string buttonChannelUI;

	// 1. Добавляем ссылку на контроллер телевизора
	[SerializeField] private TVController tvController;

	public string InteractionObjectNameSystem => "buttonChannel";

	public string InteractionObjectNameUI => buttonChannelUI;

	public string InteractionHintMessageMain => $"Нажать {buttonChannelUI}?";

	[SerializeField] bool IsNextChannel;

	// Убираем заглушки, если они не нужны для работы интерфейса
	public string InteractionHintAction => "Переключить";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;

	// 2. Реализуем логику взаимодействия
	public void Interact()
	{
		// Проверяем, что ссылка на контроллер назначена в инспекторе
		if (tvController != null)
		{
			// Вызываем метод переключения на контроллере,
			// передавая ему направление (вперед/назад)
			tvController.SwitchChannel(IsNextChannel);
		}
		else
		{
			// Если ссылка не указана, выводим ошибку в консоль,
			// чтобы вы не ломали голову, почему ничего не работает.
			Debug.LogError("Ошибка: Ссылка на TVController не указана в кнопке " + gameObject.name);
		}
	}

	void Start()
	{

	}

	void Update()
	{

	}
}
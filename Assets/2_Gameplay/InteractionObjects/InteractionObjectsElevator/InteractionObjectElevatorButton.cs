using UnityEngine;

public class InteractionObjectElevatorButton : MonoBehaviour, IInteractable
{
	// Ссылка на контроллер лифта. Перетаскиваем объект лифта из иерархии сюда.
	[SerializeField] private InteractionObjectElevatorController elevatorController;

	// Направление для этой конкретной кнопки. Галочка в инспекторе.
	[SerializeField] private bool isButtonUp;

	// --- Реализация интерфейса IInteractable ---

	public string InteractionObjectNameSystem => "Кнопка лифта";
	public string InteractionObjectNameUI => "Кнопка";
	public string InteractionHintMessageMain => "Нажмите, чтобы вызвать лифт";
	public string InteractionHintAction => "Нажать";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;

	// Этот метод вызывается при взаимодействии с объектом (например, нажатии E)
	public void Interact()
	{
		// Проверяем, подключен ли контроллер в инспекторе
		if (elevatorController == null)
		{
			Debug.LogError("Elevator Controller не назначен на кнопке!", this);
			return;
		}

		// Отправляем команду контроллеру: "Едь вверх?" (значение isButtonUp)
		elevatorController.RequestMove(isButtonUp);
	}
}
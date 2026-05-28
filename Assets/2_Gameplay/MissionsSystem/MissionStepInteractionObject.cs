// Файл: QuestInteractiveObject.cs (в сборке Gameplay)
using UnityEngine;

public class MissionStepInteractionObject : MonoBehaviour
{
	// В это поле мы будем перетаскивать нужный нам ассет условия из Project Browser

	private IInteractable _interactable;
	public MissionStepConditionAbstract linkedMissionCondition;

	private void Awake()
	{
		// При старте объекта, если у него есть связанное условие,
		// он регистрирует сам себя в этом условии.

		linkedMissionCondition.RegisterOwner(this.gameObject);
		Debug.Log($"{gameObject.name} зарегистрировал себя в условии {linkedMissionCondition.name}");

		_interactable = GetComponent<IInteractable>();
		_interactable.OnInteract += TriggerInteraction;
	//	Debug.Log(_interactable);
	}

	// Этот метод будет вызываться вашим основным скриптом взаимодействия,
	// например, когда игрок нажимает кнопку "Использовать"
	public void TriggerInteraction()
	{
		if (linkedMissionCondition is MissionStepConditionOnInteraction interactionCondition)
		{
			// Передаем в условие информацию о том, кто взаимодействовал.
			// В данном случае это сам владелец (this.gameObject).
			interactionCondition.OnPlayerInteracted(this.gameObject);
		}
	}
}
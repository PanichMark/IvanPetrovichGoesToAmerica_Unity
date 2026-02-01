using UnityEngine;
using System;
using Unity.IO.LowLevel.Unsafe;

public abstract class InteractionObjectOpenableAbstract : MonoBehaviour, IInteractable
{
	[SerializeField]
	private string _interactionItemNameSystem;
	public virtual string InteractionObjectNameSystem => _interactionItemNameSystem;
	// Приватное поле, видимое в инспекторе
	[SerializeField]
	private string _interactionItemName;
	public virtual string InteractionObjectNameUI => _interactionItemName;

	// Свойство подсказки теперь учитывает состояние двери
	public virtual string MainInteractionHintMessage => !IsDoorOpened ? $"Открыть {InteractionObjectNameUI}" : $"Закрыть {InteractionObjectNameUI}";
	public virtual string AdditionalInteractionHintMessage => null;
	public virtual bool IsAdditionalInteractionHintMessageActive => false;

	public virtual bool IsDoorOpened { get; protected set; }


	public int DoorIndex { get; protected set; }

	

	public abstract void Interact();


}


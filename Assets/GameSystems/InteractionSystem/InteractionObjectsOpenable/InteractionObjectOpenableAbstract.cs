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
	protected string interactionHintMessageMain;
	public virtual string InteractionHintMessageMain => interactionHintMessageMain;
	public virtual string InteractionHintMessageAdditional => null;
	public virtual bool IsInteractionHintMessageAdditionalActive => false;

	public virtual bool IsDoorOpened { get; protected set; }
	public string InteractionHintAction { get; protected set; }

	public int DoorIndex { get; protected set; }

	

	public abstract void Interact();


}


using UnityEngine;
using System;


public abstract class InteractionObjectOpenableAbstract : MonoBehaviour, IInteractable
{

	[SerializeField] protected string interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => interactionObjectNameSystem;
	// Приватное поле, видимое в инспекторе
	private LocalizationManager localizationManager;
	public virtual string InteractionObjectNameUI { get; protected set; }

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


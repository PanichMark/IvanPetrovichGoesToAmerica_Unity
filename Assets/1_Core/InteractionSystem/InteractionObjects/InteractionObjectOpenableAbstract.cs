using UnityEngine;
using System;

public abstract class InteractionObjectOpenableAbstract : MonoBehaviour, IInteractable
{
	public delegate void DoorStateChangedHandler(bool isOpened);
	public event DoorStateChangedHandler OnDoorStateChanged;

	[SerializeField] protected string interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => interactionObjectNameSystem;

	private LocalizationManager localizationManager;
	public virtual string InteractionObjectNameUI { get; protected set; }

	protected string interactionHintMessageMain;
	public virtual string InteractionHintMessageMain => interactionHintMessageMain;
	public virtual string InteractionHintMessageAdditional => null;
	public virtual bool IsInteractionHintMessageAdditionalActive => false;

	public virtual bool IsDoorOpened { get; protected set; }
	public string InteractionHintAction { get; protected set; }

	public int DoorIndex { get; protected set; }

	public abstract void Interact();
}
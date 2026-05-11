using UnityEngine;

public delegate void DoorStateChangedHandler(bool isOpened);

public abstract class InteractionObjectOpenableAbstract : MonoBehaviour, IInteractable
{
	public event DoorStateChangedHandler OnDoorStateChanged;

	[SerializeField] protected string _interactionObjectNameSystem;

	protected LocalizationManager _localizationManager;

	protected string _interactionHintMessageMain;

	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI { get; protected set; }
	public virtual string InteractionHintMessageMain => _interactionHintMessageMain;
	public virtual string InteractionHintMessageFail => null;
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction { get; protected set; }

	public virtual bool IsDoorOpened { get; protected set; }

	public int DoorIndex { get; protected set; }

	public abstract void Interact();
}
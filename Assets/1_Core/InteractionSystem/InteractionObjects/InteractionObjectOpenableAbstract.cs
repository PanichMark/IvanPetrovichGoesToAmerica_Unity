using UnityEngine;

public abstract class InteractionObjectOpenableAbstract : MonoBehaviour, IInteractable
{

	[SerializeField] protected string _interactionObjectNameSystem;

	protected LocalizationManager _localizationManager;

	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI => null;
	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public virtual string InteractionHintMessageFail => null;
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction { get; protected set; }

	public virtual bool IsObjectOpened { get; protected set; }

	public int OpenableObjectIndex { get; protected set; }

	public abstract void Interact();
}
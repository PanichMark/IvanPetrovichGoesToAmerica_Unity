using UnityEngine;

public abstract class InteractionObjectOpenableAbstract : GameplayObjectJsonSaveLoad, IInteractable
{
	[SerializeField] protected string _interactionObjectNameSystem;

	protected LocalizationManager _localizationManager;

	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI => null;
	public virtual string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public virtual string InteractionHintMessageFail => null;
	public bool IsOpenableUnlocked { get; protected set; }
	public virtual bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction { get; protected set; }

	[SerializeField] protected bool _isObjectOpened;
	public virtual bool IsObjectOpened => _isObjectOpened;
	protected Coroutine _currentAnimation;

	public event IInteractable.InteractableObjectHandler OnInteract;
	public abstract void Interact();

	public abstract void InteractCutscene();
}
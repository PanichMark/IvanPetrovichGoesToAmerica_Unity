using UnityEngine;

public class InteractionObjectLockAbstract : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => _interactionObjectNameSystem;

	protected string _interactionHintMessageAction;
	protected string _interactionHintMessageMain;
	public string InteractionHintMessageMain => _interactionHintMessageMain;

	[SerializeField] protected string _interactionObjectNameSystem;
	public string InteractionHintMessageFail => null;
	public delegate void UnlockLockEventHandler();
	public event UnlockLockEventHandler OnUnlockLock;
	public bool IsInteractionHintMessageFailActive => false;

	public string InteractionObjectNameUI { get; protected set; }

	public string InteractionHintMessageAction => _interactionHintMessageAction;

	public event IInteractable.InteractableObjectHandler OnInteract;

	protected void SendLockPickedSignal()
	{
		OnUnlockLock?.Invoke();
	}

	public virtual void Interact()
	{
		
	}

	public virtual void InteractAfterDialogueOrCutscene()
	{
		
	}
}

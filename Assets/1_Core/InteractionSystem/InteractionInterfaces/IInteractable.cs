public interface IInteractable
{
	string InteractionObjectNameSystem { get; }
	string InteractionObjectNameUI { get; }
	string InteractionHintMessageMain { get; }
	string InteractionHintMessageAction { get; }
	string InteractionHintMessageFail { get; }
	bool IsInteractionHintMessageFailActive { get; }

	void Interact();
}
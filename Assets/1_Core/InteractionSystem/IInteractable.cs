public interface IInteractable
{
	string InteractionObjectNameSystem { get; }
	string InteractionObjectNameUI { get; }
	string InteractionHintMessageMain { get; }
	string InteractionHintAction { get; }
	string InteractionHintMessageAdditional { get; }
	bool IsInteractionHintMessageAdditionalActive { get; }

	void Interact();
}
public interface IInteractable
{
	string InteractionObjectNameSystem { get; }
	string InteractionObjectNameUI { get; }
	string MainInteractionHintMessage { get; }
	string AdditionalInteractionHintMessage { get; }
	bool IsAdditionalInteractionHintMessageActive { get; }

	void Interact();
}
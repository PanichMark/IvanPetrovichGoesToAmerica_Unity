public interface IInteractable
{
	string InteractionObjectNameSystem { get; }
	string InteractionObjectNameUI { get; }
	string MainInteractionHint { get; }

	string AdditionalInteractionHint { get; }
	bool IsAdditionalInteractionHintActive { get; }


	void Interact();
}
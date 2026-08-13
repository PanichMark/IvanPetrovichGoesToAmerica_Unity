public class NPCaggresive : NPCneutral
{
	public override bool IsInteractionHintMessageFailActive => false;

	override public void Interact()
	{
		// Cant talk to at all
	}
}
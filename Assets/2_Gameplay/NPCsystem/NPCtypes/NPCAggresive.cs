public class NPCAggresive : NPCNeutral
{
	public override bool IsInteractionHintMessageFailActive => false;

	override public void Interact()
	{
		// Cant talk to at all
	}
}
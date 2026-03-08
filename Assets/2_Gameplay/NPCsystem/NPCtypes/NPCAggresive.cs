using UnityEngine;

public class NPCAggresive : NPCNeutral
{
	public override bool IsInteractionHintMessageAdditionalActive => false;

	override public void Interact()
	{
	
		// Cant talk to at all
		
	}
}
using UnityEngine;

public class NPCNeutral : NPCAbstract
{
	public override void Interact()
	{
		if (_npcStateMachineController?.CurrentNPCState != "StationaryAction" &&
			_npcStateMachineController?.CurrentNPCState != "Patrolling")
			return;

		if (_NPCDialogueController.RussianDialogueFile != null &&
			_NPCDialogueController.EnglishDialogueFile != null)
		{
			_npcStateMachineController.RotateTowardsPlayer();
			_NPCDialogueController.Interact();
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(ShowAndHidePhrase());
		}
	}
}
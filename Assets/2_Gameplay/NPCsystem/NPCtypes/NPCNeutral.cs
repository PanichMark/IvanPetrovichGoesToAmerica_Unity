public class NPCNeutral : NPCAbstract
{
	public override void Interact()
	{
		if (_npcStateMachineController?.CurrentNPCState != "StationaryAction" && _npcStateMachineController?.CurrentNPCState != "Patrolling")
			return;

		if (_NPCdialogueController.RussianDialogueFile != null && _NPCdialogueController.EnglishDialogueFile != null)
		{
			_npcStateMachineController.RotateTowardsPlayer();
			_NPCdialogueController.Interact();
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(ShowAndHidePhrase());
		}
	}
}
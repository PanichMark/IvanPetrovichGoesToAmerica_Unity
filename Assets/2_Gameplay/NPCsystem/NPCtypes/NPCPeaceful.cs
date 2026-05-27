public class NPCPeaceful : NPCAbstract
{
	public override void Interact()
	{
		if (_npcStateMachineController?.CurrentNPCState != "StationaryAction" &&
			_npcStateMachineController?.CurrentNPCState != "Patrolling")
			return;

		if (_NPCdialogueController.NPCdialogueData != null)
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
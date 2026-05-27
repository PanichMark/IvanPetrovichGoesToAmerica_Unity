public class NPCNeutral : NPCAbstract
{
	public override void Interact()
	{
		if (_NPCstateMachineController?.CurrentNPCState != "StationaryAction" && _NPCstateMachineController?.CurrentNPCState != "Patrolling")
			return;

		if (_NPCdialogueController != null)
		{
			_NPCstateMachineController.RotateTowardsPlayer();
			_NPCdialogueController.Interact();
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(_NPCphrasesController.TemporaryShowPhrases());
		}
	}
}
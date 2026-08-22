using UnityEngine;

[RequireComponent(typeof(NPCphrasesController))]

public class NPCneutral : NPCabstract
{
	public override void Interact()
	{
		if (_NPCstateMachineController?.CurrentNPCState != NPCstateTypes.StationaryAction
			&& _NPCstateMachineController?.CurrentNPCState != NPCstateTypes.Patrolling)
			return;

		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Interact();
		}
		else
		{
			StopAllCoroutines();
			_NPCphrasesController.TemporaryShowPhrases();
		}
	}
}
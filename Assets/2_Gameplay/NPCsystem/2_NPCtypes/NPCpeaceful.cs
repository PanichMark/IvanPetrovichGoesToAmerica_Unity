using UnityEngine;

[RequireComponent(typeof(NPCphrasesController))]

public class NPCpeaceful : NPCabstract
{
	public override void Interact()
	{
		if (_NPCstateMachineController?.CurrentNPCState != "StationaryAction" &&
			_NPCstateMachineController?.CurrentNPCState != "Patrolling")
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
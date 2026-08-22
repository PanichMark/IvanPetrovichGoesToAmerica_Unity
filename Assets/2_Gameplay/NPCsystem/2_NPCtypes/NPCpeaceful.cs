using UnityEngine;

[RequireComponent(typeof(NPCphrasesController))]

public class NPCpeaceful : NPCabstract
{
	public override void Interact()
	{
		//Debug.Log("NPC interact");

		if (_NPCstateMachineController.CurrentNPCState == NPCstateTypes.Dead)
		{
			_pickable.Interact();
			return;
		}

		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.StationaryAction &&
			_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Patrolling)
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
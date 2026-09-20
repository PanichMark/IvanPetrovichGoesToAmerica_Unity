using UnityEngine;

public class NPCstateHooked : NPCstateAbstract
{
	public NPCstateHooked(NPCstateMachineController npcStateMachineController, NPCmovementController NPCmovementController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCmovementController = NPCmovementController;
		_NPCmovementController.StopAnchorMove();
		_NPCmovementController.TurnNavmeshOff();
	}

	/* 
* НЕПОНЯТНО!
*/
}

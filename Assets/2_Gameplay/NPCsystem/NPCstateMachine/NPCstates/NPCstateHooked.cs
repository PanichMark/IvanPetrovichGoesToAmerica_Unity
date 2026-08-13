using UnityEngine;

public class NPCstateHooked : NPCstateAbstract
{
	public NPCstateHooked(NPCstateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
	}
}

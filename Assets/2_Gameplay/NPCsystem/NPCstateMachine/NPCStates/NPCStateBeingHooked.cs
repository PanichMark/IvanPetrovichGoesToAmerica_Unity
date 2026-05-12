using UnityEngine;

public class NPCStateBeingHooked : NPCStateAbstract
{
	public NPCStateBeingHooked(NPCStateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
	}
}

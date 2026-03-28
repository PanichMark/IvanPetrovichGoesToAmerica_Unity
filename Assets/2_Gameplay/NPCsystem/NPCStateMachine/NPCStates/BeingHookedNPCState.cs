using UnityEngine;

public class BeingHookedNPCState : AbstractNPCState
{
	public BeingHookedNPCState(NPCStateMachineController npcStateMachineController)
	{
		this._NPCStateMachineController = npcStateMachineController;
		//_NPCStateMachineController.StopRandomMove();
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();

	}
}

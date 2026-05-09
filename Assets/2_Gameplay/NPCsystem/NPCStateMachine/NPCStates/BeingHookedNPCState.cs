using UnityEngine;

public class BeingHookedNPCState : AbstractNPCState
{
	public BeingHookedNPCState(NPCStateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
	}
}

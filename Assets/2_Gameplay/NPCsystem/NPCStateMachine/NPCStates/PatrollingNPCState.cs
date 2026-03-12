using UnityEngine;

public class PatrollingNPCState : AbstractNPCState
{
	public PatrollingNPCState(NPCStateMachineController npcStateMachineController)
	{
		this._NPCStateMachineController = npcStateMachineController;
		//_NPCStateMachineController.StartRandomMove();
		_NPCStateMachineController.StartAnchorMove();
	}


}
using UnityEngine;

public class DefaultNPCState : AbstractNPCState
{
	public DefaultNPCState(NPCStateMachineController npcStateMachineController)
	{
		this._NPCStateMachineController = npcStateMachineController;
		//_NPCStateMachineController.StartRandomMove();
		_NPCStateMachineController.StartAnchorMove();
	}


}
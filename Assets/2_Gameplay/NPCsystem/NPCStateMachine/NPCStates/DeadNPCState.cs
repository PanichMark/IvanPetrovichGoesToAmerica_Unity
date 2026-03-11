
public class DeadNPCState : AbstractNPCState
{
	public DeadNPCState(NPCStateMachineController npcStateMachineController)
	{
		this._NPCStateMachineController = npcStateMachineController;
		//_NPCStateMachineController.StopRandomMove();
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
	}
}
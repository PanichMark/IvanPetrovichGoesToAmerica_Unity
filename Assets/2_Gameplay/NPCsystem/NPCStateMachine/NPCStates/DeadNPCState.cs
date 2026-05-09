public class DeadNPCState : AbstractNPCState
{
	public DeadNPCState(NPCStateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
	}
}
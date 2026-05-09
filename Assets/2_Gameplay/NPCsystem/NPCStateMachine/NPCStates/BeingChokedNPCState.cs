public class BeingChokedNPCState : AbstractNPCState
{
	public BeingChokedNPCState(NPCStateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
	}
}


public class ChokedNPCState : AbstractNPCState
{
	public ChokedNPCState(NPCStateMachineController npcStateMachineController)
	{
		this._NPCStateMachineController = npcStateMachineController;
		//_NPCStateMachineController.StopRandomMove();
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();

	}
}

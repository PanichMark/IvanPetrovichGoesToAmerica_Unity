public class NPCstateDead : NPCstateAbstract
{
	public NPCstateDead(NPCstateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
	}
}
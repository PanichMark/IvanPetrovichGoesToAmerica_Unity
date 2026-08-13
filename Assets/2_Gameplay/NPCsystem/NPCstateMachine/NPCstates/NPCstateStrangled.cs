public class NPCstateStrangled : NPCstateAbstract
{
	public NPCstateStrangled(NPCstateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
	}
}

public class NPCstateDead : NPCstateAbstract
{
	public NPCstateDead(NPCmovementController NPCmovementController)
	{
		_NPCmovementController = NPCmovementController;
		_NPCmovementController.StopAnchorMove();
		_NPCmovementController.TurnNavmeshOff();
	}
}
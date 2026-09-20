public class NPCstateStrangled : NPCstateAbstract
{
	public NPCstateStrangled(NPCstateMachineController npcStateMachineController, NPCmovementController NPCmovementController)
	{
		_NPCmovementController = NPCmovementController;
		_NPCStateMachineController = npcStateMachineController;
		_NPCmovementController.StopAnchorMove();
		_NPCmovementController.TurnNavmeshOff();
	}

	/*
 *	NPC в процессе удушения
 */

	/* 
 * Peaceful Neutral Aggresive
 * 4) если отпустили кнопку в процессе удушения то Alarmed
 * 
 * 22)Если додушили то Unconscious
 */
}

public class NPCstateStrangled : NPCstateAbstract
{
	public NPCstateStrangled(NPCstateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StopAnchorMove();
		_NPCStateMachineController.TurnNavmeshOff();
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

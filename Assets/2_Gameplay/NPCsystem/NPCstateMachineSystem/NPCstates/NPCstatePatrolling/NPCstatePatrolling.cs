public class NPCstatePatrolling : NPCstateAbstract
{
	public NPCstatePatrolling(NPCstateMachineController npcStateMachineController, NPCmovementController NPCmovementController)
	{
		_NPCmovementController = NPCmovementController;
		_NPCStateMachineController = npcStateMachineController;
		_NPCmovementController.TurnNavmeshOn();
		_NPCmovementController.StartAnchorMove();
	}

	public override void Update()
	{
		foreach (var config in _NPCmovementController.AnchorData)
		{
			var triggerPoint = config.NPCanchorPoint;

			if (_NPCmovementController.GetLastVisitedStopPoint() == triggerPoint)
				continue;

			if (_NPCmovementController.IsAtPosition(triggerPoint.transform.position))
			{
				_NPCmovementController.SetLastVisitedStopPoint(triggerPoint);
				_NPCStateMachineController.SetNPCState(NPCstateTypes.StationaryAction);
			}
		}
	}

	/*
	 *	ходим между AnchorPoint
	 */


	/*
	 * Peaceful Neutral Aggresive
	 * 1) если дошли до точки где нужно стоять то StationaryAction
	 * 
	 * Peaceful Neutral Aggresive
	 * 3) если тревога дошла до 50 но не до 100 сразу то Intereset
	 * 
	 * Peaceful Neutral Aggresive
	 * 4) если тревога дошла до 100 то Alarmed
	 * 
	 * Peaceful Neutral Aggresive
	 * 12) если начали душить то Strangled
	 * 
	 * Peaceful Neutral Aggresive
	 * 13) если подцепили арбалетом то Hooked
	 * 
	 * Peaceful Neutral Aggresive
	 * 14) если выстрелили ногой то Staggered
	 * 
	 * Peaceful Neutral Aggresive
	 * 15) если толкнули ногой то KnockedOff
	 * 
	 * Peaceful Neutral Aggresive
	 * 16) если Дыхание Джина то BlownAway
	 * 
	 * Peaceful Neutral Aggresive
	 * 17) если ИскраТеслы то ElectroShocked
	 * 
	 * Peaceful Neutral Aggresive
	 * 18) если под ногами нет земли то Falling
	 * 
	 * Peaceful Neutral Aggresive
	 * 20) если транквилизатор то Dizzy
	 * 
	 * Peaceful Neutral Aggresive
	 * 23) если убили с 1 удара то Dead
	 */
}
public class NPCStatePatrolling : NPCStateAbstract
{
	public NPCStatePatrolling(NPCStateMachineController npcStateMachineController)
	{
		_NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.TurnNavmeshOn();
		_NPCStateMachineController.StartAnchorMove();
	}

	public override void Update()
	{
		foreach (var config in _NPCStateMachineController.AnchorData)
		{
			var triggerPoint = config.AnchorPoint;

			if (_NPCStateMachineController.GetLastVisitedStopPoint() == triggerPoint)
				continue;

			if (_NPCStateMachineController.IsAtPosition(triggerPoint.transform.position))
			{
				_NPCStateMachineController.SetLastVisitedStopPoint(triggerPoint);
				_NPCStateMachineController.SetNPCState(
					NPCStateTypes.StationaryAction,
					config.NPCwaitDuration
				);
			}
		}
	}
}
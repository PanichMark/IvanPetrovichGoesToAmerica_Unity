using UnityEngine;

public class PatrollingNPCState : AbstractNPCState
{
	public PatrollingNPCState(NPCStateMachineController npcStateMachineController)
	{
		this._NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StartAnchorMove();

	}

	public override void Update()
	{
		foreach (var config in _NPCStateMachineController.StopConfigs)
		{
			// Получаем текущую анкорную точку
			var triggerPoint = config.anchorPoint;
			//Debug.Log($"TRIGGGER: {triggerPoint}");
			// Игнорируем последнюю посещённую точку
			if (_NPCStateMachineController.GetLastVisitedStopPoint() == triggerPoint)
				continue;

			if (_NPCStateMachineController.IsAtPosition(triggerPoint.transform.position))
			{
				_NPCStateMachineController.SetLastVisitedStopPoint(triggerPoint);
				// Переход в стационарное состояние
				_NPCStateMachineController.SetNPCState(
					NPCStateTypes.StationaryAction,
					config.waitDuration
				);

				// Запоминаем текущую точку как последнюю посещённую
				
	
			}
		}
	}
}
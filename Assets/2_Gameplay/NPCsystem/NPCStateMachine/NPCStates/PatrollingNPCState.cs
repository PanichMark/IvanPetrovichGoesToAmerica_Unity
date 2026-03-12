using UnityEngine;

public class PatrollingNPCState : AbstractNPCState
{
	public PatrollingNPCState(NPCStateMachineController npcStateMachineController)
	{
		this._NPCStateMachineController = npcStateMachineController;
		_NPCStateMachineController.StartAnchorMove();
		Debug.Log("PATROLLING");
	}

	public override void Update()
	{
		foreach (var config in _NPCStateMachineController.StopConfigs)
		{
			// Берём анкорную точку непосредственно из структуры
			var triggerPoint = config.anchorPoint;

			if (_NPCStateMachineController.IsAtPosition(triggerPoint.transform.position))
			{
			
															 // Переход в стационарное состояние
				_NPCStateMachineController.SetNPCState(NPCStateTypes.StationaryAction, config.waitDuration);

	                     // Прерываем цикл после успешного перехода
			}
		}
	}
}
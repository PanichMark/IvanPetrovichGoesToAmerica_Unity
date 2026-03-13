using UnityEngine;

public class StationaryActionNPCState : AbstractNPCState
{
	private float timer;
	private float animationDuration;

	public StationaryActionNPCState(NPCStateMachineController npcStateMachineController, float animationDuration)
	{
		this._NPCStateMachineController = npcStateMachineController;
		
		this.animationDuration = animationDuration;
		timer = 0f;
		_NPCStateMachineController.StopAnchorMove(); // Останавливаем патрулирование

	
	}

	public override void Update()
	{
		if (_NPCStateMachineController.AnchorPoints.Count != 0)
		{
			timer += Time.deltaTime;

			if (timer >= animationDuration)
			{
				// Возврат в патрульное состояние
				_NPCStateMachineController.SetNPCState(NPCStateTypes.Patrolling);
			}
		}

		
	}
}

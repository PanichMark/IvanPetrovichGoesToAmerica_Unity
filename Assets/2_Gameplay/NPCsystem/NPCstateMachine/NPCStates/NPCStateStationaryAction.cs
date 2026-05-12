using UnityEngine;

public class NPCStateStationaryAction : NPCStateAbstract
{
	private float timer;
	private float animationDuration;

	public NPCStateStationaryAction(NPCStateMachineController npcStateMachineController, float animationDuration)
	{
		this._NPCStateMachineController = npcStateMachineController;
		this.animationDuration = animationDuration;
		timer = 0f;

		_NPCStateMachineController.StopAnchorMove(); 
	}

	public override void Update()
	{
		if (_NPCStateMachineController.AnchorPoints.Count != 0)
		{
			timer += Time.deltaTime;

			if (timer >= animationDuration)
			{
				_NPCStateMachineController.SetNPCState(NPCStateTypes.Patrolling);
			}
		}
	}
}
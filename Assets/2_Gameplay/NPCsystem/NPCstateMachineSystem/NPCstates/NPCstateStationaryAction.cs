using UnityEngine;

public class NPCstateStationaryAction : NPCstateAbstract
{
	private float timer;
	private float animationDuration;

	public NPCstateStationaryAction(NPCstateMachineController npcStateMachineController, float animationDuration)
	{
		this._NPCStateMachineController = npcStateMachineController;
		this.animationDuration = animationDuration;
		timer = 0f;

		_NPCStateMachineController.StopAnchorMove(); 
	}

	public override void Update()
	{
		if (_NPCStateMachineController.AnchorData.Count != 0)
		{
			timer += Time.deltaTime;

			if (timer >= animationDuration)
			{
				_NPCStateMachineController.SetNPCState(NPCstateTypes.Patrolling);
			}
		}
	}
}
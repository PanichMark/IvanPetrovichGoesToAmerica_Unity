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

	/*
	 * Peaceful Neutral Aggresive
	 * 2) если есть AnchorPoint и закончили стоять то Patrolling
	 * 
	 * Peaceful Neutral Aggresive
	 * 3) если тревога дошла до 50  то Intereset
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
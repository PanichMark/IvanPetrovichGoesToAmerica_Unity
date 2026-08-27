using UnityEngine;

public class NPCstateInterested : NPCstateAbstract
{
    public NPCstateInterested()
    {

    }

	/*
	 *	Идем туда где видели или слышали игрока на моменте тревоги 50
	 */

	/*
	 * Peaceful Neutral Aggresive
	 * 1) если тревога упала до 0 и НЕ было AnchorPoints то StationaryAction
	 * 
	 * Peaceful Neutral Aggresive
	 * 2) если тревога упала до 0 и БЫЛИ AnchorPoints то Patrolling
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

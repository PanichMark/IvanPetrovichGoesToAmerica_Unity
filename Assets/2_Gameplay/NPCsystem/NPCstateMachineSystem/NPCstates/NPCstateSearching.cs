using UnityEngine;

public class NPCstateSearching : NPCstateAbstract
{
    public NPCstateSearching()
    {

    }
	/*
	 *	Ходим какоето время в рандомныфе поовроты с интервалами ищем игрока опять
	 */

	/* Neutral Aggresive
	 * 1) если тревога дошла до 0 и не было AnchorPoints то StationaryAction
	 * 
	 * Neutral Aggresive
	 * 2) если тревога дошла до 0 и были AnchorPoints то Patrolling
	 * 
	 * Neutral Aggresive
	 * 3) если тревога упала ниже 50 и потом поднялась выше 50 но не выше 100 то Interesed
	 * 
	 * Neutral Aggresive
	 * 4) если тревога сразу поднялась до 100 то Alarmed
	 * 
	 * Neutral Aggresive
	 * 7) Если Ranged оружие то Reloading
	 * 
	 *  Neutral Aggresive
	 * 8) если XX секунд не добежали до игрока то Searching
	 * 
	 * Neutral Aggresive
	 * 12) если начали душить то Strangled
	 * 
	 * Neutral Aggresive
	 * 13) если подцепили арбалетом то Hooked
	 * 
	 * Neutral Aggresive
	 * 14) если выстрелили ногой то Staggered
	 * 
	 * Neutral Aggresive
	 * 15) если толкнули ногой то KnockedOff
	 * 
	 * Neutral Aggresive
	 * 16) если Дыхание Джина то BlownAway
	 * 
	 * Neutral Aggresive
	 * 17) если ИскраТеслы то ElectroShocked
	 * 
	 * Neutral Aggresive
	 * 18) если под ногами нет земли то Falling
	 * 
	 * Neutral Aggresive
	 * 20) если транквилизатор то Dizzy
	 * 
	 * Neutral Aggresive
	 * 23) если убили с 1 удара то Dead
	 */
}

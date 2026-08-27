using UnityEngine;

public class NPCstateAlarmed : NPCstateAbstract
{
    public NPCstateAlarmed()
    {

    }
	/* Neutral Aggresive
	 * 5)если игрок дальше XX метров то Chasing
	 * XX в зависимости от Melee или Ranged weapons
	 * 
	 * Neutral Aggresive
	 * 6)если игрок ближе XX метров то Attacking
     * 
     * Peaceful
     * в зависимости от конфига то 
     * 9)Huddled или
     * 10) Hysteric или
     * 11)Fleeing
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

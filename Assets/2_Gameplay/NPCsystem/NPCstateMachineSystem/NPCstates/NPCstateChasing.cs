using UnityEngine;

public class NPCstateChasing : NPCstateAbstract
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public NPCstateChasing()
    {

    }
	/*
	 *	быстро бегаем за игроком
	 */

	/* Neutral Aggresive
     * 6)если игрок ближе XX метров то Attacking
     * 
     *  Neutral Aggresive
     * 8) если XX секунд не добежали до игрока то Searching
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

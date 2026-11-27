using UnityEngine;

public class NPCstateFleeing : NPCstateAbstract
{
    public NPCstateFleeing()
    {

    }

	/*
	 *	убегаем как можно дальше от игрока по длинне маршрута NavMesh
	 */

	/* 
 * Peaceful 
 * 1) если тревога дошла до 0 и не было AnchorPoints то StationaryAction
 * 
 * Peaceful 
 * 2) если тревога дошла до 0 и были AnchorPoints то Patrolling
 * 
 * Peaceful 
 * 12) если начали душить то Strangled
 * 
 * Peaceful 
 * 13) если подцепили арбалетом то Hooked
 * 
 * Peaceful 
 * 14) если выстрелили ногой то Staggered
 * 
 * Peaceful 
 * 15) если толкнули ногой то KnockedOff
 * 
 * Peaceful 
 * 16) если Дыхание Джина то BlownAway
 * 
 * Peaceful 
 * 17) если ИскраТеслы то ElectroShocked
 * 
 * Peaceful 
 * 18) если под ногами нет земли то Falling
 * 
 * Peaceful 
 * 20) если транквилизатор то Dizzy
 * 
 * Peaceful 
 * 23) если убили с 1 удара то Dead
 */
}

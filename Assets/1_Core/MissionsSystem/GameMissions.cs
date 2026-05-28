using UnityEngine;

// Этот ScriptableObject теперь живет в Core.
// Он определяет глобальную структуру прохождения игры.
[CreateAssetMenu(fileName = "GameMissions", menuName = "Missions/GameMissions")]
public class GameMissions : ScriptableObject
{
	// Массив всех миссий в игре в правильном порядке.
	// Миссии (Mission) все еще живут в Gameplay.
	public MissionAbstract[] MissionsInOrder;
}
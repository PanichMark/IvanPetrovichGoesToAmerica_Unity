using UnityEngine;

[CreateAssetMenu(fileName = "GameMissionsList", menuName = "Missions/GameMissionsList")]
public class GameMissionsList : ScriptableObject
{
	public Mission[] MissionsInOrder;
}
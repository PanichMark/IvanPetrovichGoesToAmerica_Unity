using UnityEngine;

[CreateAssetMenu(fileName = "MissionMapsList", menuName = "Missions/MissionMaps/MissionMapsList")]
public class MissionMapsList : ScriptableObject
{
	public MissionMapData[] MissionMapsInOrder;
}

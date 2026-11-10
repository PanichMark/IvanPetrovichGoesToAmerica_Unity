using UnityEngine;

[CreateAssetMenu(fileName = "MissionMapData", menuName = "Missions/MissionMapData")]
public class MissionMapData : ScriptableObject
{
	public MissionMapsList MissionMapID;
	public InteractionObjectNoteData MissionMapNoteData;
}

using UnityEngine;

[CreateAssetMenu(fileName = "MissionMapData", menuName = "Missions/MissionMaps/MissionMapData")]
public class MissionMapData : ScriptableObject
{
	public MissionMapsNamesEnum MissionMapID;
	public InteractionObjectNoteData MissionMapNoteData;
}

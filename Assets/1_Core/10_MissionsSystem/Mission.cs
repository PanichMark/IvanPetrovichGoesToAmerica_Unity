using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "Missions/Mission")]

public class Mission : ScriptableObject
{
	[SerializeField] private GameMissionsNamesEnum _missionName;
	[SerializeField] private MissionResourcesData _missionResources;
	public string MissionName => _missionName.ToString();
	public MissionResourcesData MissionResources => _missionResources;
	public MissionStep[] MissionSteps;

}
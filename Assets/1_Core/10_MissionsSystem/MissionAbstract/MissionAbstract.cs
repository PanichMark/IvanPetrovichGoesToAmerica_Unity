using UnityEngine;

public abstract class MissionAbstract : ScriptableObject, IMission
{
	[SerializeField] private GameMissionsNamesEnum _missionName;
	[SerializeField] private MissionResourcesData _missionResources;
	public string MissionName => _missionName.ToString();
	public MissionResourcesData MissionResources => _missionResources;
	public abstract MissionStepAbstract[] MissionSteps { get; }
}
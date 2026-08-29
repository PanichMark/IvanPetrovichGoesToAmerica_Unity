using UnityEngine;

public abstract class MissionAbstract : ScriptableObject, IMission
{
	[SerializeField] private GameMissionsNamesEnum _missionName;
	public string MissionName => _missionName.ToString();
	public abstract MissionStepAbstract[] MissionSteps { get; }
}
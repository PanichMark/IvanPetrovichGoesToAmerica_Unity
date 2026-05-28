using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "Missions/NewMission")]
public class Mission : MissionAbstract
{
	public string missionName;

	public MissionStep[] missionStepsArray;

	public override MissionStepAbstract[] steps => missionStepsArray;
}
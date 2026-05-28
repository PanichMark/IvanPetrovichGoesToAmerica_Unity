using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "Missions/Mission")]
public class Mission : MissionAbstract
{
	public string MissionDiscription;

	public MissionStep[] MissionStepsArray;

	public override MissionStepAbstract[] Steps => MissionStepsArray;
}
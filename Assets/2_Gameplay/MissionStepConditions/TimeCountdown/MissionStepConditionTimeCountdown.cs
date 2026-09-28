using UnityEngine;

[CreateAssetMenu(fileName = "TimeCountdown", menuName = "Missions/StepConditions/TimeCountdown")]
public class MissionStepConditionTimeCountdown : MissionStepConditionAbstract, IMissionStepConditionWithCountdown
{
	public override void ResetStepCondition()
	{
		//throw new System.NotImplementedException();
	}
}

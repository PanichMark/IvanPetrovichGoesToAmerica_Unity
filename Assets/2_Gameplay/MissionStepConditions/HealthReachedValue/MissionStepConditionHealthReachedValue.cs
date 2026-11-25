using UnityEngine;

[CreateAssetMenu(fileName = "HealthReachedValue", menuName = "Missions/StepConditions/HealthReachedValue")]
public class MissionStepConditionHealthReachedValue : MissionStepConditionAbstract
{
	public void CompleteCondition()
	{
		if (!_isConditionMet)
		{
			_isConditionMet = true;
			_missionStepAbstract.OnStepCompleted(GoToNextStep);
			Debug.Log("[Условие] Условие по здоровью выполнено.");
		}
	}

	public override void ResetStepCondition()
	{
		_isConditionMet = false;
	}
}
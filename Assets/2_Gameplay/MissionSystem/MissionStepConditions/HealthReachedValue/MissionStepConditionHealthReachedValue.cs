using UnityEngine;

[CreateAssetMenu(fileName = "HealthReachedValue", menuName = "Missions/StepConditions/HealthReachedValue")]
public class MissionStepConditionHealthReachedValue : MissionStepConditionAbstract
{
	public void CompleteCondition()
	{
		if (!_isConditionMet && IsActive)
		{
			_isConditionMet = true;
			_missionStepAbstract.OnStepCompleted(GoToNextStep);
			Debug.Log("[Условие] Условие по здоровью выполнено.");
		}
	}

	public override void ResetStepConditionBySystem()
	{
		_isConditionMet = false;
	}

	public override void ResetStepConditionByReturning()
	{
		_isConditionMet = false;
	}
}
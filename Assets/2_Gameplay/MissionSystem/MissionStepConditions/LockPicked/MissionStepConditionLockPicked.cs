using UnityEngine;

[CreateAssetMenu(fileName = "LockPicked", menuName = "Missions/StepConditions/LockPicked")]
public class MissionStepConditionLockPicked : MissionStepConditionAbstract
{
	public void CompleteCondition()
	{
		if (!_isConditionMet)
		{
			_isConditionMet = true;
			_missionStepAbstract.OnStepCompleted(GoToNextStep);
			Debug.Log("[Условие] Замок взломан.");
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
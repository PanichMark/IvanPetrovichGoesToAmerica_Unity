using UnityEngine;

[CreateAssetMenu(fileName = "PickableObjectReleased", menuName = "Missions/StepConditions/PickableObjectReleased")]
public class MissionStepConditionPickableObjectReleased : MissionStepConditionAbstract
{
	public void CompleteCondition()
	{
		if (!_isConditionMet)
		{
			_isConditionMet = true;
			_missionStepAbstract.OnStepCompleted(GoToNextStep);
			Debug.Log("[Условие] Подбираемый предмет выброшен.");
		}
	}

	public override void ResetStepCondition()
	{
		_isConditionMet = false;
	}
}
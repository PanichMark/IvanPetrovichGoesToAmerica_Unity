using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeCountdown", menuName = "Missions/StepConditions/TimeCountdown")]
public class MissionStepConditionTimeCountdown : MissionStepConditionAbstract, IMissionStepConditionWithCountdown
{
	public event Action<float> OnStepConditionTimeTick;

	public void CompleteCondition()
	{
		if (!_isConditionMet)
		{
			_isConditionMet = true;
			_missionStepAbstract.OnStepCompleted(GoToNextStep);
			Debug.Log("[Условие] Время вышло.");
		}
	}

	public void UpdateTimer(float currentTimeRemaining)
	{
		Debug.Log(currentTimeRemaining);


		
		OnStepConditionTimeTick?.Invoke(currentTimeRemaining);
		
	}

	public override void ResetStepCondition()
	{
		_isConditionMet = false;
	}
}
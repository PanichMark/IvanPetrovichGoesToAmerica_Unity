using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeCountdown", menuName = "Missions/StepConditions/TimeCountdown")]
public class MissionStepConditionTimeCountdown : MissionStepConditionAbstract, IMissionStepConditionWithCountdown
{
	public event Action<float> OnStepConditionTimeTick;

	public void CompleteCondition()
	{
		if (!_isConditionMet && IsActive)
		{
			_isConditionMet = true;
			_missionStepAbstract.OnStepCompleted(GoToNextStep);
			//Debug.Log("[Условие] Время вышло.");
		}
		else
		{
			Debug.Log("Timecountdown was already met THIS !");
		}	
	}

	public void UpdateTimer(float currentTimeRemaining)
	{
		//Debug.Log(currentTimeRemaining);


		
		OnStepConditionTimeTick?.Invoke(currentTimeRemaining);
		
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
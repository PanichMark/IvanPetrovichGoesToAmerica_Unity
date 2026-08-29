using UnityEngine;

public interface IMissionStepCondition
{
	bool IsConditionMet { get; }

	GameObject StepConditionOwner { get; }

	void ResetStepCondition();

	void Initialize(MissionStepAbstract missionStepAbstract);
}
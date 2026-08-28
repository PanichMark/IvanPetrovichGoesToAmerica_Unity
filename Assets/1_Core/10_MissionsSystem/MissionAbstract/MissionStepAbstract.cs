using System.Collections.Generic;
using UnityEngine;

public abstract class MissionStepAbstract : ScriptableObject, IMissionStep
{
	public string MissionStepGoal_RU;
	public string MissionStepGoal_EN;

	public bool ShowMissionMarker => throw new System.NotImplementedException();

	public IReadOnlyList<IMissionStepCondition> Conditions
	{
		get { return StepConditions.ConvertAll(c => (IMissionStepCondition)c); }
	}

	public List<MissionStepConditionAbstract> StepConditions = new List<MissionStepConditionAbstract>();

	public abstract void OnStepCompleted();

}
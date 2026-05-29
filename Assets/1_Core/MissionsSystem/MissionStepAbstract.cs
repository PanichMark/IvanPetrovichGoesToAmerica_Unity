using UnityEngine;

public abstract class MissionStepAbstract : ScriptableObject
{
	public string MissionStepGoal_RU;
	public string MissionStepGoal_EN;

	public abstract void OnStepCompleted();

}
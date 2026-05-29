using UnityEngine;

public abstract class MissionStepAbstract : ScriptableObject
{
	public string MissionStepGoal;

	public abstract void OnStepCompleted();

}
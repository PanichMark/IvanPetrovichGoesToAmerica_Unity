using UnityEngine;

public abstract class MissionStepConditionAbstract : ScriptableObject
{
	public abstract bool IsConditionMet();
}
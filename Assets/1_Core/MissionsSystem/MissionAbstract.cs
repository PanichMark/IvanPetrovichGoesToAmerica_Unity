using UnityEngine;

public abstract class MissionAbstract : ScriptableObject, IMission
{
	public abstract MissionStepAbstract[] Steps { get; }
}
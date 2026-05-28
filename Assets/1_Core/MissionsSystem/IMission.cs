using UnityEngine;

public interface IMission
{
	MissionStepAbstract[] Steps { get; }
}
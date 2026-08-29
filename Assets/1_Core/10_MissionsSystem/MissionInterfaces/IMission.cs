using UnityEngine;

public interface IMission
{
	public string MissionName { get; }
	MissionStepAbstract[] MissionSteps { get; }
}
using UnityEngine;

public interface IMission
{
	MissionStepAbstract[] steps { get; }
}
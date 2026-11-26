using UnityEngine;

[System.Serializable]
public struct NPCmovementAnchorData
{
	public GameObject NPCanchorPoint;
	public float NPCwaitDuration;
	public Vector3 NPCrotation;
	public MissionStep RequiredMissionStepToProceed;
	public bool WaitForPatrollingAfterMissionStep;
}
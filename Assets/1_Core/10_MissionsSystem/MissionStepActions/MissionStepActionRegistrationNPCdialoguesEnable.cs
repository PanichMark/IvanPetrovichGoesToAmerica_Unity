using UnityEngine;

public class MissionStepActionRegistrationNPCdialoguesEnable : MonoBehaviour
{
	[SerializeField] private MissionStep _linkedMissionStep;

	private void Start()
	{
		Debug.Log($"{gameObject.name} registered in NPC Dialogue Enable for {_linkedMissionStep}");
		_linkedMissionStep.RegisterNPCdialoguesEnable(gameObject);

	}
}

using UnityEngine;

public class MissionStepActionRegistrationNPCdialoguesDisable : MonoBehaviour
{
	[SerializeField] private MissionStep _linkedMissionStep;

	private void Start()
	{
		Debug.Log($"{gameObject.name} registered in NPC Dialogue Disable for {_linkedMissionStep}");
		_linkedMissionStep.RegisterNPCdialoguesDisable(gameObject);

	}
}

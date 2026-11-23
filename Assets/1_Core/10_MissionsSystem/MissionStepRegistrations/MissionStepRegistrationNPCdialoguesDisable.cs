using UnityEngine;

public class MissionStepRegistrationNPCdialoguesDisable : MonoBehaviour
{
	[SerializeField] private MissionStep _linkedMissionStep;

	private void Start()
	{
		Debug.Log($"{gameObject.name} registered in NPC Dialogue Disable for {_linkedMissionStep}");
		_linkedMissionStep.RegisterNPCdialoguesDisable(gameObject);

	}
}

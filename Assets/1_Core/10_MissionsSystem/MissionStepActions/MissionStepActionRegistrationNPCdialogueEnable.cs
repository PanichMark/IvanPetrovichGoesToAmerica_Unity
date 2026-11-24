using UnityEngine;

public class MissionStepActionRegistrationNPCdialogueEnable : MonoBehaviour
{
	[SerializeField] private MissionStep _linkedMissionStep;
	[SerializeField] private bool _enable;

	private void Start()
	{
		//Debug.Log($"{gameObject.name} registered in NPC Dialogue Disable for {_linkedMissionStep}");
		_linkedMissionStep.RegisterNPCdialoguesEnable(gameObject, _enable);



	}
}

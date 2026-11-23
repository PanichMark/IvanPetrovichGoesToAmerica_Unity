using UnityEngine;

public class MissionStepActionRegistrationGameObjectTurnOn : MonoBehaviour
{
	[SerializeField] private MissionStep _linkedMissionStep;

	private void Start()
	{
		Debug.Log($"{gameObject.name} registered in TurnOn for {_linkedMissionStep}");
		_linkedMissionStep.RegisterObjectsTurnOn(gameObject);
		
	}
}
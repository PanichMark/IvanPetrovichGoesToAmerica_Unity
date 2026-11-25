using UnityEngine;

public class MissionStepActionRegistrationGameObjectSetActive : MonoBehaviour
{
	[SerializeField] private MissionStep _linkedMissionStep;
	[SerializeField] private bool _enable;

	private void Start()
	{
		//Debug.Log($"{gameObject.name} registered in TurnOn for {_linkedMissionStep}");
		_linkedMissionStep.RegisterObjectsTurnOn(gameObject, _enable);
		
	}
}
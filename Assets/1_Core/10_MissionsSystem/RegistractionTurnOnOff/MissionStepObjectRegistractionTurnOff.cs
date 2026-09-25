using UnityEngine;

public class MissionStepObjectRegistractionTurnOff : MonoBehaviour
{
	[SerializeField] private MissionStep _linkedMissionStep;

	private void Start()
	{
		Debug.Log($"{gameObject.name} registered in TurnOff for {_linkedMissionStep}");
		_linkedMissionStep.RegisterObjectsTurnOff(gameObject);
		
	}
}
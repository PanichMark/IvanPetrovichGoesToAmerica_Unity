using UnityEngine;

public class MissionStepObjectRegistractionTurnOn : MonoBehaviour
{
	[SerializeField] private MissionStepAbstract _linkedMissionStep;

	private void Start()
	{
		Debug.Log($"{gameObject.name} registered in TurnOn for {_linkedMissionStep}");
		_linkedMissionStep.RegisterObjectsTurnOn(gameObject);
		
	}
}
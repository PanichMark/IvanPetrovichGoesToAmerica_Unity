using UnityEngine;

public class MissionStepConditionNPCstateChangedRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	[SerializeField] private NPCstateTypes _targetStateToTrack;

	private NPCstateMachineController _stateMachine;

	private void Start()
	{
		_stateMachine = GetComponent<NPCstateMachineController>();


		_stateMachine.OnNewNPCstate += HandleStateChanged;
	}

	private void HandleStateChanged(NPCstateTypes newState)
	{
		if (_linkedMissionCondition is MissionStepConditionNPCstateChanged stateCondition)
		{
			if (newState == _targetStateToTrack)
			{
				stateCondition.CheckState(newState);
			}
		}
	}

	private void OnDestroy()
	{
	
			_stateMachine.OnNewNPCstate -= HandleStateChanged;
		
	}
}
using UnityEngine;

public class MissionStepActionNPCchangeState : MonoBehaviour, IMissionStepAction
{
	[SerializeField] private MissionStep _linkedMissionStep;
	private NPCstateMachineController _npcStateMachineController;
	[SerializeField] private NPCstateTypes _npcStateToSet;

	private void Start()
	{
		_npcStateMachineController = GetComponent<NPCstateMachineController>();
		_linkedMissionStep.RegisterAction(this);
		
	}

	public void PerformAction()
	{
		if (_npcStateMachineController.CurrentNPCState != _npcStateToSet)
		{
			_npcStateMachineController.SetNPCState(_npcStateToSet);
		}
	}
}
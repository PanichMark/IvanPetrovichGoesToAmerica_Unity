using UnityEngine;

public class MissionStepActionNPCchangeState : MonoBehaviour, IMissionStepAction
{
	[SerializeField] private MissionStep _linkedMissionStep;
	[SerializeField] private NPCstateMachineController _npcStateMachineController;
	[SerializeField] private NPCstateTypes _npcStateToSet;

	private void Start()
	{
		if (_linkedMissionStep != null)
		{
			_linkedMissionStep.RegisterAction(this);
		}
	}

	public void PerformAction()
	{
		if (_npcStateMachineController != null &&
			_npcStateMachineController.CurrentNPCState != _npcStateToSet)
		{
			_npcStateMachineController.SetNPCState(_npcStateToSet);
		}
	}
}
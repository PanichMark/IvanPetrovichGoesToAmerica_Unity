using UnityEngine;

public class MissionStepActionNPCdialogueEnable : MonoBehaviour, IMissionStepAction
{
	[SerializeField] private MissionStep _linkedMissionStep;
	[SerializeField] private bool _enable;

	private void Start()
	{
		if (_linkedMissionStep != null)
		{
			_linkedMissionStep.RegisterAction(this);
		}
	}

	public void PerformAction()
	{
		var dialogueController = GetComponent<NPCdialogueController>();
		if (dialogueController != null)
		{
			dialogueController.enabled = _enable;
		}
	}
}

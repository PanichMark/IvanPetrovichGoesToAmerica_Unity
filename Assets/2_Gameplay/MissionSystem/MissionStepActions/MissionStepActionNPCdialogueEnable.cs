using UnityEngine;

public class MissionStepActionNPCdialogueEnable : MonoBehaviour, IMissionStepAction
{
	[SerializeField] private MissionStep _linkedMissionStep;
	[SerializeField] private bool _enable;
	[SerializeField] private int _dialogueIndex;

	private void Start()
	{
		if (_linkedMissionStep != null)
		{
			_linkedMissionStep.RegisterAction(this);
		}
	}

	public void PerformAction()
	{
		var neutralNPC = GetComponent<NPCneutral>();
		if (neutralNPC != null)
		{
			neutralNPC.SetDialogueControllerEnabled(_dialogueIndex, _enable);
			return;
		}

		var peacefulNPC = GetComponent<NPCpeaceful>();
		if (peacefulNPC != null)
		{
			peacefulNPC.SetDialogueControllerEnabled(_dialogueIndex, _enable);
			return;
		}

		Debug.LogError($"Cannot find NPCneutral or NPCpeaceful on '{name}' to set dialogue state.", this);
	}
}

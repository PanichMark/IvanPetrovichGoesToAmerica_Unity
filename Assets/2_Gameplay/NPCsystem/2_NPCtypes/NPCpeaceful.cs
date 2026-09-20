using UnityEngine;


public class NPCpeaceful : NPClivingBeing
{
	private NPCdialogueController _NPCdialogueController;

	protected override void InitializeNPClivingBeing()
	{
		_NPCdialogueController = GetComponent<NPCdialogueController>();

		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Initialize(
				_NPCmovementController,
				_NPCstateMachineController);
		}
	}

	public override void Interact()
	{
		//Debug.Log("NPC interact");

		if (_NPCstateMachineController.CurrentNPCState == NPCstateTypes.Dead || _NPCstateMachineController.CurrentNPCState == NPCstateTypes.Unconscious)
		{
			_pickable.Interact();
			return;
		}

		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.StationaryAction &&
			_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Patrolling)
			return;

		if (!_canNPCbeRobbed)
		{
			if (_NPCdialogueController != null)
			{
				_NPCdialogueController.Interact();
			}
			else
			{
				StopAllCoroutines();
				_NPCphrasesController.TemporaryShowPhrases();
			}
		}
		else
		{
			_NPClootObjectComponent.Interact();
			NPCwasRobbed();
		}
	}

	protected override void DisableDialogueController()
	{
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.enabled = false;
		}
	}
}
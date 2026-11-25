using UnityEngine;


public class NPCpeaceful : NPClivingBeing, IMissionStepActionNPCdialogueEnable
{
	private NPCdialogueController _NPCdialogueController;

	[SerializeField] private NPCphrasesDataPeaceful _NPCPhrasesDataPeaceful;
	[SerializeField] private NPCpeacefulReactionType _NPCPeacefulReactionType;

	protected override void InitializeNPClivingBeing()
	{
		_NPCphrasesController.InitializePeaceful(
			this,
			_NPCPhrasesDataPeaceful);

		_NPCdialogueController = GetComponent<NPCdialogueController>();

		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Initialize(
				this,
				_NPCmovementController);
		}
	}

	public void EnableDialogue()
	{
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.enabled = true;
		}
	}

	public void DisableDialogue()
	{
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.enabled = false;
		}
	}

	public override void Interact()
	{
		//Debug.Log("NPC interact");

		if (_NPCstateMachineController.CurrentNPCState == NPCstateTypes.Dead || _NPCstateMachineController.CurrentNPCState == NPCstateTypes.Unconscious)
		{
			CallOnInteract();
			_pickable.Interact();

			if (!_wasNPCrobbed)
			{
				if (_NPCLootObject != null)
				{
					CallOnInteract();
					_NPClootObjectComponent.Interact();
					NPCwasRobbed();
				}
			}

			return;
		}

		if (_NPCstateMachineController.CurrentNPCState != NPCstateTypes.StationaryAction &&
			_NPCstateMachineController.CurrentNPCState != NPCstateTypes.Patrolling)
			return;

		if (!_canNPCbeRobbed)
		{
			if (_NPCdialogueController != null && _NPCdialogueController.enabled == true)

			{
				_NPCdialogueController.Interact();
			}
			else
			{
				CallOnInteract();
				StopAllCoroutines();
				_NPCphrasesController.TemporaryShowPhrases();
			}
		}
		else
		{
			CallOnInteract();
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
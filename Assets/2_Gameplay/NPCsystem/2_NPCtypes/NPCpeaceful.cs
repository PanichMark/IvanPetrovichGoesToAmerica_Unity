using UnityEngine;


public class NPCpeaceful : NPCabstract
{
	protected NPCdialogueController _NPCdialogueController;

	protected override void InitializeNPC()
	{
		_NPChealthController = GetComponent<NPChealthController>();
		_NPCdialogueController = GetComponent<NPCdialogueController>();

		
		_NPChealthController.Initialize(
			this,
			_NPCstateMachineController);

		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Initialize(_NPCstateMachineController);
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

	protected override void DisableInteractiveNPCscripts()
	{
		_NPCphrasesController.enabled = false;
		
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.enabled = false;
		}
	}
}
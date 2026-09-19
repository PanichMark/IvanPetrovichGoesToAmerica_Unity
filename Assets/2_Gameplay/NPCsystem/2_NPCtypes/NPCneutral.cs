using UnityEngine;

public class NPCneutral : NPCabstract
{
	protected NPCdialogueController _NPCdialogueController;
	protected NPCphrasesController _NPCphrasesController;

	protected override void InitializeNPC()
	{
		_NPCphrasesController = GetComponent<NPCphrasesController>();
		_NPCdialogueController = GetComponent<NPCdialogueController>();

		_NPCphrasesController.Initialize(this);

		_NPCdialogueController.Initialize(_NPCstateMachineController);
	}

	public override void Interact()
	{
		if (_NPCstateMachineController?.CurrentNPCState != NPCstateTypes.StationaryAction
			&& _NPCstateMachineController?.CurrentNPCState != NPCstateTypes.Patrolling)
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
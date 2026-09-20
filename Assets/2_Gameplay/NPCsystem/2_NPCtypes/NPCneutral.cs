public class NPCneutral : NPClivingBeing
{
	private NPCdialogueController _NPCdialogueController;

	private NPCweaponController _NPCweaponController;
	private NPCweaponAnimationController _NPCweaponAnimationController;

	protected override void InitializeNPClivingBeing()
	{
		_NPCdialogueController = GetComponent<NPCdialogueController>();

		_NPCweaponController = GetComponent<NPCweaponController>();
		_NPCweaponAnimationController = GetComponent<NPCweaponAnimationController>();

		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Initialize(_NPCstateMachineController);
		}

		_NPCweaponController.Initialize(
			_NPCstateMachineController,
			_NPCdetectionManager);
		_NPCweaponAnimationController.Initialize(
			_NPCweaponController,
			_NPCanimator);
	}

	public override void Interact()
	{
		if (_NPCstateMachineController.CurrentNPCState == NPCstateTypes.Dead || _NPCstateMachineController.CurrentNPCState == NPCstateTypes.Unconscious)
		{
			_pickable.Interact();
			return;
		}

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

	protected override void DisableDialogueController()
	{
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.enabled = false;
		}
	}
}
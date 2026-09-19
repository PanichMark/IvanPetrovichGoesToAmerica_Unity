public class NPCneutral : NPClivingBeing
{
	protected NPCdialogueController _NPCdialogueController;

	private NPCweaponController _NPCweaponController;
	private NPCweaponAnimationController _NPCweaponAnimationController;

	protected override void InitializeNPClivingBeing()
	{
		_NPCweaponController = GetComponent<NPCweaponController>();
		_NPCweaponAnimationController = GetComponent<NPCweaponAnimationController>();

		_NPChealthController = GetComponent<NPChealthController>();
		_NPCdialogueController = GetComponent<NPCdialogueController>();

		_NPChealthController.Initialize(
			this,
			_NPCstateMachineController);

		_NPCweaponController.Initialize(
			_NPCstateMachineController,
			_NPCdetectionManager);

		_NPCweaponAnimationController.Initialize();

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

	protected override void DisableialogueController()
	{
		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.enabled = false;
		}
	}
}
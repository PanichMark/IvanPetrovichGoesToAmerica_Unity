public class NPCaggressive : NPCabstract
{
	private NPCweaponController _NPCweaponController;
	private NPCweaponAnimationController _NPCweaponAnimationController;

	protected override void InitializeNPC()
	{
		_NPChealthController = GetComponent<NPChealthController>();
		_NPCweaponController = GetComponent<NPCweaponController>();
		_NPCweaponAnimationController = GetComponent<NPCweaponAnimationController>();

		_NPChealthController.Initialize(
			this,
			_NPCstateMachineController);

		_NPCweaponController.Initialize(
			_NPCstateMachineController,
			_NPCdetectionManager);

		_NPCweaponAnimationController.Initialize();
		
	}

	override public void Interact()
	{
		if (_NPCstateMachineController.CurrentNPCState == NPCstateTypes.Dead)
		{
			_pickable.Interact();
			return;
		}
	}
}
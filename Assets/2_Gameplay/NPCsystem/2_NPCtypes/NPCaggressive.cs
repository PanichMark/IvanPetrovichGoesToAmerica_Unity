public class NPCaggressive : NPClivingBeing
{
	private NPCweaponController _NPCweaponController;
	private NPCweaponAnimationController _NPCweaponAnimationController;

	protected override void InitializeNPClivingBeing()
	{
		_NPCweaponController = GetComponent<NPCweaponController>();
		_NPCweaponAnimationController = GetComponent<NPCweaponAnimationController>();

		_NPCweaponController.Initialize(
			_NPCstateMachineController,
			_NPCdetectionManager);

		_NPCweaponAnimationController.Initialize();
	}

	override public void Interact()
	{
		if (_NPCstateMachineController.CurrentNPCState == NPCstateTypes.Dead || _NPCstateMachineController.CurrentNPCState == NPCstateTypes.Unconscious)
		{
			_pickable.Interact();
			return;
		}
	}
}
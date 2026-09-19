public class NPCaggressive : NPCabstract
{
	private NPCweaponController _NPCweaponController;

	protected override void InitializeNPC()
	{
		_NPCweaponController = GetComponent<NPCweaponController>();

		_NPCweaponController.Initialize(
			_NPCstateMachineController,
			_NPCdetectionManager);
		
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
using UnityEngine;

public class NPCaggressive : NPClivingBeing
{
	private NPCweaponController _NPCweaponController;
	private NPCweaponAnimationController _NPCweaponAnimationController;
	[SerializeField] private NPCphrasesDataNonPeaceful _NPCPhrasesDataAggressive;

	protected override void InitializeNPClivingBeing()
	{
		_NPCweaponController = GetComponent<NPCweaponController>();
		_NPCweaponAnimationController = GetComponent<NPCweaponAnimationController>();

		_NPCphrasesController.InitializeNonPeaceful(
			this,
			_NPCPhrasesDataAggressive);

		_NPCweaponController.Initialize(
			_NPCstateMachineController,
			_NPCdetectionManager,
			_transferArmatureBones);

		_NPCweaponAnimationController.Initialize(
			_NPCdetectionVisualController,
			_NPCweaponController,
			_NPCanimator);
	}

	override public void Interact()
	{
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

		if (_canNPCbeRobbed)
		{
			CallOnInteract();
			_NPClootObjectComponent.Interact();
			NPCwasRobbed();
		}
	}
}
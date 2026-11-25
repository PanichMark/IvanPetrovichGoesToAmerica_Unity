using UnityEngine;

public class NPCneutral : NPClivingBeing
{
	private NPCdialogueController _NPCdialogueController;

	private NPCweaponController _NPCweaponController;
	private NPCweaponAnimationController _NPCweaponAnimationController;
	[SerializeField] private NPCphrasesDataNonPeaceful _NPCPhrasesDataNeutral;

	protected override void InitializeNPClivingBeing()
	{
		_NPCdialogueController = GetComponent<NPCdialogueController>();

		_NPCweaponController = GetComponent<NPCweaponController>();
		_NPCweaponAnimationController = GetComponent<NPCweaponAnimationController>();

		_NPCphrasesController.InitializeNonPeaceful(
			this,
			_NPCPhrasesDataNeutral);

		if (_NPCdialogueController != null)
		{
			_NPCdialogueController.Initialize(
				this,
				_NPCmovementController);
		}

		_NPCweaponController.Initialize(
			_NPCstateMachineController,
			_NPCdetectionManager,
			_transferArmatureBones);

		_NPCweaponAnimationController.Initialize(
			_NPCdetectionVisualController,
			_NPCweaponController,
			_NPCanimator);
	}

	public override void Interact()
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

		if (_NPCstateMachineController?.CurrentNPCState != NPCstateTypes.StationaryAction
			&& _NPCstateMachineController?.CurrentNPCState != NPCstateTypes.Patrolling)
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
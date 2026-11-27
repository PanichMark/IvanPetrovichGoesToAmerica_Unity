using System.Collections.Generic;
using UnityEngine;

public class NPCneutral : NPClivingBeing
{
	private List<NPCdialogueController> _NPCdialogueControllers = new List<NPCdialogueController>();

	private NPCweaponController _NPCweaponController;
	private NPCweaponAnimationController _NPCweaponAnimationController;
	[SerializeField] private NPCphrasesDataNonPeaceful _NPCPhrasesDataNeutral;

	protected override void InitializeNPClivingBeing()
	{
		_NPCdialogueControllers = new List<NPCdialogueController>(GetComponents<NPCdialogueController>());

		_NPCweaponController = GetComponent<NPCweaponController>();
		_NPCweaponAnimationController = GetComponent<NPCweaponAnimationController>();

		_NPCphrasesController.InitializeNonPeaceful(
			this,
			_NPCPhrasesDataNeutral);

		foreach (var dialogueController in _NPCdialogueControllers)
		{
			dialogueController.Initialize(
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
			if (TryInteractWithEnabledDialogue())
			{
				return;
			}

			CallOnInteract();
			StopAllCoroutines();
			_NPCphrasesController.TemporaryShowPhrases();
		}
		else
		{
			CallOnInteract();
			_NPClootObjectComponent.Interact();
			NPCwasRobbed();
		}
	}

	public bool SetDialogueControllerEnabled(int dialogueIndex, bool enable)
	{
		if (dialogueIndex < 0 || dialogueIndex >= _NPCdialogueControllers.Count)
		{
			Debug.LogError($"Dialogue index {dialogueIndex} is out of range on NPC '{name}'.", this);
			return false;
		}

		_NPCdialogueControllers[dialogueIndex].enabled = enable;
		return true;
	}

	private bool TryInteractWithEnabledDialogue()
	{
		NPCdialogueController enabledDialogueController = null;

		foreach (var dialogueController in _NPCdialogueControllers)
		{
			if (!dialogueController.enabled)
			{
				continue;
			}

			if (enabledDialogueController != null)
			{
				Debug.LogError($"Multiple enabled dialogue components on NPC '{name}'.", this);
				return true;
			}

			enabledDialogueController = dialogueController;
		}

		if (enabledDialogueController == null)
		{
			return false;
		}

		enabledDialogueController.Interact();
		return true;
	}

	protected override void DisableDialogueController()
	{
		foreach (var dialogueController in _NPCdialogueControllers)
		{
			dialogueController.enabled = false;
		}
	}
}
using UnityEngine;

public class InteractionObjectElevatorButton : MonoBehaviour, IInteractable, IElectroShockable
{
	public delegate void InteractionFailedDelegate();

	private LocalizationManager _localizationManager;

	public string InteractionObjectNameSystem => null;
	public string InteractionObjectNameUI => null;

	private string _interactionHintMessageMain;
	public string InteractionHintMessageMain => _interactionHintMessageMain;

	private string _interactionHintMessageFail;
	public string InteractionHintMessageFail => _interactionHintMessageFail;
	private bool _isInteractionHintMessageFailActive;
	public bool IsInteractionHintMessageFailActive => _isInteractionHintMessageFailActive;

	public string InteractionHintMessageAction => null;

	private bool _secondFloorButton;
	private bool _buttonUp;
	private InteractionObjectElevatorController _targetElevator;

	public event IInteractable.InteractableObjectHandler OnInteract;
	public event InteractionFailedDelegate OnInteractionFailed;

	public void Initialize(InteractionObjectElevatorController controller, bool secondFloorButton, bool buttonUp)
	{
		_targetElevator = controller;
		_secondFloorButton = secondFloorButton;
		_buttonUp = buttonUp;

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		ChangeLanguage(_localizationManager);
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		if (_secondFloorButton)
		{
			if (_buttonUp)
			{
				_interactionHintMessageMain = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_CallElevator")}?";
			}
			else
			{
				_interactionHintMessageMain = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_SendElevatorDown")}?";
			}
		}
		else
		{
			if (_buttonUp)
			{
				_interactionHintMessageMain = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_SendElevatorUp")}?";
			}
			else
			{
				_interactionHintMessageMain = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_CallElevator")}?";
			}
		}

		_interactionHintMessageFail = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_Wait")}";
	}

	public void Interact()
	{
		_isInteractionHintMessageFailActive = false;
		bool success = _targetElevator.MoveElevator(_buttonUp);
		if (!success)
		{
			_isInteractionHintMessageFailActive = true;
			OnInteractionFailed?.Invoke();
		}
	}

	public void InteractCutscene()
	{
		Interact();
	}

	public void Electrify(float damage)
	{
	}
}
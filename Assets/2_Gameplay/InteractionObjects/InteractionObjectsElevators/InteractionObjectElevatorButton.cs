using UnityEngine;

public class InteractionObjectElevatorButton : MonoBehaviour, IInteractable
{
	[SerializeField] private InteractionObjectElevatorController _targetElevator;
	[SerializeField] private bool _secondFloorButton;
	[SerializeField] private bool _buttonUp;
	
	private LocalizationManager _localizationManager;
	public string InteractionObjectNameSystem => null;
	public string InteractionObjectNameUI => null;

	private string _interactionHintMessageMain;
	public string InteractionHintMessageMain => _interactionHintMessageMain;
	public string InteractionHintMessageAction => null;
	public string InteractionHintMessageFail => _interactionHintMessageFail;
	private string _interactionHintMessageFail;
	public bool IsInteractionHintMessageFailActive => false;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

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
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private void ChangeLanguage()
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
		_targetElevator.MoveElevator(_buttonUp);
	}

	public void InteractCutscene()
	{
		Interact();
	}
}
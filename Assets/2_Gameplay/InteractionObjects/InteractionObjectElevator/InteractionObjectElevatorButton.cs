using UnityEngine;

public class InteractionObjectElevatorButton : MonoBehaviour, IInteractable
{
	public delegate void InteractionFailedDelegate();

	[SerializeField] private InteractionObjectElevatorController _targetElevator;
	[SerializeField] private bool _secondFloorButton;
	[SerializeField] private bool _buttonUp;
	public event IInteractable.InteractableObjectHandler OnInteract;
	public event InteractionFailedDelegate OnInteractionFailed;

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

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		ChangeLanguage();

		if (_localizationManager != null)
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
		_isInteractionHintMessageFailActive = false; 

		if (_targetElevator == null)
			return;

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
}
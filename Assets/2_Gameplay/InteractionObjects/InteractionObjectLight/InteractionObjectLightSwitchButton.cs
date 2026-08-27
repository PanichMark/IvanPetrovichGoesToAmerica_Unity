using UnityEngine;

public class InteractionObjectLightSwitchButton : MonoBehaviour, IInteractable, IElectroShockable
{
	[SerializeField] private InteractionObjectLightSwitchController _lightSwitchController;

	[SerializeField] private string _interactionObjectNameSystem;
	[SerializeField] private bool _isButtonSingle;
	[SerializeField] private bool _isThisTurnOnButton = true;
	private LocalizationManager _localizationManager;

	public event IInteractable.InteractableObjectHandler OnInteract;
	public string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	public string InteractionHintMessageFail => "";
	public bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction => _interactionHintMessageAction;

	private string _interactionHintMessageAction;

	void Start()
	{
	_localizationManager = ServiceLocator.Resolve<LocalizationManager>();

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		if (_isThisTurnOnButton)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOn", gameObject.name)}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOff", gameObject.name)}";
		}
	}

	public void Interact()
	{
		if (_isButtonSingle)
		{
			if (!_lightSwitchController.IsLightTurnedOn)
			{
				_lightSwitchController.TurnOn();
			}
			else
			{
				_lightSwitchController.TurnOff();
			}
		}
		else
		{
			if (_isThisTurnOnButton)
			{
				_lightSwitchController.TurnOn();
			}
			else
			{
				_lightSwitchController.TurnOff();
			}
		}
	}

	public void InteractCutscene()
	{
		Interact();
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (_isThisTurnOnButton)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOn", gameObject.name)}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("UI_HUD_Interaction_HintMessage_Action_TurnOff", gameObject.name)}";
		}
	}

	public void Electrify(float damage)
	{
		if (!_lightSwitchController.IsLightTurnedOn)
		{
			_lightSwitchController.TurnOn();
		}
	}
}
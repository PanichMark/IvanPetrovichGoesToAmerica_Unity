using UnityEngine;

public class InteractionObjectElectricalPanel : MonoBehaviour, IInteractable, IElectroShockable
{
	public string InteractionObjectNameSystem => "InteractionObject_ElectricalPanel";

	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";

	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";
	private LocalizationManager _localizationManager;
	public string InteractionHintMessageAction => $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Interact")}";

	public bool IsOutOfService;
	private string _interactionHintMessageFail;
	public string InteractionHintMessageFail => $"{_interactionHintMessageFail}!";
	private float _health;
	public bool IsInteractionHintMessageFailActive => true;

	public event IInteractable.InteractableObjectHandler OnInteract;

	private PlayerResourcesHealthManager _playerResourcesHealthManager;

	private void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_playerResourcesHealthManager = ServiceLocator.Resolve<PlayerResourcesHealthManager>("PlayerResourcesHealthManager");

		if (IsOutOfService)
		{
			_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_OutOfService");
		}
		else
		{
			_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_ElectroShock");
		}

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public void Interact()
	{
		if (!IsOutOfService)
		{
			_playerResourcesHealthManager.TakeDamage(5);
		}
	}

	public void InteractCutscene()
	{
		//throw new System.NotImplementedException();
	}

	public void Electrify(float damage)
	{
		if (!IsOutOfService)
		{
			_health -= damage;

			if (_health <= 0)
			{
				_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_OutOfService");

				IsOutOfService = true;
			}
		}
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		if (IsOutOfService)
		{
			_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_OutOfService");
		}
		else
		{
			_interactionHintMessageFail = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Fail_ElectroShock");
		}
	}
}

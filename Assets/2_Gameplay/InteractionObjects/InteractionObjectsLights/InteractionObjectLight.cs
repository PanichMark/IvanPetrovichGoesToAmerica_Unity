using UnityEngine;

public class InteractionObjectLight : MonoBehaviour, IInteractable, IElectroShockable
{
	[SerializeField] private string _interactionObjectNameSystem;
	protected LocalizationManager _localizationManager;
	public string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public string InteractionObjectNameUI => $"{_localizationManager.GetLocalizedString(InteractionObjectNameSystem)}";
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}";
	public string InteractionHintMessageFail => null;
	public bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction {  get; private set; }
	public event IInteractable.InteractableObjectHandler OnInteract;

	[SerializeField] private GameObject _lightObject;
	private bool _isLightTurnedOn = false;
	private Material _lightMaterial;
	[SerializeField] private Color _lightEmissionColor = Color.white;

	void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		_lightMaterial = _lightObject.GetComponent<Renderer>().material;
		_lightMaterial.SetColor("_EmissionColor", Color.black);
		if (_isLightTurnedOn)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Turnoff");
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn");
		}
		_localizationManager.OnLanguageChanged += ChangeLanguage;

	}

	public void Interact()
	{
		if (_isLightTurnedOn)
		{
			TurnOff();
		}
		else
		{
			TurnOn();
		}
	}

	private void TurnOn()
	{
		_isLightTurnedOn = true;
		_lightMaterial.SetColor("_EmissionColor", _lightEmissionColor);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff");
		Debug.Log("Light turned on.");
	}

	private void TurnOff()
	{
		_isLightTurnedOn = false;
		_lightMaterial.SetColor("_EmissionColor", Color.black);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn");
		Debug.Log("Light turned off.");
	}

	public void InteractCutscene()
	{
		Interact();
	}

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		if (_isLightTurnedOn)
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff");
		}
		else
		{
			InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn");
		}
	}

	public void Electrify(float damage)
	{
		if (!_isLightTurnedOn)
		{
			TurnOn();
		}
	}
}
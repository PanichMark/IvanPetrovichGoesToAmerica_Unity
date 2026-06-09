using UnityEngine;
using System.Collections.Generic;

public class InteractionObjectLightSwitchButton : MonoBehaviour, IInteractable, IElectroShockable
{
	private InteractionObjectLightSwitchController _lightSwitchController;

	[SerializeField] private string _interactionObjectNameSystem;
	[SerializeField] private bool _isThisTurnOnButton = true;
	private LocalizationManager _localizationManager;
	private List<Material> _lightMaterialsList = new List<Material>();
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
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		_lightSwitchController = GetComponentInParent<InteractionObjectLightSwitchController>();

		_lightMaterialsList.Clear();

		foreach (var obj in _lightSwitchController.LightObjectsList)
		{
			if (obj == null) continue;

			Renderer renderer = obj.GetComponent<Renderer>();
			if (renderer != null)
			{
				_lightMaterialsList.Add(renderer.material);
			}
		}
		_localizationManager.OnLanguageChanged += ChangeLanguage;
		if (_isThisTurnOnButton)
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff")}";
		}
	}

	public void Interact()
	{
		Color emissionColor = _lightSwitchController.LightEmissionColor;

		for (int i = 0; i < _lightMaterialsList.Count; i++)
		{
			if (_lightMaterialsList[i] == null) continue;

			if (_isThisTurnOnButton)
			{
				_lightMaterialsList[i].SetColor("_EmissionColor", emissionColor);
			}
			else
			{
				_lightMaterialsList[i].SetColor("_EmissionColor", Color.black);
			}

			_lightMaterialsList[i].DisableKeyword("_EMISSION");
			_lightMaterialsList[i].EnableKeyword("_EMISSION");
			_lightMaterialsList[i].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
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
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOn")}";
		}
		else
		{
			_interactionHintMessageAction = $"{_localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_TurnOff")}";
		}
	}

	public void Electrify()
	{
		//throw new System.NotImplementedException();
	}
}
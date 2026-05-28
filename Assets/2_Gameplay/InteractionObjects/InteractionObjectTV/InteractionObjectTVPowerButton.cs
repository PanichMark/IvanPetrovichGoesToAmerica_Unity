using UnityEngine;

public class InteractionObjectTVPowerButton : MonoBehaviour, IInteractable
{
	public delegate void TVpowerEventHandler();
	public event TVpowerEventHandler OnTurnTVon;
	public event TVpowerEventHandler OnTurnTVoff;

	private string _interactionHintMessageAction;

	public string InteractionObjectNameSystem => null;

	public string InteractionObjectNameUI => null;

	public string InteractionHintMessageMain => $"{_interactionHintMessageAction}?";
	public event IInteractable.InteractableObjectHandler OnInteract;

	private GameObject _tvScreen;

	public string InteractionHintMessageAction => _interactionHintMessageAction;

	private bool _isTVturnedOn;

	private LocalizationManager _localizationManager;
	public string InteractionHintMessageFail => null;

	public bool IsInteractionHintMessageFailActive => false;

	void Start()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerON");

		_tvScreen = transform.parent.Find("TVcanvas").gameObject;
		_tvScreen.SetActive(false);
		_isTVturnedOn = false;

		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public void Interact()
	{
		if (_isTVturnedOn)
		{
			_tvScreen.SetActive(false);
			_isTVturnedOn = false;

			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerON");
			OnTurnTVoff?.Invoke();
		}
		else
		{
			_tvScreen.SetActive(true);
			_isTVturnedOn = true;

			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerOFF");
			OnTurnTVon?.Invoke();
		}
	}

	public void InteractCutscene()
	{
		Interact();
	}

	private void ChangeLanguage()
	{
		if (_isTVturnedOn)
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerOFF");
		}
		else
		{
			_interactionHintMessageAction = _localizationManager.GetLocalizedString("InteractionObject_TVbutton_PowerON");
		}
	}
}
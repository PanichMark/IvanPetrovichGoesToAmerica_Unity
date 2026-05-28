using UnityEngine;

public class InteractionObjectTVButtonChannel : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();
	private InteractionObjectTVPowerButton _interactionObjectTVPowerbutton;
	[SerializeField] private string _interactionObjectNameSystem;
	private InteractionObjectTVController _tvController;
	[SerializeField] private bool _isNextChannel;
	private LocalizationManager _localizationManager;

	public event IInteractable.InteractableObjectHandler OnInteract;
	private Collider _collider;
	public string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public string InteractionObjectNameUI => _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
	public string InteractionHintMessageMain => $"{InteractionObjectNameUI}?";
	public string InteractionHintMessageAction => null;
	public string InteractionHintMessageFail => null;
	public bool IsInteractionHintMessageFailActive => false;

	private void Start()
	{
		_collider = GetComponent<Collider>();
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_interactionObjectTVPowerbutton = transform.parent.Find("ButtonPower").GetComponent<InteractionObjectTVPowerButton>();
		_tvController = transform.parent.GetComponent<InteractionObjectTVController>();
		DisableButtonChannel();
		_interactionObjectTVPowerbutton.OnTurnTVon += EnableButtonChannel;
		_interactionObjectTVPowerbutton.OnTurnTVoff += DisableButtonChannel;
	}

	public void Interact()
	{
		_tvController.SwitchChannel(_isNextChannel);
	}

	public void InteractCutscene()
	{
		Interact();
	}

	private void EnableButtonChannel()
	{
		_collider.enabled = true;
	}

	private void DisableButtonChannel()
	{
		_collider.enabled = false;
	}
}
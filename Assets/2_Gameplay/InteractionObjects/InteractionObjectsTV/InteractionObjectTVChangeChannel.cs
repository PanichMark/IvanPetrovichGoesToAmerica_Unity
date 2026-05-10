using UnityEngine;

public class InteractionObjectTVButtonChannel : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] private string _buttonChannelUI;
	private InteractionObjectTVController _tvController;
	[SerializeField] private bool _isNextChannel;

	public string InteractionObjectNameSystem => "buttonChannel";
	public string InteractionObjectNameUI => _buttonChannelUI;
	public string InteractionHintMessageMain => $"Press {_buttonChannelUI}?";
	public string InteractionHintAction => "Switch";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;

	public void Interact()
	{
		if (_tvController != null)
		{
			_tvController.SwitchChannel(_isNextChannel);
		}
		else
		{
			Debug.LogError("Error: TVController reference is not set on button " + gameObject.name);
		}
	}

	private void Start()
	{
		_tvController = transform.parent.Find("TVController").gameObject.GetComponent<InteractionObjectTVController>();
	}
}
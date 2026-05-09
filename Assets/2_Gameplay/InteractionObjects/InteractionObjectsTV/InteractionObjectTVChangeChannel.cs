using UnityEngine;

public class InteractionObjectTVButtonChannel : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] private string buttonChannelUI;
	[SerializeField] private InteractionObjectTVController tvController;
	[SerializeField] private bool IsNextChannel;

	public string InteractionObjectNameSystem => "buttonChannel";
	public string InteractionObjectNameUI => buttonChannelUI;
	public string InteractionHintMessageMain => $"Press {buttonChannelUI}?";
	public string InteractionHintAction => "Switch";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;

	private void Start() { }

	private void Update() { }

	public void Interact()
	{
		if (tvController != null)
		{
			tvController.SwitchChannel(IsNextChannel);
		}
		else
		{
			Debug.LogError("Error: TVController reference is not set on button " + gameObject.name);
		}
	}
}
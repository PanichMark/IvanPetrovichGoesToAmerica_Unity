using UnityEngine;

public class InteractionObjectElevatorButton : MonoBehaviour, IInteractable
{
	[SerializeField] private InteractionObjectElevatorController _elevatorController;
	[SerializeField] private bool _isButtonUp;

	public string InteractionObjectNameSystem => "Elevator Button";
	public string InteractionObjectNameUI => "Button";
	public string InteractionHintMessageMain => "Press to call the elevator";
	public string InteractionHintMessageAction => "Press";
	public string InteractionHintMessageFail => "";
	public bool IsInteractionHintMessageFailActive => false;

	public void Interact()
	{
		if (_elevatorController == null)
		{
			Debug.LogError("Elevator Controller is not assigned to the button!", this);
			return;
		}

		_elevatorController.RequestMove(_isButtonUp);
	}
}
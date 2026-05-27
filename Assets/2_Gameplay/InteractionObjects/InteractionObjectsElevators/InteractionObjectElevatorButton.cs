using UnityEngine;

public class InteractionObjectElevatorButton : MonoBehaviour, IInteractable
{
	[SerializeField] private InteractionObjectElevatorController _targetElevator;
	[SerializeField] private bool _isThisGoUpButton;

	public string InteractionObjectNameSystem => "Elevator Button";
	public string InteractionObjectNameUI => "Button";
	public string InteractionHintMessageMain => "Press to call the elevator";
	public string InteractionHintMessageAction => "Press";
	public string InteractionHintMessageFail => "";
	public bool IsInteractionHintMessageFailActive => false;

	public void Interact()
	{
		if (_targetElevator == null)
		{
			Debug.LogError("Elevator Controller is not assigned to the button!", this);
			return;
		}

		_targetElevator.MoveElevator(_isThisGoUpButton);
	}

	public void InteractCutscene()
	{
		Interact();
	}
}
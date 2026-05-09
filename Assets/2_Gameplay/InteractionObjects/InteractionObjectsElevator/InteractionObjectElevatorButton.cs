using UnityEngine;

public class InteractionObjectElevatorButton : MonoBehaviour, IInteractable
{
	[SerializeField] private InteractionObjectElevatorController elevatorController;
	[SerializeField] private bool isButtonUp;

	public string InteractionObjectNameSystem => "Elevator Button";
	public string InteractionObjectNameUI => "Button";
	public string InteractionHintMessageMain => "Press to call the elevator";
	public string InteractionHintAction => "Press";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;

	public void Interact()
	{
		if (elevatorController == null)
		{
			Debug.LogError("Elevator Controller is not assigned to the button!", this);
			return;
		}

		elevatorController.RequestMove(isButtonUp);
	}
}
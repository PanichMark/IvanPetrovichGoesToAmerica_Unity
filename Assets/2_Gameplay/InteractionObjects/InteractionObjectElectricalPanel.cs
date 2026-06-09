using UnityEngine;

public class InteractionObjectElectricalPanel : MonoBehaviour, IInteractable, IElectroShockable
{
	public string InteractionObjectNameSystem => throw new System.NotImplementedException();

	public string InteractionObjectNameUI => throw new System.NotImplementedException();

	public string InteractionHintMessageMain => throw new System.NotImplementedException();

	public string InteractionHintMessageAction => throw new System.NotImplementedException();

	public string InteractionHintMessageFail => throw new System.NotImplementedException();

	public bool IsInteractionHintMessageFailActive => throw new System.NotImplementedException();

	public event IInteractable.InteractableObjectHandler OnInteract;

	public void Electrify()
	{
		//throw new System.NotImplementedException();
	}

	public void Interact()
	{
		//throw new System.NotImplementedException();
	}

	public void InteractCutscene()
	{
		//throw new System.NotImplementedException();
	}
}

using UnityEngine;

public class InteractionObjectUseless : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => throw new System.NotImplementedException();

	public string InteractionObjectNameUI => throw new System.NotImplementedException();

	public string InteractionHintMessageMain => throw new System.NotImplementedException();

	public string InteractionHintMessageAction => throw new System.NotImplementedException();

	public string InteractionHintMessageFail => throw new System.NotImplementedException();

	public bool IsInteractionHintMessageFailActive => throw new System.NotImplementedException();

	public event IInteractable.InteractableObjectHandler OnInteract;

	public void Interact()
	{
		throw new System.NotImplementedException();
	}

	public void InteractCutscene()
	{
		throw new System.NotImplementedException();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

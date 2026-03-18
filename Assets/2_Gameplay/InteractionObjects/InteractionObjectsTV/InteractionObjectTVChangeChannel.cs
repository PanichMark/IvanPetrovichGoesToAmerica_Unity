using UnityEngine;

public class InteractionObjectTVButtonChannel : MonoBehaviour, IInteractable
{
	[SerializeField] private string buttonChannelUI;
	public string InteractionObjectNameSystem => "buttonChannel";
	
	public string InteractionObjectNameUI => buttonChannelUI;

	public string InteractionHintMessageMain => $"Нажать {buttonChannelUI}?";
	[SerializeField] bool IsNextChannel;
	public string InteractionHintAction => throw new System.NotImplementedException();

	public string InteractionHintMessageAdditional => throw new System.NotImplementedException();

	public bool IsInteractionHintMessageAdditionalActive => false;

	public void Interact()
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

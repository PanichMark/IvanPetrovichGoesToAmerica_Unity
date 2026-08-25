using System.Collections;
using UnityEngine;

public class InteractionObjectBreakable : GameplayObjectSaveLoad, IInteractable, IBreakable
{
	public bool IsObjectDestroyed => throw new System.NotImplementedException();

	public float CurrentDurability => throw new System.NotImplementedException();

	public float DuribilityThreshold => throw new System.NotImplementedException();

	public string InteractionObjectNameSystem => throw new System.NotImplementedException();

	public string InteractionObjectNameUI => throw new System.NotImplementedException();

	public string InteractionHintMessageMain => throw new System.NotImplementedException();

	public string InteractionHintMessageAction => throw new System.NotImplementedException();

	public string InteractionHintMessageFail => throw new System.NotImplementedException();

	public bool IsInteractionHintMessageFailActive => throw new System.NotImplementedException();

	public bool CanObjectBeBroken => throw new System.NotImplementedException();

	public GameObject Normal3Dmodel => throw new System.NotImplementedException();

	public GameObject Damaged3Dmodel => throw new System.NotImplementedException();

	public GameObject Broken3Dmodel => throw new System.NotImplementedException();

	public event IInteractable.InteractableObjectHandler OnInteract;

	public void Interact()
	{
		throw new System.NotImplementedException();
	}

	public void InteractCutscene()
	{
		throw new System.NotImplementedException();
	}

	public IEnumerator ModelBreakingAnimation()
	{
		throw new System.NotImplementedException();
	}

	public void ObjectIsFullyBroken()
	{
		throw new System.NotImplementedException();
	}

	public void TakeBreakDamage(float amount)
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

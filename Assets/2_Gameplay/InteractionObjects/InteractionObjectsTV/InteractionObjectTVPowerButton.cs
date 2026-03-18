using UnityEngine;

public class InteractionObjectTVPowerButton : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => throw new System.NotImplementedException();
	private string PowerButtonName = "Кнопка питания телевизора";
	public string InteractionObjectNameUI => PowerButtonName;

	public string InteractionHintMessageMain => $"Нажать {PowerButtonName}?";
	[SerializeField] GameObject TVscreen;
	public string InteractionHintAction => throw new System.NotImplementedException();
	private bool IsTVturnedOn;
	public string InteractionHintMessageAdditional => throw new System.NotImplementedException();

	public bool IsInteractionHintMessageAdditionalActive => false;

	public void Interact()
	{
		if (IsTVturnedOn)
		{
			TVscreen.SetActive(false);
			IsTVturnedOn = false;
		}
		else
		{
			TVscreen.SetActive(true);
			IsTVturnedOn = true;
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		TVscreen.SetActive(false);
		IsTVturnedOn = false;
    }

 
}

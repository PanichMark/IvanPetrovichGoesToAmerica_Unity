using UnityEngine;

public class InteractionObjectTVPowerButton : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => throw new System.NotImplementedException();

	private string _powerButtonName = "Кнопка питания телевизора";

	public string InteractionObjectNameUI => _powerButtonName;

	public string InteractionHintMessageMain => $"Нажать {_powerButtonName}?";

	private GameObject _tvScreen;

	public string InteractionHintAction => throw new System.NotImplementedException();

	private bool _isTVturnedOn;

	public string InteractionHintMessageAdditional => throw new System.NotImplementedException();

	public bool IsInteractionHintMessageAdditionalActive => false;

	public void Interact()
	{
		if (_isTVturnedOn)
		{
			_tvScreen.SetActive(false);
			_isTVturnedOn = false;
		}
		else
		{
			_tvScreen.SetActive(true);
			_isTVturnedOn = true;
		}
	}

	void Start()
	{
		_tvScreen = transform.parent.Find("CanvasTV").gameObject;
		_tvScreen.SetActive(false);
		_isTVturnedOn = false;
	}
}
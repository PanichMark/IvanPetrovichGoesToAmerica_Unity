using UnityEngine;

public class InteractionObjectLight : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => "LightSwitch";
	public string InteractionObjectNameUI => "Свет";
	public string InteractionHintMessageMain => $"Включить/выключить {InteractionObjectNameUI}";
	public string HintAction => "Взаимодействие";
	public string InteractionHintMessageFail => "";
	public bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction => "";

	[SerializeField] private GameObject _lightObject;
	private bool _isLightTurnedOn = false;
	private Material _lightMaterial;
	[SerializeField] private Color _lightEmissionColor = Color.white;

	void Start()
	{
		_lightMaterial = _lightObject.GetComponent<Renderer>().material;
		_lightMaterial.SetColor("_EmissionColor", Color.black);
	}

	public void Interact()
	{
		if (_isLightTurnedOn)
		{
			_isLightTurnedOn = false;
			_lightMaterial.SetColor("_EmissionColor", Color.black);
			Debug.Log("Light turned off.");
		}
		else
		{
			_isLightTurnedOn = true;
			_lightMaterial.SetColor("_EmissionColor", _lightEmissionColor);
			Debug.Log("Light turned on.");
		}
	}
}
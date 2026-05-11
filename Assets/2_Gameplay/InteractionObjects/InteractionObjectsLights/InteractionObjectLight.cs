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

	public GameObject lightObject;
	private bool _isLightTurnedOn = false;
	private Material _cachedMaterial;
	public Color emissionColorOn = Color.white;

	void Start()
	{
		_cachedMaterial = lightObject.GetComponent<Renderer>().material;
		_cachedMaterial.SetColor("_EmissionColor", Color.black);
	}

	public void Interact()
	{
		if (_isLightTurnedOn)
		{
			_isLightTurnedOn = false;
			_cachedMaterial.SetColor("_EmissionColor", Color.black);
			Debug.Log("Light turned off.");
		}
		else
		{
			_isLightTurnedOn = true;
			_cachedMaterial.SetColor("_EmissionColor", emissionColorOn);
			Debug.Log("Light turned on.");
		}
	}
}
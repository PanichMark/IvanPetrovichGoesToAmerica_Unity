using UnityEngine;

public class InteractionObjectLight : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => "LightSwitch";
	public string InteractionObjectNameUI => "Свет";
	public string InteractionHintMessageMain => $"Включить/выключить {InteractionObjectNameUI}";
	public string HintAction => "Взаимодействие";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;
	public string InteractionHintAction => "";

	public GameObject lightObject;
	private bool isLightTurnedOn = false;
	private Material cachedMaterial;
	public Color emissionColorOn = Color.white;

	void Start()
	{
		cachedMaterial = lightObject.GetComponent<Renderer>().material;
		cachedMaterial.SetColor("_EmissionColor", Color.black);
	}

	public void Interact()
	{
		if (isLightTurnedOn)
		{
			isLightTurnedOn = false;
			cachedMaterial.SetColor("_EmissionColor", Color.black);
			Debug.Log("Light turned off.");
		}
		else
		{
			isLightTurnedOn = true;
			cachedMaterial.SetColor("_EmissionColor", emissionColorOn);
			Debug.Log("Light turned on.");
		}
	}
}
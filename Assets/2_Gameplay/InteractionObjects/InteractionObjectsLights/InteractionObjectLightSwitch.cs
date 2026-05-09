using UnityEngine;
using System.Collections.Generic;

public class InteractionObjectLightSwitch : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] private List<GameObject> lightsList = new List<GameObject>();
	[SerializeField] private bool isButtonActivate = true;

	public Color emissionColorOn = Color.white;

	public string InteractionObjectNameSystem => "LightSwitch";
	public string InteractionObjectNameUI => "Свет";
	public string InteractionHintMessageMain => $"Включить/выключить {InteractionObjectNameUI}";
	public string HintAction => "Взаимодействие";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;
	public string InteractionHintAction => "";

	private List<Material> cachedMaterials = new List<Material>();

	void Start()
	{
		cachedMaterials.Clear();

		foreach (var obj in lightsList)
		{
			if (obj == null) continue;

			Renderer renderer = obj.GetComponent<Renderer>();
			if (renderer != null)
			{
				cachedMaterials.Add(renderer.material);
			}
		}
	}

	public void Interact()
	{
		bool shouldTurnOn = isButtonActivate;
		bool shouldTurnOff = !isButtonActivate;

		for (int i = 0; i < cachedMaterials.Count; i++)
		{
			if (cachedMaterials[i] == null) continue;

			if (shouldTurnOn)
			{
				cachedMaterials[i].SetColor("_EmissionColor", emissionColorOn);
			}
			else if (shouldTurnOff)
			{
				cachedMaterials[i].SetColor("_EmissionColor", Color.black);
			}

			lightsList[i].GetComponent<Renderer>().UpdateGIMaterials();
		}
	}
}
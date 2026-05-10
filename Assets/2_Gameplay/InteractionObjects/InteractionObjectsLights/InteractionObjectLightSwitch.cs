using UnityEngine;
using System.Collections.Generic;

public class InteractionObjectLightSwitch : MonoBehaviour, IInteractable
{
	public delegate void InteractionDelegate();

	[SerializeField] private List<GameObject> _lightsList = new List<GameObject>();
	[SerializeField] private bool _isButtonActivate = true;

	public Color emissionColorOn = Color.white;

	public string InteractionObjectNameSystem => "LightSwitch";
	public string InteractionObjectNameUI => "Свет";
	public string InteractionHintMessageMain => $"Включить/выключить {InteractionObjectNameUI}";
	public string HintAction => "Взаимодействие";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;
	public string InteractionHintAction => "";

	private List<Material> _cachedMaterials = new List<Material>();

	void Start()
	{
		_cachedMaterials.Clear();

		foreach (var obj in _lightsList)
		{
			if (obj == null) continue;

			Renderer renderer = obj.GetComponent<Renderer>();
			if (renderer != null)
			{
				_cachedMaterials.Add(renderer.material);
			}
		}
	}

	public void Interact()
	{
		bool shouldTurnOn = _isButtonActivate;
		bool shouldTurnOff = !_isButtonActivate;

		for (int i = 0; i < _cachedMaterials.Count; i++)
		{
			if (_cachedMaterials[i] == null) continue;

			if (shouldTurnOn)
			{
				_cachedMaterials[i].SetColor("_EmissionColor", emissionColorOn);
			}
			else if (shouldTurnOff)
			{
				_cachedMaterials[i].SetColor("_EmissionColor", Color.black);
			}

			_lightsList[i].GetComponent<Renderer>().UpdateGIMaterials();
		}
	}
}
using UnityEngine;
using System.Collections.Generic;

public class InteractionObjectLightSwitch : MonoBehaviour, IInteractable
{

	[SerializeField] private List<GameObject> _lightObjectsList = new List<GameObject>();
	[SerializeField] private bool _isThisTurnOnButton = true;

	[SerializeField] private Color _lightEmmisionColor = Color.white;

	public string InteractionObjectNameSystem => "LightSwitch";
	public string InteractionObjectNameUI => "Свет";
	public string InteractionHintMessageMain => $"Включить/выключить {InteractionObjectNameUI}";
	public string HintAction => "Взаимодействие";
	public string InteractionHintMessageFail => "";
	public bool IsInteractionHintMessageFailActive => false;
	public string InteractionHintMessageAction => "";

	private List<Material> _lightMaterialsList = new List<Material>();

	void Start()
	{
		_lightMaterialsList.Clear();

		foreach (var obj in _lightObjectsList)
		{
			if (obj == null) continue;

			Renderer renderer = obj.GetComponent<Renderer>();
			if (renderer != null)
			{
				_lightMaterialsList.Add(renderer.material);
			}
		}
	}

	public void Interact()
	{
		bool shouldTurnOn = _isThisTurnOnButton;
		bool shouldTurnOff = !_isThisTurnOnButton;

		for (int i = 0; i < _lightMaterialsList.Count; i++)
		{
			if (_lightMaterialsList[i] == null) continue;

			if (shouldTurnOn)
			{
				_lightMaterialsList[i].SetColor("_EmissionColor", _lightEmmisionColor);
			}
			else if (shouldTurnOff)
			{
				_lightMaterialsList[i].SetColor("_EmissionColor", Color.black);
			}

			_lightObjectsList[i].GetComponent<Renderer>().UpdateGIMaterials();
		}
	}
}
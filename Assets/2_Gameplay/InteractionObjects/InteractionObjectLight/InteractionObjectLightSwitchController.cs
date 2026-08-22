using UnityEngine;
using System.Collections.Generic;

public class InteractionObjectLightSwitchController : MonoBehaviour
{
	[SerializeField] private List<GameObject> _lightObjectsList = new List<GameObject>();
	[SerializeField] private Color _lightEmissionColor = Color.white;

	public List<GameObject> LightObjectsList => _lightObjectsList;
	public Color LightEmissionColor => _lightEmissionColor;
}
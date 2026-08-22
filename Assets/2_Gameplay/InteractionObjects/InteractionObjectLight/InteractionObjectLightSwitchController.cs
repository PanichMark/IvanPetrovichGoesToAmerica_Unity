using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectLightSwitchController : InteractionObjectLightAbstract
{
	public override void TurnOn()
	{
		IsLightTurnedOn = true;
		ApplyEmission(LightEmissionColor);
	}

	public override void TurnOff()
	{
		IsLightTurnedOn = false;
		ApplyEmission(Color.black);
	}

	public List<GameObject> LightObjectsList => _lightObjectsList;
	public Color LightEmissionColor => _lightEmissionColor;
}
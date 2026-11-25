using UnityEngine;

public class InteractionObjectPickableGrenade : InteractionObjectPickableThrowableUndestructable
{
	[SerializeField] private float _explosionRadius;
	[SerializeField] private bool _explodeOnImpact;
}
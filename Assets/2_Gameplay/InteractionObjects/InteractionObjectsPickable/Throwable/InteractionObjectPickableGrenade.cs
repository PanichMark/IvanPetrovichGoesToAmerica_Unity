using UnityEngine;

public class InteractionObjectPickableGrenade : InteractionObjectPickableThrowable
{
	[SerializeField] private float _explosionRadius;
	[SerializeField] private bool _explodeOnImpact;
}
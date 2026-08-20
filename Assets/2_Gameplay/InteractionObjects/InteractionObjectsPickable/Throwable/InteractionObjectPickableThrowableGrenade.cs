using UnityEngine;

public class InteractionObjectPickableThrowableGrenade : InteractionObjectPickableThrowable
{
	[SerializeField] private float _explosionRadius;
	[SerializeField] private bool _explodeOnImpact;
}
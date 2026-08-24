using UnityEngine;

public class InteractionObjectPickableGrenade : InteractionObjectPickableThrowableAbstract
{
	[SerializeField] private float _explosionRadius;
	[SerializeField] private bool _explodeOnImpact;
}
using UnityEngine;

public class InteractionObjectPickableNonThrowableChickenCage : InteractionObjectPickableNonThrowableAbstract, IDamageable
{
	public bool CanObjectBeDamaged => throw new System.NotImplementedException();

	public float CurrentHealth => throw new System.NotImplementedException();

	public void ObjectIsFullyDamaged()
	{
		throw new System.NotImplementedException();
	}

	public void TakeDamage(float amount)
	{
		throw new System.NotImplementedException();
	}
}

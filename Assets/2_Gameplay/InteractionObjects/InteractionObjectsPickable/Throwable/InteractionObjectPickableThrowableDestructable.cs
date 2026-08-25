using System.Collections;
using UnityEngine;

public class InteractionObjectPickableThrowableDestructable : InteractionObjectPickableThrowableAbstract, IDamageable, IBreakable
{
	[Header("Object Health")]
	[SerializeField] protected float _health;
	[SerializeField] private bool _canBeDamaged;
	[SerializeField] private float _breakingThreshold;

	public float CurrentDurability => _health;

	public float DuribilityThreshold => _breakingThreshold;

	public bool CanObjectBeBroken => true;

	public float CurrentHealth => _health;

	public bool CanObjectBeDamaged => _canBeDamaged;

	public GameObject Normal3Dmodel => throw new System.NotImplementedException();

	public GameObject Damaged3Dmodel => throw new System.NotImplementedException();

	public GameObject Broken3Dmodel => throw new System.NotImplementedException();

	public void TakeDamage(float amount)
	{
		if (CanObjectBeDamaged)
		{
			Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {CurrentHealth - amount}");

			_health -= amount;

			if (_health <= 0)
			{
				ObjectIsFullyDamaged();
			}
		}
	}

	public virtual void TakeBreakDamage(float amount)
	{
		if (CanObjectBeBroken)
		{
			if (amount >= DuribilityThreshold)
			{
				_health -= amount;

				if (_health <= 0)
				{
					ObjectIsFullyBroken();
				}
			}
		}
	}

	public void ObjectIsFullyBroken()
	{
		_isObjectDestroyed = true;

		gameObject.SetActive(false);
	}

	public IEnumerator ModelBreakingAnimation()
	{
		throw new System.NotImplementedException();
	}
}

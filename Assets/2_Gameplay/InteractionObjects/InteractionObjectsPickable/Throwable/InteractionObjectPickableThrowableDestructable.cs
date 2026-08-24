using UnityEngine;

public class InteractionObjectPickableThrowableDestructable : InteractionObjectPickableThrowableAbstract, IDamageable, IBreakable
{
	[Header("Object Health")]
	[SerializeField] protected float _health;
	[SerializeField] private bool _canBeDamaged;
	[SerializeField] protected bool _canBeBroken;
	[SerializeField] private float _breakingThreshold;

	public float CurrentDurability => _health;

	public float DuribilityThreshold => _breakingThreshold;

	public bool CanObjectBeBroken => _canBeBroken;

	public float CurrentHealth => _health;

	public bool CanObjectBeDamaged => _canBeDamaged;

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
}

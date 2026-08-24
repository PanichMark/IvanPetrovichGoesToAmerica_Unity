using UnityEngine;

public class InteractionObjectPickableNonThrowableDestructable : InteractionObjectPickableNonThrowableAbstract, IBreakable
{
	[Header("Object Health")]
	[SerializeField] protected float _health;
	[SerializeField] private float _breakingThreshold;

	public float CurrentDurability => _health;

	public float DuribilityThreshold => _breakingThreshold;

	public bool CanObjectBeBroken => true;


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

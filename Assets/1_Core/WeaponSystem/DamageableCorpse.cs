using UnityEngine;

public class DamageableCorpse : MonoBehaviour, IDamageable
{
	public bool IsObjectDestroyed => throw new System.NotImplementedException();

	public float CurrentHealth => throw new System.NotImplementedException();

	public void ObjectIsFullyDamaged()
	{
		//throw new System.NotImplementedException();
	}

	public void TakeDamage(float amount)
	{
		//throw new System.NotImplementedException();
	}
}

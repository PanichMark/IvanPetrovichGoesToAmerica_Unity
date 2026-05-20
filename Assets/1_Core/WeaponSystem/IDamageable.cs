public interface IDamageable
{
	bool WasObjectDestroyed { get; }
	float CurrentHealth { get; }

	void TakeDamage(float amount);

	void ObjectIsFullyDamaged();
}
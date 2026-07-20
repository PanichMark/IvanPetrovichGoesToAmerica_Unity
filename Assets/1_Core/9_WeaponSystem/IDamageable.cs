public interface IDamageable
{
	bool IsObjectDestroyed { get; }
	float CurrentHealth { get; }

	void TakeDamage(float amount);

	void ObjectIsFullyDamaged();
}
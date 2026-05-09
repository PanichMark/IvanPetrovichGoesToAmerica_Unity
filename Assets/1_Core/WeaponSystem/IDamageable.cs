public interface IDamageable
{
	bool WasObjectDestroyed { get; }
	float Health { get; }

	void TakeDamage(float amount);

	void ObjectIsFullyDamaged();
}
public interface IDamageable
{
	bool CanObjectBeDamaged {  get; }
	bool IsObjectDestroyed { get; }
	float CurrentHealth { get; }

	void TakeDamage(float amount);

	void ObjectIsFullyDamaged();
}
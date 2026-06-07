public interface IBreakable
{
	bool IsObjectBroken { get; }
	float CurrentDurability { get; }
	float DamageThreshold { get; }

	void TakeDamage(float amount);

	void ObjectIsFullyBroken();
}
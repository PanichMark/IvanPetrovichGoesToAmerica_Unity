public interface IBreakable
{
	bool CanObjectBeBroken {  get; }
	bool IsObjectDestroyed { get; }
	float CurrentDurability { get; }
	float DuribilityThreshold { get; }

	void TakeBreakDamage(float amount);

	void ObjectIsFullyBroken();
}
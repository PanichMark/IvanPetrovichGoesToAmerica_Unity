using UnityEngine;

public class InteractionObjectOpenableDoorBreakable : InteractionObjectOpenableDoorUnbreakable, IBreakable
{
	[SerializeField] private float _maxDurability = 100f;
	[SerializeField] private float _damageThreshold = 50f;

	public bool IsObjectBroken => CurrentDurability <= 0f;
	public float CurrentDurability { get; private set; }
	public float DamageThreshold => _damageThreshold;

	private void Awake()
	{
		CurrentDurability = _maxDurability;
	}

	public void TakeDamage(float amount)
	{
		// Проверка на порог урона
		if (amount < _damageThreshold)
		{
			Debug.Log($"Недостаточно урона для break. Требуется: {_damageThreshold}, получено: {amount}");
			return;
		}

		// Нанесение урона
		CurrentDurability -= amount;
		Debug.Log($"Нанесено урона: {amount}. Осталось прочности: {CurrentDurability}");

		// Проверка на разрушение
		if (CurrentDurability <= 0f)
		{
			ObjectIsFullyBroken();
		}
	}

	public void ObjectIsFullyBroken()
	{
		Debug.Log("Был broke!");
		Destroy(gameObject);
	}
}
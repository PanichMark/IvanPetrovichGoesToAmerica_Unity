public interface IDamageable
{
	// Текущее количество здоровья персонажа
	bool WasObjectDestroyed { get; }
	float Health { get; }

	// Метод для нанесения урона объекту
	void TakeDamage(float amount);

	// Метод, вызываемый при гибели объекта
	void ObjectIsFullyDamaged();
}


using UnityEngine;

public class WeaponUnlocker : MonoBehaviour
{
	public WeaponController weaponController; // Контроллер оружия

	void Update()
	{
		// Обрабатываем нажатие цифровых клавиш от 2 до 9
		for (int i = 2; i <= 9; i++)
		{
			if (Input.GetKeyDown((KeyCode)(KeyCode.Alpha2 + i - 2)))
			{
				// Получаем префаб оружия по индексу
				GameObject weaponPrefab = GetWeaponPrefabByIndex(i);

				if (weaponPrefab != null)
				{
					weaponController.UnlockWeapon(weaponPrefab); // Разблокировка оружия
				}
				else
				{
					Debug.LogWarning($"Префаб оружия для индекса {i} не найден!");
				}
			}
		}
	}

	// Возвращает префаб оружия по заданному индексу
	private GameObject GetWeaponPrefabByIndex(int index)
	{
		switch (index)
		{
			case 2:
				return Resources.Load<GameObject>("MeleePoliceBaton_0"); // Ваш путь к префабу оружия
			case 3:
				return Resources.Load<GameObject>("RangedHarmonicaRevolver_3");
			case 4:
				return Resources.Load<GameObject>("RangedSawedoffShotgun_5");
			case 5:
				return Resources.Load<GameObject>("RangedPlungerCrossbow_6");
			case 6:
				return Resources.Load<GameObject>("EugenicGenie_9");
			default:
				return null;
		}
	}
}

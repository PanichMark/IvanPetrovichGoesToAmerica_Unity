using UnityEngine;

public class WeaponUnlocker : MonoBehaviour
{
	public WeaponController weaponController; // Ссылка на контроллер оружия

	void Update()
	{
		// Обработка нажатия цифровых клавиш от 2 до 9
		for (int i = 2; i <= 9; i++)
		{
			if (Input.GetKeyDown((KeyCode)(KeyCode.Alpha2 + i - 2))) // Alpha2 соответствует клавише "2"
			{
				// Определяем имя оружия в формате "Категория_ИмяОружия_Индекс"
				string weaponName = GetWeaponNameByIndex(i);

				// Загружаем данные оружия из ScriptableObject
				WeaponData weaponData = Resources.Load<WeaponData>($"WeaponWheelButtons/{weaponName}");

				if (weaponData != null)
				{
					weaponController.UnlockWeapon(weaponData.WeaponPrefab); // Вызываем разблокировку оружия
				}
				else
				{
					Debug.LogWarning($"Данные оружия '{weaponName}' не найдены!");
				}
			}
		}
	}

	// Метод для получения имени оружия по индексу
	private string GetWeaponNameByIndex(int index)
	{
		switch (index)
		{
			case 2:
				return "MeleePoliceBaton_0";
			case 3:
				return "RangedHarmonicaRevolver_3";
			case 4:
				return "RangedPlungerCrossbow_6";
			case 5:
				return "EugenicGenie_9";
			default:
				return "";
		}
	}
}
using UnityEngine;

public class NewWeaponController : MonoBehaviour
{
	public DynamicWeaponWheel weaponWheel; // Ссылка на объект DynamicWeaponWheel

	void Update()
	{
		// Обработка ввода стрелок
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			weaponWheel.AddNewSegment();  // Добавляем новый сегмент
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			RemoveLastSegment();          // Убираем последний сегмент
		}
	}

	// Метод удаления последнего сегмента
	void RemoveLastSegment()
	{
		if (weaponWheel.numberOfSegments > 0)
		{
			weaponWheel.numberOfSegments--;
			weaponWheel.RecreateWheel();  // Пересоздаем колесо
		}
	}
}
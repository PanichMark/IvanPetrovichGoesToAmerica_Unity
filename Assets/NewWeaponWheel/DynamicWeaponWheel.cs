using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWeaponWheel : MonoBehaviour
{
	public GameObject wheelSegmentPrefab; // Prefab отдельного сегмента (кнопки)
	public Transform centerPoint;         // Центр кольца
	public float radius = 100f;          // Радиус окружности кнопок

	// Единый словарь для разблокированных видов оружия
	// Ключ: строка "ИмяОружия_Индекс", значение: объект оружия
	public Dictionary<string, GameObject> UnlockedWeapons = new Dictionary<string, GameObject>();

	private List<GameObject> wheelSegments = new List<GameObject>();

	public event System.Action<int> OnSegmentSelected; // Сигнал о выборе сегмента

	// Метод предварительной подготовки (добавляем оружие в словарь)
	void PreloadWeapons()
	{
		// Загружаем префабы из ресурсов
	

		// Добавляем оружие в словарь с ключами в формате "ИмяОружия_Индекс"
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWheelButtons/PoliceButton"), "PoliceBaton", 0);
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWheelButtons/Revolver"), "HarmonicaRevolver", 1);
		AddUnlockedWeapon(Resources.Load<GameObject>("WeaponWheelButtons/EugenicGenie"), "FireballSpell", 5);
	}

	// Вызов метода предварительной подготовки
	void Start()
	{
		PreloadWeapons(); // Предварительно заполняем словарь оружием
		CreateWheel();    // Создаём колесо оружия
	}

	// Метод добавления разблокированного оружия
	public void AddUnlockedWeapon(GameObject weaponPrefab, string weaponName, int index)
	{
		// Создаем ключ в формате "ИмяОружия_Индекс"
		string key = $"{weaponName}_{index}";

		// Добавляем оружие в словарь
		UnlockedWeapons[key] = weaponPrefab;

		RecreateWheel();                  // Пересоздание колеса
	}

	public void RecreateWheel()
	{
		foreach (var seg in wheelSegments)
			Destroy(seg.gameObject);      // Удаляем предыдущие сегменты

		wheelSegments.Clear();            // Чистка списка

		CreateWheel();                    // Формирование нового колеса
	}

	void CreateWheel()
	{
		// Собираем список разблокированных видов оружия
		List<GameObject> activeWeapons = CollectActiveWeapons();

		if (activeWeapons.Count == 0)
			return;                       // Нельзя создать колесо без оружия

		float angleStep = 360f / activeWeapons.Count; // Угловое расстояние между сегментами

		for (int i = 0; i < activeWeapons.Count; i++)
		{
			GameObject segmentInstance = Instantiate(wheelSegmentPrefab);
			segmentInstance.transform.SetParent(centerPoint.parent); // Родитель — тот же, что и у центра
			segmentInstance.name = $"Segment {i + 1}";

			// Настраиваем иконку и реакцию на нажатие кнопки
			var button = segmentInstance.GetComponent<Button>();
			button.image.sprite = activeWeapons[i].GetComponent<SpriteRenderer>().sprite; // Загрузка иконки оружия
			button.onClick.AddListener(() => OnSegmentSelected?.Invoke(i)); // Отправляем индекс кнопки

			// Вычисляем позицию кнопки на окружности
			float adjustedAngle = i * angleStep + 90f; // Учтем сдвиг на -90 градусов
			Vector3 positionOnCircle = CalculatePositionOnCircle(adjustedAngle, radius);
			segmentInstance.transform.position = centerPoint.position + positionOnCircle;

			wheelSegments.Add(segmentInstance); // Запоминаем новый сегмент
		}
	}

	// Собираем активный список оружия, исключая пустые позиции
	public List<GameObject> CollectActiveWeapons()
	{
		return new List<GameObject>(UnlockedWeapons.Values);
	}

	// Функция для вычисления позиции на окружности
	Vector3 CalculatePositionOnCircle(float angleInDegrees, float radius)
	{
		float x = Mathf.Cos(Mathf.Deg2Rad * angleInDegrees) * radius;
		float y = Mathf.Sin(Mathf.Deg2Rad * angleInDegrees) * radius;
		return new Vector3(x, y, 0f);
	}
}

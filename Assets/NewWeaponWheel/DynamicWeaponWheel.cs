using System.Collections.Generic;
using UnityEngine;

public class DynamicWeaponWheel : MonoBehaviour
{
	public GameObject wheelSegmentPrefab; // Prefab отдельного сегмента (кнопки)
	public Transform centerPoint;         // Центр кольца
	public int numberOfSegments = 1;     // Текущее количество сегментов
	public float radius = 100f;          // Радиус окружности кнопок

	private List<GameObject> wheelSegments = new List<GameObject>();

	void Start()
	{
		CreateWheel();                    // Создание изначального колеса
	}

	public void AddNewSegment()
	{
		if (numberOfSegments >= 8) // Проверка ограничения на максимум 10 сегментов
			return;

		numberOfSegments++;           // Увеличение количества сегментов
		RecreateWheel();              // Пересоздание колеса
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
		if (numberOfSegments <= 0 || numberOfSegments > 10)
			return;                       // Проверка диапазона количества сегментов

		float angleStep = 360f / numberOfSegments; // Угловое расстояние между сегментами

		for (int i = 0; i < numberOfSegments; i++)
		{
			GameObject segmentInstance = Instantiate(wheelSegmentPrefab);
			segmentInstance.transform.SetParent(centerPoint.parent); // Родитель — тот же, что и у центра
			segmentInstance.name = $"Segment {i + 1}";

			// Вычисляем позицию кнопки на окружности, предварительно уменьшив угол на 90 градусов
			float adjustedAngle = i * angleStep + 90f; // Учтем сдвиг на -90 градусов
			Vector3 positionOnCircle = CalculatePositionOnCircle(adjustedAngle, radius);
			segmentInstance.transform.position = centerPoint.position + positionOnCircle;

			wheelSegments.Add(segmentInstance); // Запоминаем новый сегмент
		}
	}

	// Функция для вычисления позиции на окружности
	Vector3 CalculatePositionOnCircle(float angleInDegrees, float radius)
	{
		float x = Mathf.Cos(Mathf.Deg2Rad * angleInDegrees) * radius;
		float y = Mathf.Sin(Mathf.Deg2Rad * angleInDegrees) * radius;
		return new Vector3(x, y, 0f);
	}
}
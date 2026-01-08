using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LockController : MonoBehaviour, IInteractable
{
	[SerializeField]
	private GameObject gearPrefab;

	// Количество сегментов вращения (задается в инспекторе)
	[SerializeField]
	private int segmentsCount;

	// Скорость вращения (радианы в секунду)
	[SerializeField]
	private float rotationSpeed;

	// Скорость перемещения (метры в секунду)
	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private GameObject CubeFollow;

	public string InteractionObjectNameSystem => throw new System.NotImplementedException();

	public string MainInteractionHint => $"Вскрыть {InteractionObjectNameUI}";

	public string AdditionalInteractionHint => throw new System.NotImplementedException();

	public bool IsAdditionalInteractionHintActive => false;

	public string InteractionObjectNameUI => interactionObjectNameUI;

	[SerializeField]
	private string interactionObjectNameUI;

	[SerializeField]
	private MenuManager menuManager;

	// Флаг блокировки взаимодействия
	private bool isMovingOrRotating = false;

	// Экземпляры объектов
	private GameObject currentGearInstance;
	private GameObject currentCubeFollow;

	// Угловой шаг поворота
	private float rotationStep;

	// Шаг перемещения
	private float movementStep;

	// Детекторы зон коллизий (берутся автоматически из префаба CubeFollow)
	private MeshCollider UpZoneCollider;     // Верхняя зона
	private MeshCollider DownZoneCollider;   // Нижняя зона
	private MeshCollider LeftZoneCollider;   // Левая зона
	private MeshCollider RightZoneCollider;  // Правая зона

	// Доступность направлений движения
	private bool _canMoveUp = true;
	private bool _canMoveDown = true;
	private bool _canMoveLeft = true;
	private bool _canMoveRight = true;
	/*
	private void OnDrawGizmos()
	{
		if (currentGearInstance != null && Application.isPlaying)
		{
			// Получаем все коллайдеры дочерних объектов
			MeshCollider[] childColliders = currentGearInstance.GetComponentsInChildren<MeshCollider>();

			// Отображаем каждый коллайдер зеленого цвета
			Gizmos.color = Color.green;

			foreach (MeshCollider col in childColliders)
			{
				// Получаем саму сетку и рисуем её
				Gizmos.DrawWireMesh(col.sharedMesh, col.transform.position, col.transform.rotation, col.transform.lossyScale);
			}

			// Теперь рисуем остальные зоны (они тоже MeshCollider)
			if (UpZoneCollider != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(((MeshCollider)UpZoneCollider).sharedMesh, ((MeshCollider)UpZoneCollider).transform.position, ((MeshCollider)UpZoneCollider).transform.rotation, ((MeshCollider)UpZoneCollider).transform.lossyScale);
			}

			if (DownZoneCollider != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(((MeshCollider)DownZoneCollider).sharedMesh, ((MeshCollider)DownZoneCollider).transform.position, ((MeshCollider)DownZoneCollider).transform.rotation, ((MeshCollider)DownZoneCollider).transform.lossyScale);
			}

			if (LeftZoneCollider != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(((MeshCollider)LeftZoneCollider).sharedMesh, ((MeshCollider)LeftZoneCollider).transform.position, ((MeshCollider)LeftZoneCollider).transform.rotation, ((MeshCollider)LeftZoneCollider).transform.lossyScale);
			}

			if (RightZoneCollider != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(((MeshCollider)RightZoneCollider).sharedMesh, ((MeshCollider)RightZoneCollider).transform.position, ((MeshCollider)RightZoneCollider).transform.rotation, ((MeshCollider)RightZoneCollider).transform.lossyScale);
			}
		}
	}
	*/
	public void Interact()
	{
		menuManager.OpenInteractionMenu(gameObject); // Передача самого объекта сюда!

		// Создаем экземпляр шестерёнки
		currentGearInstance = Instantiate(gearPrefab, GetPuzzleSpawnPosition(), Quaternion.identity);
		currentGearInstance.transform.LookAt(Camera.main.transform); // Поворачиваем к камере
		currentGearInstance.transform.Translate(-0.05f, 0f, 0f, Space.Self);
		

		gameObject.tag = "Untagged";

		// Создаем экземпляр куба
		currentCubeFollow = Instantiate(CubeFollow, GetCubeSpawnPosition(), Quaternion.identity);
		currentCubeFollow.transform.LookAt(Camera.main.transform);
		


		// Простым поиском находим детекторы зон коллизий
		Transform root = currentCubeFollow.transform;
		UpZoneCollider = root.Find("UpZone")?.GetComponent<MeshCollider>();
		DownZoneCollider = root.Find("DownZone")?.GetComponent<MeshCollider>();
		LeftZoneCollider = root.Find("LeftZone")?.GetComponent<MeshCollider>();
		RightZoneCollider = root.Find("RightZone")?.GetComponent<MeshCollider>();

		CheckForIntersection();

		// Проверяем наличие детекторов
		if (UpZoneCollider == null || DownZoneCollider == null ||
			LeftZoneCollider == null || RightZoneCollider == null)
		{
			Debug.LogWarning("Не удалось присвоить детекторы зон!");
		}

		// Рассчитываем шаги вращения и перемещения
		rotationStep = 360f / segmentsCount;
		movementStep = 0.1f; // Например, расстояние шага в метрах
	}

	private void Update()
	{
		if (!isMovingOrRotating && currentGearInstance != null)
		{
			// Движение возможно только в открытых направлениях
			if (_canMoveUp && Input.GetKeyDown(KeyCode.UpArrow))
			{
				StartCoroutine(RotateGear(rotationStep));
			}
			else if (_canMoveDown && Input.GetKeyDown(KeyCode.DownArrow))
			{
				StartCoroutine(RotateGear(-rotationStep));
			}
			else if (_canMoveRight && Input.GetKeyDown(KeyCode.RightArrow))
			{
				StartCoroutine(MoveRight());
			}
			else if (_canMoveLeft && Input.GetKeyDown(KeyCode.LeftArrow))
			{
				StartCoroutine(MoveLeft());
			}
		}
	}



	private void CheckForIntersection()
	{
		Physics.SyncTransforms();

		if (currentCubeFollow != null && currentGearInstance != null)
		{
			// Получаем все дочерние коллайдеры шестерёнки (только MeshCollider!)
			MeshCollider[] childColliders = currentGearInstance.GetComponentsInChildren<MeshCollider>();

			// Начальные условия — считаем, что пока все направления открыты
			_canMoveUp = true;
			_canMoveDown = true;
			_canMoveLeft = true;
			_canMoveRight = true;

			// Проходим по каждому дочернему коллайдеру шестерёнки
			foreach (var gearCollider in childColliders)
			{
				// Проверяем зоны на пересечения (полигональные коллайдеры)
				if (IsIntersectingWithCollider(UpZoneCollider, gearCollider)) _canMoveUp = false;
				if (IsIntersectingWithCollider(DownZoneCollider, gearCollider)) _canMoveDown = false;
				if (IsIntersectingWithCollider(LeftZoneCollider, gearCollider)) _canMoveLeft = false;
				if (IsIntersectingWithCollider(RightZoneCollider, gearCollider)) _canMoveRight = false;
			}
		}
	}

	// Новый метод для проверки пересечения MeshCollider
	bool IsIntersectingWithCollider(MeshCollider firstCollider, MeshCollider secondCollider)
	{
		// Проверяем столкновение между MeshCollider'ами
		Vector3 direction;
		float distance;
		return Physics.ComputePenetration(firstCollider, firstCollider.transform.position, firstCollider.transform.rotation,
										  secondCollider, secondCollider.transform.position, secondCollider.transform.rotation,
										  out direction, out distance);
	}

	// Плавное пошаговое вращение шестерни
	IEnumerator RotateGear(float targetAngle)
	{
		isMovingOrRotating = true;
		Quaternion startRotation = currentGearInstance.transform.rotation;
		Quaternion endRotation = startRotation * Quaternion.Euler(new Vector3(targetAngle, 0, 0)); // Вращение по оси X

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			currentGearInstance.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * rotationSpeed; // Используем unscaledDeltaTime
			yield return null;
		}

		currentGearInstance.transform.rotation = endRotation;
		CheckForIntersection();
		isMovingOrRotating = false;
	}

	// Перемещение объекта вправо
	IEnumerator MoveRight()
	{
		isMovingOrRotating = true;
		Vector3 startPosition = currentGearInstance.transform.position;
		Vector3 endPosition = startPosition + currentGearInstance.transform.right * movementStep;

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			currentGearInstance.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * moveSpeed; // Используем unscaledDeltaTime
			yield return null;
		}

		currentGearInstance.transform.position = endPosition;
		CheckForIntersection();
		isMovingOrRotating = false;
	}

	// Перемещение объекта влево
	IEnumerator MoveLeft()
	{
		isMovingOrRotating = true;
		Vector3 startPosition = currentGearInstance.transform.position;
		Vector3 endPosition = startPosition - currentGearInstance.transform.right * movementStep;

		float elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			currentGearInstance.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
			elapsedTime += Time.unscaledDeltaTime * moveSpeed; // Используем unscaledDeltaTime
			yield return null;
		}

		currentGearInstance.transform.position = endPosition;
		CheckForIntersection();
		isMovingOrRotating = false;

	}

	// Получение позиции спауна шестерни перед камерой
	private Vector3 GetPuzzleSpawnPosition()
	{
		var camPos = Camera.main.transform.position;
		return camPos + Camera.main.transform.forward * 1f; // Спауним впереди камеры
	}

	// Получение позиции спауна куба перед камерой
	private Vector3 GetCubeSpawnPosition()
	{
		var camPos = Camera.main.transform.position;
		return camPos + Camera.main.transform.forward * 0.7f; // Немного ближе к камере
	}
}


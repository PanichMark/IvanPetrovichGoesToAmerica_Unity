using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LockController : MonoBehaviour, IInteractable
{
	[SerializeField]
	private GameObject gearPrefab;

	// Количество сегментов вращения (задается в инспекторе)
	[SerializeField]
	private int segmentsCount = 12;

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
	private Collider UpZoneCollider;     // Верхняя зона
	private Collider DownZoneCollider;   // Нижняя зона
	private Collider LeftZoneCollider;   // Левая зона
	private Collider RightZoneCollider;  // Правая зона

	// Доступность направлений движения
	private bool _canMoveUp = true;
	private bool _canMoveDown = true;
	private bool _canMoveLeft = true;
	private bool _canMoveRight = true;

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
		UpZoneCollider = root.Find("UpZone")?.GetComponent<BoxCollider>();
		DownZoneCollider = root.Find("DownZone")?.GetComponent<BoxCollider>();
		LeftZoneCollider = root.Find("LeftZone")?.GetComponent<BoxCollider>();
		RightZoneCollider = root.Find("RightZone")?.GetComponent<BoxCollider>();

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

		// Регулярная проверка коллизий
		//CheckForIntersection();
	}

	private void FixedUpdate()
	{
		if (Time.timeScale == 0f)
		{
			Physics.Simulate(Time.fixedDeltaTime); // Принудительная симуляция физики
		}

		
	}

	private void LateUpdate()
	{
		CheckForIntersection();
	}

	private void CheckForIntersection()
	{
		if (currentCubeFollow != null && currentGearInstance != null)
		{
			// Получаем все дочерние коллайдеры шестерёнки
			Collider[] childColliders = currentGearInstance.GetComponentsInChildren<Collider>();

			// Начальные условия — считаем, что пока все направления открыты
			_canMoveUp = true;
			_canMoveDown = true;
			_canMoveLeft = true;
			_canMoveRight = true;

			// Проходим по каждому дочернему коллайдеру шестерёнки
			foreach (var gearCollider in childColliders)
			{
				// Меняем состояние коллайдера, чтобы обновить bounding-box
				gearCollider.enabled = false;
				gearCollider.enabled = true;

				// Обновляем физическое состояние
				Physics.SyncTransforms();

				// Проверяем зоны на пересечения
				if (UpZoneCollider.bounds.Intersects(gearCollider.bounds))
				{
					_canMoveUp = false;
				}

				if (DownZoneCollider.bounds.Intersects(gearCollider.bounds))
				{
					_canMoveDown = false;
				}

				if (LeftZoneCollider.bounds.Intersects(gearCollider.bounds))
				{
					_canMoveLeft = false;
				}

				if (RightZoneCollider.bounds.Intersects(gearCollider.bounds))
				{
					_canMoveRight = false;
				}
			}
		}
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
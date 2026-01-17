using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Подключаем пространство имен UI

public class LockController : MonoBehaviour, IInteractable
{
	
	[SerializeField] private GameObject gearPrefab;               // Префаб шестерёнки
	[SerializeField] private int segmentsCount;                  // Количество сегментов вращения
	[SerializeField] private float rotationSpeed;                // Скорость вращения
	[SerializeField] private float moveSpeed;                    // Скорость перемещения
	[SerializeField] private GameObject CubeFollow;              // Префаб следящего куба
	[SerializeField] private Button ClosePuzzleButton;           // Кнопка закрытия пазла
	private MenuManager menuManager;                             // Менеджер меню


	private string interactionObjectNameUI;                      // Название объекта интерфейса
	private bool isMovingOrRotating = false;                     // Блокировка взаимодействия
	private GameObject currentGearInstance;                      // Текущий экземпляр шестерёнки
	private GameObject currentCubeFollow;                        // Текущий экземпляр куба
	private MeshCollider EndCollider;                            // Коллайдер объекта "END"
	private float rotationStep;                                  // Шаг угла вращения
	private float movementStep;                                  // Шаг перемещения
	private MeshCollider CentreZoneCollider;                     // Центровый коллайдер
	private MeshCollider UpZoneCollider;                         // Верхняя зона
	private MeshCollider DownZoneCollider;                       // Нижняя зона
	private MeshCollider LeftZoneCollider;                       // Левая зона
	private MeshCollider RightZoneCollider;                      // Правая зона
	private bool _canMoveUp = true;                              // Возможность двигаться вверх
	private bool _canMoveDown = true;                            // Возможность двигаться вниз
	private bool _canMoveLeft = true;                            // Возможность двигаться влево
	private bool _canMoveRight = true;                           // Возможность двигаться вправо
	private List<MeshCollider> cachedWallColliders;              // Кэшированные коллайдеры стен
	
	public string InteractionObjectNameSystem => throw new NotImplementedException();
	public string MainInteractionHint => $"Вскрыть {interactionObjectNameUI}";
	public string AdditionalInteractionHint => throw new NotImplementedException();
	public bool IsAdditionalInteractionHintActive => false;
	public string InteractionObjectNameUI => interactionObjectNameUI;

	/*
	private void OnDrawGizmos()
	{
		if (currentGearInstance != null && Application.isPlaying)
		{
			// Поиск группы "PuzzleWalls" внутри currentGearInstance
			Transform wallsGroup = currentGearInstance.transform.Find("Walls");

			if (wallsGroup != null)
			{
				// Получаем все коллайдеры дочерних объектов внутри группы "PuzzleWalls"
				MeshCollider[] childColliders = wallsGroup.GetComponentsInChildren<MeshCollider>();

				// Отображаем коллайдеры внутри группы "PuzzleWalls" зеленым цветом
				Gizmos.color = Color.green;

				foreach (MeshCollider col in childColliders)
				{
					// Рисуем границу сетки (wireframe) коллайдера
					Gizmos.DrawWireMesh(col.sharedMesh, col.transform.position, col.transform.rotation, col.transform.lossyScale);
				}
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


	private void Awake()
	{
		menuManager = ServiceLocator.Resolve<MenuManager>();
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
	
	public void Interact()
	{
		menuManager.OpenInteractionMenu();

		// Создание экземпляров объектов
		currentGearInstance = Instantiate(gearPrefab, GetPuzzleSpawnPosition(), Quaternion.identity);
		currentGearInstance.transform.LookAt(Camera.main.transform);
		currentGearInstance.transform.Translate(-0.05f, 0f, 0f, Space.Self);

		// Определение коллайдера объекта "END"
		Transform endTransform = currentGearInstance.transform.Find("END");
		if (endTransform != null)
		{
			EndCollider = endTransform.GetComponent<MeshCollider>();
		}
		else
		{
			Debug.LogError("Объект с именем \"END\" не найден.");
		}

		// Настройка кнопок
		
			ClosePuzzleButton.onClick.RemoveAllListeners();      // Удаляем предыдущие события
			ClosePuzzleButton.onClick.AddListener(OnClosePuzzle);// Присваиваем обработчик
			ClosePuzzleButton.gameObject.SetActive(true);       // Активируем кнопку
		

		gameObject.tag = "Untagged"; // Меняем тег объекта

		// Создание куба
		currentCubeFollow = Instantiate(CubeFollow, GetCubeSpawnPosition(), Quaternion.identity);
		currentCubeFollow.transform.LookAt(Camera.main.transform);

		// Поиск детекторов зон коллизий
		Transform root = currentCubeFollow.transform;
		CentreZoneCollider = root.Find("CentreZone")?.GetComponent<MeshCollider>();
		UpZoneCollider = root.Find("UpZone")?.GetComponent<MeshCollider>();
		DownZoneCollider = root.Find("DownZone")?.GetComponent<MeshCollider>();
		LeftZoneCollider = root.Find("LeftZone")?.GetComponent<MeshCollider>();
		RightZoneCollider = root.Find("RightZone")?.GetComponent<MeshCollider>();

		// Поиск группы "Walls" и сохранение коллайдеров
		Transform wallsGroup = currentGearInstance.transform.Find("Walls");
		if (wallsGroup != null)
		{
			cachedWallColliders = new List<MeshCollider>(wallsGroup.GetComponentsInChildren<MeshCollider>());
		}
		else
		{
			Debug.LogError("Группа 'Walls' не найдена.");
		}

		CheckForIntersection();

		// Проверка детекторов зон
		if (UpZoneCollider == null || DownZoneCollider == null ||
			LeftZoneCollider == null || RightZoneCollider == null)
		{
			Debug.LogWarning("Не удалось присвоить детекторы зон!");
		}

		// Расчёт шагов вращения и перемещения
		rotationStep = 360f / segmentsCount;
		movementStep = 0.1f; // Расстояние шага в метрах
	}

	private void OnClosePuzzle()
	{
		menuManager.CloseInteractionMenu();
		Destroy(currentGearInstance);
		Destroy(currentCubeFollow);
		gameObject.tag = "Interactable";
		ClosePuzzleButton?.gameObject.SetActive(false); // Скрываем кнопку
	}

	private void CheckForIntersection()
	{
		Physics.SyncTransforms();

		if (currentCubeFollow != null && currentGearInstance != null)
		{
			_canMoveUp = true;
			_canMoveDown = true;
			_canMoveLeft = true;
			_canMoveRight = true;

			// Проверка пересечения с ранее сохранённым списком коллайдеров
			if (cachedWallColliders != null)
			{
				foreach (var collider in cachedWallColliders)
				{
					if (IsIntersectingWithCollider(UpZoneCollider, collider)) _canMoveUp = false;
					if (IsIntersectingWithCollider(DownZoneCollider, collider)) _canMoveDown = false;
					if (IsIntersectingWithCollider(LeftZoneCollider, collider)) _canMoveLeft = false;
					if (IsIntersectingWithCollider(RightZoneCollider, collider)) _canMoveRight = false;
				}
			}
			else
			{
				Debug.LogError("Список коллайдеров не заполнен.");
			}

			// Дополнительная проверка на пересечение с объектом "END"
			if (EndCollider != null && IsIntersectingWithCollider(CentreZoneCollider, EndCollider))
			{
				Debug.Log("Центр пересекся с объектом 'END'.");
				OnClosePuzzle(); // Автоматически закрываем пазл
			}
		}
	}

	private bool IsIntersectingWithCollider(MeshCollider firstCollider, MeshCollider secondCollider)
	{
		Vector3 direction;
		float distance;
		return Physics.ComputePenetration(firstCollider, firstCollider.transform.position, firstCollider.transform.rotation,
										  secondCollider, secondCollider.transform.position, secondCollider.transform.rotation,
										  out direction, out distance);
	}

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

	private Vector3 GetPuzzleSpawnPosition()
	{
		var camPos = Camera.main.transform.position;
		return camPos + Camera.main.transform.forward * 1f; // Спауним впереди камеры
	}

	private Vector3 GetCubeSpawnPosition()
	{
		var camPos = Camera.main.transform.position;
		return camPos + Camera.main.transform.forward * 0.7f; // Немного ближе к камере
	}
}

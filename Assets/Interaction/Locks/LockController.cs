using System.Collections; // Необходимо для использования IEnumerator
using UnityEngine;

public class LockController : MonoBehaviour, IInteractable
{
	[SerializeField]
	private GameObject gearPrefab;

	// Количество сегментов вращения (задаётся в инспекторе)
	[SerializeField]
	private int segmentsCount = 12;

	// Скорость вращения (радианы в секунду)
	[SerializeField]
	private float rotationSpeed;

	// Скорость перемещения (метры в секунду)
	[SerializeField]
	private float moveSpeed;

	[SerializeField] GameObject CubeFollow;

	public string InteractionObjectNameSystem => throw new System.NotImplementedException();

	public string MainInteractionHint => $"Вскрыть {InteractionObjectNameUI}";

	public string AdditionalInteractionHint => throw new System.NotImplementedException();

	public bool IsAdditionalInteractionHintActive => false;

	public string InteractionObjectNameUI => interactionObjectNameUI;

	[SerializeField] private string interactionObjectNameUI;

	[SerializeField] private MenuManager menuManager;

	// Флаг блокировки взаимодействия
	private bool isMovingOrRotating = false;

	// Экземпляр шестерёнки
	private GameObject currentGearInstance;

	private GameObject currentCubeFollow;

	// Угловой шаг поворота
	private float rotationStep;

	// Шаг перемещения
	private float movementStep;

	public void Interact()
	{
		menuManager.OpenInteractionMenu(gameObject); // Передача самого объекта сюда!

		// Создаем экземпляр шестерёнки
		currentGearInstance = Instantiate(gearPrefab, GetPuzzleSpawnPosition(), Quaternion.identity);
		currentGearInstance.transform.LookAt(Camera.main.transform); // Поворачиваем к камере
		currentGearInstance.transform.Translate(-0.05f, 0f, 0f, Space.Self);

		gameObject.tag = "Untagged";


		currentCubeFollow = Instantiate(CubeFollow, GetCubeSpawnPosition(), Quaternion.identity);
		currentCubeFollow.transform.LookAt(Camera.main.transform);


		// Рассчитываем шаги вращения и перемещения
		rotationStep = 360f / segmentsCount;
		movementStep = 0.1f; // Например, расстояние шага в метрах
	}

	private void Update()
	{
		if (!isMovingOrRotating && currentGearInstance != null)
		{
			// Проверяем ввод клавиатуры
			if (Input.GetKeyDown(KeyCode.UpArrow)) StartCoroutine(RotateGear(rotationStep));
			else if (Input.GetKeyDown(KeyCode.DownArrow)) StartCoroutine(RotateGear(-rotationStep));
			else if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(MoveRight());
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(MoveLeft());

			
		}
		CheckForIntersection();
	}

	private void FixedUpdate()
	{
		if (Time.timeScale == 0f)
		{
			Physics.Simulate(Time.fixedDeltaTime); // Принудительная симуляция физики
		}
	}

	private void CheckForIntersection()
	{
		if (currentCubeFollow != null && currentGearInstance != null &&
			currentCubeFollow.TryGetComponent(out Collider cubeCollider) &&
			currentGearInstance.TryGetComponent(out Collider gearCollider))
		{
			// Сначала переинициализируем состояние коллайдеров,
			// чтобы убедиться, что ограничивающая область обновлена
			cubeCollider.enabled = false;
			cubeCollider.enabled = true;

			gearCollider.enabled = false;
			gearCollider.enabled = true;

			// Синхронизируем преобразования физически активных объектов
			Physics.SyncTransforms();

			// Проверяем пересечение границ коллайдеров
			if (cubeCollider.bounds.Intersects(gearCollider.bounds))
			{
				Debug.Log("Куб частично входит в шестерню!");
			}
			else
			{
				Debug.Log("Объекты не пересекаются.");
			}
		}
	}


	// Метод для плавного пошагового вращения шестерни
	IEnumerator RotateGear(float targetAngle)
	{
		isMovingOrRotating = true;
		Quaternion startRotation = currentGearInstance.transform.rotation;
		Quaternion endRotation = startRotation * Quaternion.Euler(new Vector3(targetAngle, 0, 0)); // Для оси X

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
		return camPos + Camera.main.transform.forward * 1f; // Создаем экземпляр на расстоянии 0.18 м перед камерой
	}


	// Получение позиции спауна шестерни перед камерой
	private Vector3 GetCubeSpawnPosition()
	{
		var camPos = Camera.main.transform.position;
		return camPos + Camera.main.transform.forward * 0.72f; // Создаем экземпляр на расстоянии 0.18 м перед камерой
	}
}
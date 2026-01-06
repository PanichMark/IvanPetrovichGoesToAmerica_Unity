using UnityEngine;
using System.Collections; // Необходимо для использования IEnumerator

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

	// Угловой шаг поворота
	private float rotationStep;

	// Шаг перемещения
	private float movementStep;

	public void Interact()
	{
		menuManager.OpenInteractionMenu();

		// Создаем экземпляр шестерёнки
		currentGearInstance = Instantiate(gearPrefab, GetSpawnPosition(), Quaternion.identity);
		currentGearInstance.transform.LookAt(Camera.main.transform); // Поворачиваем к камере
		// После строки с созданием экземпляра
		currentGearInstance.transform.Translate(-0.01f, 0f, 0f, Space.Self);


		gameObject.tag = "Untagged";

		// Рассчитываем шаги вращения и перемещения
		rotationStep = 360f / segmentsCount;
		movementStep = 0.02f; // Например, расстояние шага в метрах
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
			elapsedTime += Time.deltaTime * rotationSpeed;
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
			elapsedTime += Time.deltaTime * moveSpeed;
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
			elapsedTime += Time.deltaTime * moveSpeed;
			yield return null;
		}

		currentGearInstance.transform.position = endPosition;
		isMovingOrRotating = false;
	}

	// Получение позиции спауна шестерни перед камерой
	private Vector3 GetSpawnPosition()
	{
		var camPos = Camera.main.transform.position;
		return camPos + Camera.main.transform.forward * 0.18f; // Создаем экземпляр на расстоянии 0.18 м перед камерой
	}
}
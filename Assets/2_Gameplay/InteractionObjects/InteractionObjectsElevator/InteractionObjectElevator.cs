using UnityEngine;
using System.Collections;

public class InteractionObjectElevator : MonoBehaviour, IInteractable
{
	public string InteractionObjectNameSystem => "Elevator";
	public string InteractionObjectNameUI => "Лифт";
	public string InteractionHintMessageMain => "Нажмите, чтобы вызвать лифт";
	public string InteractionHintAction => "Вызвать";
	public string InteractionHintMessageAdditional => "";
	public bool IsInteractionHintMessageAdditionalActive => false;

	[SerializeField] private GameObject elevator;
	[SerializeField] private float elevatorSpeed = 2f;
	[SerializeField] private float upperYPosition = 10f;

	// Поле для выбора направления. Выставляется в инспекторе для каждой кнопки.
	[SerializeField] private bool isButtonUp;

	private Vector3 targetPosition;
	private bool isMoving = false;

	// Переменная для хранения нижней точки (начальной позиции)
	private float lowerYPosition;

	void Start()
	{
		// При старте сцены запоминаем текущую Y-позицию лифта как нижнюю точку
		lowerYPosition = elevator.transform.position.y;
	}

	public void Interact()
	{
		if (isMoving) return;

		targetPosition = elevator.transform.position;

		if (isButtonUp)
		{
			targetPosition.y = upperYPosition;
		}
		else
		{
			targetPosition.y = lowerYPosition;
		}

		StartCoroutine(MoveElevator());
	}

	private IEnumerator MoveElevator()
	{
		isMoving = true;
		while (Vector3.Distance(elevator.transform.position, targetPosition) > 0.01f)
		{
			elevator.transform.position = Vector3.MoveTowards(
				elevator.transform.position,
				targetPosition,
				elevatorSpeed * Time.deltaTime
			);
			yield return null;
		}
		elevator.transform.position = targetPosition;
		isMoving = false;
	}
}
using UnityEngine;
using System.Collections;

public class InteractionObjectElevatorController : MonoBehaviour
{
	[SerializeField] private float elevatorSpeed = 2f;
	[SerializeField] private float upperYPosition = 10f;

	private float lowerYPosition;
	private bool isMoving = false;

	void Start()
	{
		// Запоминаем начальную позицию лифта как нижнюю точку
		lowerYPosition = transform.position.y;
	}

	// Публичный метод для вызова извне (кнопками)
	public void RequestMove(bool moveUp)
	{
		if (isMoving) return;
		StartCoroutine(MoveElevator(moveUp));
	}

	private IEnumerator MoveElevator(bool moveUp)
	{
		isMoving = true;

		float targetY = moveUp ? upperYPosition : lowerYPosition;
		Vector3 targetPosition = transform.position;
		targetPosition.y = targetY;

		while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
		{
			transform.position = Vector3.MoveTowards(
				transform.position,
				targetPosition,
				elevatorSpeed * Time.deltaTime
			);
			yield return null;
		}

		transform.position = targetPosition;
		isMoving = false;
	}
}
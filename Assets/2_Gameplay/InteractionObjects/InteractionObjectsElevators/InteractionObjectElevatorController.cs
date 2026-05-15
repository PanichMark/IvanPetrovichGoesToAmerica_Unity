using UnityEngine;
using System.Collections;

public class InteractionObjectElevatorController : MonoBehaviour
{
	[SerializeField] private float _elevatorSpeed;
	[SerializeField] private float _elevatorUpPosition;

	private float _elevatorDownPosition;
	private bool _isElevatorMoving = false;

	void Start()
	{
		_elevatorDownPosition = transform.position.y;
	}

	public void MoveElevator(bool moveUp)
	{
		if (_isElevatorMoving)
			return;

		StartCoroutine(ElevatorAnimation(moveUp));
	}

	private IEnumerator ElevatorAnimation(bool moveElevatorUp)
	{
		_isElevatorMoving = true;

		float targetY;
		if (moveElevatorUp)
		{
			targetY = _elevatorUpPosition;
		}
		else
		{
			targetY = _elevatorDownPosition;
		}

		Vector3 targetPosition = transform.position;
		targetPosition.y = targetY;

		while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, _elevatorSpeed * Time.deltaTime);
			yield return null;
		}

		transform.position = targetPosition;
		_isElevatorMoving = false;
	}
}
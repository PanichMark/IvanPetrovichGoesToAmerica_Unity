using UnityEngine;
using System.Collections;

public class InteractionObjectElevatorController : MonoBehaviour
{
	[SerializeField] private float _elevatorSpeed = 2f;
	[SerializeField] private float _upperYPosition = 10f;

	private float _lowerYPosition;
	private bool _isMoving = false;

	void Start()
	{
		_lowerYPosition = transform.position.y;
	}

	public void RequestMove(bool moveUp)
	{
		if (_isMoving)
			return;

		StartCoroutine(MoveElevator(moveUp));
	}

	private IEnumerator MoveElevator(bool moveUp)
	{
		_isMoving = true;

		float targetY = moveUp ? _upperYPosition : _lowerYPosition;
		Vector3 targetPosition = transform.position;
		targetPosition.y = targetY;

		while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
		{
			transform.position = Vector3.MoveTowards(
				transform.position,
				targetPosition,
				_elevatorSpeed * Time.deltaTime
			);
			yield return null;
		}

		transform.position = targetPosition;
		_isMoving = false;
	}
}
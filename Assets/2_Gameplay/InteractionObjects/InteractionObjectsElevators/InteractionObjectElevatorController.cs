using UnityEngine;
using System.Collections;

public class InteractionObjectElevatorController : MonoBehaviour
{
	[SerializeField] private float _elevatorSpeed = 2.0f;
	[SerializeField] private float _elevatorUpPosition = 5.0f;
	[SerializeField] private float _elevatorPositionTolerance = 0.1f;

	private float _elevatorDownPosition;
	private bool _isElevatorMoving = false;

	void Start()
	{
		_elevatorDownPosition = transform.position.y;
	}

	public bool MoveElevator(bool moveUp)
	{
		if (_isElevatorMoving)
			return false;

		float targetY = moveUp ? _elevatorUpPosition : _elevatorDownPosition;

		if (Mathf.Abs(transform.position.y - targetY) < _elevatorPositionTolerance)
			return false;

		StartCoroutine(ElevatorAnimation(targetY));
		return true;
	}

	private IEnumerator ElevatorAnimation(float targetY)
	{
		_isElevatorMoving = true;
		Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);

		while (Vector3.Distance(transform.position, targetPosition) > _elevatorPositionTolerance)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, _elevatorSpeed * Time.deltaTime);
			yield return null;
		}

		transform.position = targetPosition;
		_isElevatorMoving = false;
	}
}
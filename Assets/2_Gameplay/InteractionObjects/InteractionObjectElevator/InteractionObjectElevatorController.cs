using UnityEngine;
using System.Collections;

public class InteractionObjectElevatorController : GameplayObjectSaveLoad
{
	[SerializeField] private float _elevatorSpeed = 2.0f;
	[SerializeField] private float _elevatorUpPosition = 5.0f;
	[SerializeField] private float _elevatorPositionTolerance = 0.1f;

	[SerializeField] private InteractionObjectElevatorButton _downCallButton;
	[SerializeField] private InteractionObjectElevatorButton _downSendButton;
	[SerializeField] private InteractionObjectElevatorButton _upCallButton;
	[SerializeField] private InteractionObjectElevatorButton _upSendButton;

	[SerializeField] private float _doorSlidingPosition;
	[SerializeField] private float _doorSlidingSpeed;
	[SerializeField] private GameObject _cabinDoorRight;
	[SerializeField] private GameObject _cabinDoorLeft;
	[SerializeField] private GameObject _downDoorRight;
	[SerializeField] private GameObject _downDoorLeft;
	[SerializeField] private GameObject _upDoorRight;
	[SerializeField] private GameObject _upDoorLeft;

	private float _elevatorDownPosition;
	private bool _isElevatorMoving = false;

	void Start()
	{
		_elevatorDownPosition = transform.position.y;

		_downCallButton.Initialize(this, false, false);
		_downSendButton.Initialize(this, false, true);
		_upCallButton.Initialize(this, true, true);

		_upSendButton.Initialize(this, true, false);
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
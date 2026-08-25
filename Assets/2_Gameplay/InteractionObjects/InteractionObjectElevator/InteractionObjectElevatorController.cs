using UnityEngine;
using System.Collections;

public class InteractionObjectElevatorController : GameplayObjectSaveLoad
{
	[SerializeField] private float _elevatorHeightOffset;
	[SerializeField] private float _elevatorSpeed;
	private float _elevatorPositionTolerance = 0.01f;

	[SerializeField] private InteractionObjectElevatorButton _downCallButton;
	[SerializeField] private InteractionObjectElevatorButton _downSendButton;
	[SerializeField] private InteractionObjectElevatorButton _upCallButton;
	[SerializeField] private InteractionObjectElevatorButton _upSendButton;

	[SerializeField] private float _doorOpenPosition;
	[SerializeField] private float _doorSlidingSpeed;
	[SerializeField] private GameObject _cabinDoorRight;
	[SerializeField] private GameObject _cabinDoorLeft;
	[SerializeField] private GameObject _downDoorRight;
	[SerializeField] private GameObject _downDoorLeft;
	[SerializeField] private GameObject _upDoorRight;
	[SerializeField] private GameObject _upDoorLeft;

	private float _elevatorDownPosition;
	private float _elevatorUpPosition;
	private bool _isElevatorMoving;

	private bool _areDoorsPresent;

	void Start()
	{
		_elevatorDownPosition = transform.position.y;

		_downCallButton.Initialize(this, false, false);
		_downSendButton.Initialize(this, false, true);
		_upCallButton.Initialize(this, true, true);
		_upSendButton.Initialize(this, true, false);

		_elevatorDownPosition = transform.position.y;
		_elevatorUpPosition = _elevatorDownPosition + _elevatorHeightOffset;

		_areDoorsPresent = (_cabinDoorRight != null && _cabinDoorLeft != null && _downDoorRight != null && _downDoorLeft != null && _upDoorRight != null && _upDoorLeft != null);

		ImmediatelyOpenFloorDoors(false);
	}

	private void ImmediatelyOpenFloorDoors(bool isTopFloor)
	{
		if (_areDoorsPresent)
		{
			OpenDoor(_cabinDoorRight, true);
			OpenDoor(_cabinDoorLeft, false);

			if (!isTopFloor)
			{
				OpenDoor(_downDoorRight, true);
				OpenDoor(_downDoorLeft, false);
			}
			else
			{
				OpenDoor(_upDoorRight, true);
				OpenDoor(_upDoorLeft, false);
			}
		}
	}

	private void OpenDoor(GameObject door, bool isDoorRight)
	{
		if (isDoorRight)
		{
			door.transform.localPosition = new Vector3(door.transform.localPosition.x - _doorOpenPosition, door.transform.localPosition.y, door.transform.localPosition.z);
		}
		else
		{
			door.transform.localPosition = new Vector3(door.transform.localPosition.x + _doorOpenPosition, door.transform.localPosition.y, door.transform.localPosition.z);
		}
	}

	private void CloseDoor(GameObject door, bool isDoorRight)
	{
		if (isDoorRight)
		{
			door.transform.localPosition = new Vector3(door.transform.localPosition.x + _doorOpenPosition, door.transform.localPosition.y, door.transform.localPosition.z);
		}
		else
		{
			door.transform.localPosition = new Vector3(door.transform.localPosition.x - _doorOpenPosition, door.transform.localPosition.y, door.transform.localPosition.z);
		}
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
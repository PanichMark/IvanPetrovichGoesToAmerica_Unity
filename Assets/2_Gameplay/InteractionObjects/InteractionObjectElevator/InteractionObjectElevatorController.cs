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
	private bool _isElevatorUp;
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
			ImmediatelyOpenDoor(_cabinDoorRight, true);
			ImmediatelyOpenDoor(_cabinDoorLeft, false);

			if (!isTopFloor)
			{
				ImmediatelyOpenDoor(_downDoorRight, true);
				ImmediatelyOpenDoor(_downDoorLeft, false);
			}
			else
			{
				ImmediatelyOpenDoor(_upDoorRight, true);
				ImmediatelyOpenDoor(_upDoorLeft, false);
			}
		}
	}

	private void ImmediatelyOpenDoor(GameObject door, bool isDoorRight)
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

	private void ImmediatelyCloseDoor(GameObject door, bool isDoorRight)
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

		StartCoroutine(ElevatorAnimation(targetY, moveUp));
		return true;
	}

	private IEnumerator ElevatorAnimation(float targetY, bool moveUp)
	{
		_isElevatorMoving = true;

		if (_areDoorsPresent)
		{
			yield return StartCoroutine(SlowlyCloseFloorDoors(moveUp));

			yield return new WaitForSeconds(0.33f);
		}

		Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);

		while (Vector3.Distance(transform.position, targetPosition) > _elevatorPositionTolerance)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, _elevatorSpeed * Time.deltaTime);
			yield return null;
		}

		transform.position = targetPosition;

		if (_areDoorsPresent)
		{
			yield return new WaitForSeconds(0.33f);

			yield return StartCoroutine(SlowlyOpenFloorDoors(moveUp));
		}

		if (moveUp)
		{
			_isElevatorUp = true;
		}
		else
		{
			_isElevatorUp = false;
		}

			_isElevatorMoving = false;
	}

	private IEnumerator SlowlyOpenFloorDoors(bool isDownFloor)
	{
		Coroutine cabR = StartCoroutine(SlowlyOpenDoor(_cabinDoorRight, true));
		Coroutine cabL = StartCoroutine(SlowlyOpenDoor(_cabinDoorLeft, false));

		Coroutine floorR;
		Coroutine floorL;

		if (!isDownFloor)
		{
			floorR = StartCoroutine(SlowlyOpenDoor(_downDoorRight, true));
			floorL = StartCoroutine(SlowlyOpenDoor(_downDoorLeft, false));
		}
		else
		{
			floorR = StartCoroutine(SlowlyOpenDoor(_upDoorRight, true));
			floorL = StartCoroutine(SlowlyOpenDoor(_upDoorLeft, false));
		}

		// Ждем завершения всех запущенных процессов
		yield return cabR;
		yield return cabL;
		yield return floorR;
		yield return floorL;
	}

	private IEnumerator SlowlyOpenDoor(GameObject door, bool isDoorRight)
	{
		float startX = door.transform.localPosition.x;
		float targetX;

		if (isDoorRight)
		{
			targetX = startX - _doorOpenPosition;
		}
		else
		{
			targetX = startX + _doorOpenPosition;
		}

		while (Mathf.Abs(door.transform.localPosition.x - targetX) > 0.01f)
		{
			float currentX = door.transform.localPosition.x;
			float newX = Mathf.MoveTowards(currentX, targetX, _doorSlidingSpeed * Time.deltaTime);
			door.transform.localPosition = new Vector3(newX, door.transform.localPosition.y, door.transform.localPosition.z);
			yield return null;
		}

		door.transform.localPosition = new Vector3(targetX, door.transform.localPosition.y, door.transform.localPosition.z);
	}

	private IEnumerator SlowlyCloseFloorDoors(bool isDownFloor)
	{
		if (!_areDoorsPresent) { yield break; }

		Coroutine cabR = StartCoroutine(SlowlyCloseDoor(_cabinDoorRight, true));
		Coroutine cabL = StartCoroutine(SlowlyCloseDoor(_cabinDoorLeft, false));

		Coroutine floorR;
		Coroutine floorL;

		if (isDownFloor)
		{
			floorR = StartCoroutine(SlowlyCloseDoor(_downDoorRight, true));
			floorL = StartCoroutine(SlowlyCloseDoor(_downDoorLeft, false));
		}
		else
		{
			floorR = StartCoroutine(SlowlyCloseDoor(_upDoorRight, true));
			floorL = StartCoroutine(SlowlyCloseDoor(_upDoorLeft, false));
		}

		// Ждем завершения всех запущенных процессов
		yield return cabR;
		yield return cabL;
		yield return floorR;
		yield return floorL;
	}

	private IEnumerator SlowlyCloseDoor(GameObject door, bool isDoorRight)
	{
		float startX = door.transform.localPosition.x;
		float targetX;

		if (isDoorRight)
		{
			targetX = startX + _doorOpenPosition;
		}
		else
		{
			targetX = startX - _doorOpenPosition;
		}

		while (Mathf.Abs(door.transform.localPosition.x - targetX) > 0.01f)
		{
			float currentX = door.transform.localPosition.x;
			float newX = Mathf.MoveTowards(currentX, targetX, _doorSlidingSpeed * Time.deltaTime);
			door.transform.localPosition = new Vector3(newX, door.transform.localPosition.y, door.transform.localPosition.z);
			yield return null;
		}

		door.transform.localPosition = new Vector3(targetX, door.transform.localPosition.y, door.transform.localPosition.z);
	}

	public override IEnumerator SaveData(GameData data)
	{
		yield return null;
	}

	public override IEnumerator LoadData(GameData data)
	{
		yield return null;
	}
}
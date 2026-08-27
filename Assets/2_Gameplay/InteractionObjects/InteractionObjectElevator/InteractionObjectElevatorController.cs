using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class InteractionObjectElevatorController : GameplayObjectJsonSaveLoad
{
	[SerializeField] private string _elevatorName;
	public bool IsPoweredOn {  get; private set; }

	public delegate void ElevatorPowerHandler();
	public event ElevatorPowerHandler OnElevatorPoweredOn;

	[SerializeField] private float _elevatorHeightOffset;
	[SerializeField] private float _poweredOffOffset;
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

	[SerializeField] private InteractionObjectElectricalPanel _electricalPanel;

	private float _elevatorPoweredOffPosition;
	private float _elevatorDownPosition;
	private float _elevatorUpPosition;
	private bool _isElevatorMoving;
	private bool _isElevatorUp;
	private bool _areDoorsPresent;

	void Start()
	{
		if (_electricalPanel == null)
		{
			IsPoweredOn = true;
		}
		



			_downCallButton.Initialize(this, false, false);
		_downSendButton.Initialize(this, false, true);
		_upCallButton.Initialize(this, true, true);
		_upSendButton.Initialize(this, true, false);

		_elevatorDownPosition = transform.position.y;
		_elevatorPoweredOffPosition = _elevatorDownPosition + _poweredOffOffset;
		_elevatorUpPosition = _elevatorDownPosition + _elevatorHeightOffset;

		_areDoorsPresent = (_cabinDoorRight != null && _cabinDoorLeft != null && _downDoorRight != null && _downDoorLeft != null && _upDoorRight != null && _upDoorLeft != null);

		if (_electricalPanel != null)
		{
			ImmediatelyMoveElevatorToPoweredOffPosition();
			_electricalPanel.OnWentOutOfService += PowerElevatorOn;
		}

		if (_areDoorsPresent)
		{
			ImmediatelyOpenDoor(_cabinDoorRight, true);
			ImmediatelyOpenDoor(_cabinDoorLeft, false);

			ImmediatelyOpenDoor(_downDoorRight, true);
			ImmediatelyOpenDoor(_downDoorLeft, false);
		}
	}

	private void PowerElevatorOn()
	{
		IsPoweredOn = true;
		ImmediatelyMoveElevatorToPoweredOnPosition();

		_downCallButton.OnPoweredOn();
		_downSendButton.OnPoweredOn();
		_upCallButton.OnPoweredOn();
		_upSendButton.OnPoweredOn();
	}

	private void ImmediatelyMoveElevatorToPoweredOffPosition()
	{
		transform.position = new Vector3(transform.position.x, _elevatorPoweredOffPosition, transform.position.z);
	}

	private void ImmediatelyMoveElevatorToPoweredOnPosition()
	{
		transform.position = new Vector3(transform.position.x, _elevatorDownPosition, transform.position.z);
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
		if (!IsPoweredOn)
		{
			return false;
		}

		if (_isElevatorMoving)
		{
			return false;
		}

		if (_isElevatorUp && moveUp == true)
		{
			return false;
		}

		if (!_isElevatorUp && moveUp == false)
		{
			return false;
		}

		float targetY;
		if (moveUp)
		{
			targetY = _elevatorUpPosition;
		}
		else
		{
			targetY = _elevatorDownPosition;
		}

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

	public override IEnumerator SaveJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.ElevatorsData == null)
		{
			data.ElevatorsData = new Dictionary<GameScenesGameplayDataEnum, List<ElevatorData>>();
		}

		if (!data.ElevatorsData.ContainsKey(currentScene))
		{
			data.ElevatorsData[currentScene] = new List<ElevatorData>();
		}

		var targetList = data.ElevatorsData[currentScene];
		int indexInList = targetList.FindIndex(item => item.ElevatorIndex == GameplayObjectIndex);

		var updatedItem = new ElevatorData
		{
			ElevatorIndex = GameplayObjectIndex,
			ElevatorNameSystem = _elevatorName,
			IsElevatorUp = _isElevatorUp,
			IsElevatorPoweredOn = IsPoweredOn
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public override IEnumerator LoadJsonData(JsonGameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.ElevatorsData == null || !data.ElevatorsData.TryGetValue(currentScene, out var sourceList)) yield break;

		if (sourceList.Count > 0)
		{
			ElevatorData savedState = sourceList.Find(item => item.ElevatorIndex == GameplayObjectIndex);

			if (savedState.ElevatorIndex != 0)
			{
				_isElevatorUp = savedState.IsElevatorUp;

				IsPoweredOn = savedState.IsElevatorPoweredOn;

				if (IsPoweredOn)
				{
					PowerElevatorOn();
				}

				if (_isElevatorUp == true)
				{
					gameObject.transform.position = new Vector3(transform.position.x, gameObject.transform.position.y + _elevatorUpPosition, transform.position.z);

					if (_areDoorsPresent)
					{
						ImmediatelyCloseDoor(_downDoorRight, true);
						ImmediatelyCloseDoor(_downDoorLeft, false);

						ImmediatelyOpenDoor(_upDoorRight, true);
						ImmediatelyOpenDoor(_upDoorLeft, false);
					}
				}
			}
		}

		yield return null;
	}
}
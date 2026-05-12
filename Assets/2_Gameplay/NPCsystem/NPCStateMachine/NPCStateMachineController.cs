using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachineController : MonoBehaviour
{
	public delegate void StateChangeHandler();

	[SerializeField] private NPCStateTypes _initialState = NPCStateTypes.StationaryAction;
	private float _animationDuration = 99999f;
	private float _initialRotationY;
	private GameObject _cachedPlayer;

	[SerializeField] private List<GameObject> _anchorPoints = new List<GameObject>();
	[SerializeField] private List<NPCAnchorData> _stopConfigs = new List<NPCAnchorData>();

	private NPCStateAbstract _NPCstate;
	private NPCStateTypes _NPCstateType;
	private NPCAbstract _NPCabstract;
	private NavMeshAgent _navMeshAgent;
	private int _nextIndex = 0;
	private GameObject _lastVisitedStopPoint;
	private Coroutine _currentMovementCoroutine;

	public string CurrentNPCState { get; private set; } = "StationaryAction";
	public List<NPCAnchorData> StopConfigs => _stopConfigs;
	public List<GameObject> AnchorPoints => _anchorPoints;
	public float AnimationDuration => _animationDuration;
	public Coroutine currentRotationCoroutine { get; private set; }

	public bool IsAtPosition(Vector3 position, float tolerance = 1f)
	{
		return Vector3.Distance(transform.position, position) <= tolerance;
	}

	public int FindLastVisitedStopIndex()
	{
		if (_lastVisitedStopPoint == null)
			return -1;
		return _anchorPoints.FindIndex(point => point == _lastVisitedStopPoint);
	}

	public void SetLastVisitedStopPoint(GameObject point)
	{
		_lastVisitedStopPoint = point;
	}

	public GameObject GetLastVisitedStopPoint()
	{
		return _lastVisitedStopPoint;
	}

	void Start()
	{
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_initialRotationY = transform.eulerAngles.y;
		_cachedPlayer = ServiceLocator.Resolve<GameObject>("PlayerGameObject");
		_NPCabstract = GetComponent<NPCAbstract>();

		SetNPCState(_initialState);

		if (_initialState == NPCStateTypes.Dead)
			TurnNavmeshOff();
		else
			TurnNavmeshOn();
	}

	private IEnumerator RotateTowardsPlayerCoroutine()
	{
		float rotationSpeed = 160f;
		Vector3 direction = _cachedPlayer.transform.position - transform.position;
		float desiredYAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
		Quaternion startRotation = transform.rotation;
		Quaternion endRotation = Quaternion.Euler(0, desiredYAngle, 0);

		while (true)
		{
			float angleDiff = Quaternion.Angle(transform.rotation, endRotation);
			if (angleDiff < 0.1f)
				break;
			float step = rotationSpeed * Time.unscaledDeltaTime;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, step);
			yield return null;
		}

		transform.rotation = endRotation;
	}

	public void RotateTowardsPlayer()
	{
		if (currentRotationCoroutine != null)
			StopCoroutine(currentRotationCoroutine);
		currentRotationCoroutine = StartCoroutine(RotateTowardsPlayerCoroutine());
	}

	public IEnumerator MoveBetweenAnchorPointsCoroutine()
	{
		int lastVisitIndex = 0;

		while (true)
		{
			if (_anchorPoints.Count > 0)
			{
				if (_lastVisitedStopPoint != null)
					lastVisitIndex = FindLastVisitedStopIndex();
				else
					lastVisitIndex = _nextIndex++;

				GameObject targetPoint = _anchorPoints[_nextIndex];
				_navMeshAgent.destination = targetPoint.transform.position;
				while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
					yield return null;
				_lastVisitedStopPoint = _anchorPoints[_nextIndex];
			}

			_nextIndex++;
			if (_nextIndex >= _anchorPoints.Count)
				_nextIndex = 0;
		}
	}

	public void RotateTowardsInitialRotation()
	{
		if (currentRotationCoroutine != null)
			StopCoroutine(currentRotationCoroutine);
		currentRotationCoroutine = StartCoroutine(RotateBackCoroutine(_initialRotationY));
	}

	private IEnumerator RotateBackCoroutine(float targetYAngle)
	{
		float rotationSpeed = 180f;
		Quaternion startRotation = transform.rotation;
		Quaternion endRotation = Quaternion.Euler(0, targetYAngle, 0);

		while (true)
		{
			float angleDiff = Quaternion.Angle(transform.rotation, endRotation);
			if (angleDiff < 0.1f)
				break;
			float step = rotationSpeed * Time.unscaledDeltaTime;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, step);
			yield return null;
		}

		transform.rotation = endRotation;
		currentRotationCoroutine = null;
	}

	private void Update()
	{
		_NPCstate.Update();
	}

	public IEnumerator RandomMoveCoroutine()
	{
		while (true)
		{
			float randomX = Random.Range(-1f, 1f);
			float randomZ = Random.Range(-1f, 1f);
			Vector3 direction = new Vector3(randomX, 0f, randomZ).normalized;
			float duration = Random.Range(1f, 3f);
			float elapsedTime = 0f;

			while (elapsedTime <= duration)
			{
				float speed = 2f;
				float distance = speed * Time.deltaTime;
				transform.Translate(direction * distance, Space.World);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			yield return new WaitForSeconds(Random.Range(1f, 3f));
		}
	}

	public void StartRandomMove()
	{
		_currentMovementCoroutine = StartCoroutine(RandomMoveCoroutine());
	}

	public void StopRandomMove()
	{
		if (_currentMovementCoroutine != null)
		{
			StopCoroutine(_currentMovementCoroutine);
			_currentMovementCoroutine = null;
		}
	}

	public void StartAnchorMove()
	{
		_currentMovementCoroutine = StartCoroutine(MoveBetweenAnchorPointsCoroutine());
	}

	public void StopAnchorMove()
	{
		if (_currentMovementCoroutine != null)
		{
			StopCoroutine(_currentMovementCoroutine);
			_currentMovementCoroutine = null;
		}
	}

	public void TurnNavmeshOn()
	{
		_navMeshAgent.enabled = true;
	}

	public void TurnNavmeshOff()
	{
		_navMeshAgent.enabled = false;
	}

	public void SetNPCState(NPCStateTypes stateType, float animDuration)
	{
		_animationDuration = animDuration;
		SetNPCState(stateType);
	}

	public void SetNPCState(NPCStateTypes playerMovementStateType)
	{
		NPCStateAbstract newState;

		if (playerMovementStateType == NPCStateTypes.StationaryAction)
		{
			newState = new NPCStateStationaryAction(this, _animationDuration);
			CurrentNPCState = "StationaryAction";
			_NPCabstract.gameObject.tag = "Interactable";
		}
		else if (playerMovementStateType == NPCStateTypes.Patrolling)
		{
			newState = new NPCStatePatrolling(this);
			CurrentNPCState = "Patrolling";
			_NPCabstract.gameObject.tag = "Interactable";
		}
		else if (playerMovementStateType == NPCStateTypes.Interested)
			newState = new NPCStateInterested();
		else if (playerMovementStateType == NPCStateTypes.Alarmed)
			newState = new NPCStateAlarmed();
		else if (playerMovementStateType == NPCStateTypes.Chasing)
			newState = new NPCStateChasing();
		else if (playerMovementStateType == NPCStateTypes.Attacking)
			newState = new NPCStateAttacking();
		else if (playerMovementStateType == NPCStateTypes.Reloading)
			newState = new NPCStateReloading();
		else if (playerMovementStateType == NPCStateTypes.Searching)
			newState = new NPCStateSearching();
		else if (playerMovementStateType == NPCStateTypes.Scared)
		{
			newState = new NPCStateScared();
			CurrentNPCState = "Scared";
			_NPCabstract.gameObject.tag = "Untagged";
		}
		else if (playerMovementStateType == NPCStateTypes.Fleeing)
			newState = new NPCStateFleeing();
		else if (playerMovementStateType == NPCStateTypes.BeingHooked)
		{
			newState = new NPCStateBeingHooked(this);
			CurrentNPCState = "BeingHooked";
		}
		else if (playerMovementStateType == NPCStateTypes.BeingChoked)
			newState = new NPCStateBeingChoked(this);
		else if (playerMovementStateType == NPCStateTypes.Falling)
			newState = new NPCStateFalling();
		else if (playerMovementStateType == NPCStateTypes.KnockedOff)
			newState = new NPCStateKnockedOff();
		else if (playerMovementStateType == NPCStateTypes.BlownAway)
			newState = new NPCStateBlownAway();
		else if (playerMovementStateType == NPCStateTypes.StandingUp)
			newState = new NPCStateStandingUp();
		else if (playerMovementStateType == NPCStateTypes.Dead)
		{
			if (!_NPCabstract.IsNPCdead)
				_NPCabstract.SetHealthToZero();
			_NPCabstract.ConvertToPickableObject();
			newState = new NPCStateDead(this);
			CurrentNPCState = "Dead";
		}
		else
		{
			Debug.Log("Invalid state type!");
			return;
		}

		_NPCstate = newState;
	}
}
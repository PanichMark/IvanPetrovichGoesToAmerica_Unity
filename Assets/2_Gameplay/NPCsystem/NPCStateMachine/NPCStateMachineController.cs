using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachineController : MonoBehaviour
{
	public delegate void StateChangeHandler();

	[SerializeField] private NPCStateTypes initialState = NPCStateTypes.StationaryAction;
	private float animationDuration = 99999f;
	private float initialRotationY;
	private GameObject cachedPlayer;

	[SerializeField] private List<GameObject> anchorPoints = new List<GameObject>();
	[SerializeField] private List<AnchorPointStop> stopConfigs = new List<AnchorPointStop>();

	private AbstractNPCState npcState;
	private NPCStateTypes npcStateType;
	private NPCAbstract npcAbstract;
	private NavMeshAgent navMeshAgent;
	private int nextIndex = 0;
	private GameObject lastVisitedStopPoint;
	private Coroutine currentMovementCoroutine;

	public string CurrentNPCState { get; private set; } = "StationaryAction";
	public List<AnchorPointStop> StopConfigs => stopConfigs;
	public List<GameObject> AnchorPoints => anchorPoints;
	public float AnimationDuration => animationDuration;
	public Coroutine currentRotationCoroutine { get; private set; }

	public bool IsAtPosition(Vector3 position, float tolerance = 1f)
	{
		return Vector3.Distance(transform.position, position) <= tolerance;
	}

	public int FindLastVisitedStopIndex()
	{
		if (lastVisitedStopPoint == null)
			return -1;
		return anchorPoints.FindIndex(point => point == lastVisitedStopPoint);
	}

	public void SetLastVisitedStopPoint(GameObject point)
	{
		lastVisitedStopPoint = point;
	}

	public GameObject GetLastVisitedStopPoint()
	{
		return lastVisitedStopPoint;
	}

	void Start()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
		initialRotationY = transform.eulerAngles.y;
		cachedPlayer = ServiceLocator.Resolve<GameObject>("Player");
		npcAbstract = GetComponent<NPCAbstract>();

		SetNPCState(initialState);

		if (initialState == NPCStateTypes.Dead)
			TurnNavmeshOff();
		else
			TurnNavmeshOn();
	}

	private IEnumerator RotateTowardsPlayerCoroutine()
	{
		float rotationSpeed = 160f;
		Vector3 direction = cachedPlayer.transform.position - transform.position;
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
			if (anchorPoints.Count > 0)
			{
				if (lastVisitedStopPoint != null)
					lastVisitIndex = FindLastVisitedStopIndex();
				else
					lastVisitIndex = nextIndex++;

				GameObject targetPoint = anchorPoints[nextIndex];
				navMeshAgent.destination = targetPoint.transform.position;
				while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
					yield return null;
				lastVisitedStopPoint = anchorPoints[nextIndex];
			}

			nextIndex++;
			if (nextIndex >= anchorPoints.Count)
				nextIndex = 0;
		}
	}

	public void RotateTowardsInitialRotation()
	{
		if (currentRotationCoroutine != null)
			StopCoroutine(currentRotationCoroutine);
		currentRotationCoroutine = StartCoroutine(RotateBackCoroutine(initialRotationY));
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
		npcState.Update();
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
		currentMovementCoroutine = StartCoroutine(RandomMoveCoroutine());
	}

	public void StopRandomMove()
	{
		if (currentMovementCoroutine != null)
		{
			StopCoroutine(currentMovementCoroutine);
			currentMovementCoroutine = null;
		}
	}

	public void StartAnchorMove()
	{
		currentMovementCoroutine = StartCoroutine(MoveBetweenAnchorPointsCoroutine());
	}

	public void StopAnchorMove()
	{
		if (currentMovementCoroutine != null)
		{
			StopCoroutine(currentMovementCoroutine);
			currentMovementCoroutine = null;
		}
	}

	public void TurnNavmeshOn()
	{
		navMeshAgent.enabled = true;
	}

	public void TurnNavmeshOff()
	{
		navMeshAgent.enabled = false;
	}

	public void SetNPCState(NPCStateTypes stateType, float animDuration)
	{
		animationDuration = animDuration;
		SetNPCState(stateType);
	}

	public void SetNPCState(NPCStateTypes playerMovementStateType)
	{
		AbstractNPCState newState;

		if (playerMovementStateType == NPCStateTypes.StationaryAction)
		{
			newState = new StationaryActionNPCState(this, animationDuration);
			CurrentNPCState = "StationaryAction";
			npcAbstract.gameObject.tag = "Interactable";
		}
		else if (playerMovementStateType == NPCStateTypes.Patrolling)
		{
			newState = new PatrollingNPCState(this);
			CurrentNPCState = "Patrolling";
			npcAbstract.gameObject.tag = "Interactable";
		}
		else if (playerMovementStateType == NPCStateTypes.Interested)
			newState = new InterestedNPCState();
		else if (playerMovementStateType == NPCStateTypes.Alarmed)
			newState = new AlarmedNPCState();
		else if (playerMovementStateType == NPCStateTypes.Chasing)
			newState = new ChasingNPCState();
		else if (playerMovementStateType == NPCStateTypes.Attacking)
			newState = new AttackingNPCState();
		else if (playerMovementStateType == NPCStateTypes.Reloading)
			newState = new ReloadingNPCState();
		else if (playerMovementStateType == NPCStateTypes.Searching)
			newState = new SearchingNPCState();
		else if (playerMovementStateType == NPCStateTypes.Scared)
		{
			newState = new ScaredNPCState();
			CurrentNPCState = "Scared";
			npcAbstract.gameObject.tag = "Untagged";
		}
		else if (playerMovementStateType == NPCStateTypes.Fleeing)
			newState = new FleeingNPCState();
		else if (playerMovementStateType == NPCStateTypes.BeingHooked)
		{
			newState = new BeingHookedNPCState(this);
			CurrentNPCState = "BeingHooked";
		}
		else if (playerMovementStateType == NPCStateTypes.BeingChoked)
			newState = new BeingChokedNPCState(this);
		else if (playerMovementStateType == NPCStateTypes.Falling)
			newState = new FallingNPCState();
		else if (playerMovementStateType == NPCStateTypes.KnockedOff)
			newState = new KnockedOffNPCState();
		else if (playerMovementStateType == NPCStateTypes.BlownAway)
			newState = new BlownAwayNPCState();
		else if (playerMovementStateType == NPCStateTypes.StandingUp)
			newState = new StandingUpNPCState();
		else if (playerMovementStateType == NPCStateTypes.Dead)
		{
			if (!npcAbstract.IsNPCdead)
				npcAbstract.SetHealthToZero();
			npcAbstract.ConvertToPickableObject();
			newState = new DeadNPCState(this);
			CurrentNPCState = "Dead";
		}
		else
		{
			Debug.Log("Invalid state type!");
			return;
		}

		npcState = newState;
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCmovementController : MonoBehaviour
{
	public int CurrentGoalAnchorPointIndex {  get; private set; }
	private Coroutine _currentMovementCoroutine;
	private NavMeshAgent _navMeshAgent;
	[SerializeField] private List<NPCanchorData> _anchorData = new List<NPCanchorData>();
	public List<NPCanchorData> AnchorData => _anchorData;
	public float AnimationDuration => _animationDuration;
	private float _animationDuration = 99999f;
	public Coroutine currentRotationCoroutine { get; private set; }
	private NPCstateMachineController _NPCstateMachineController;
	private float _initialRotationY;
	private GameObject _cachedPlayer;
	public float CurrentStationaryWaitTime { get; private set; }
	public void Initialize(
		NPCstateMachineController NPCstateMachineController,
		NavMeshAgent navMeshAgent)
	{
		_NPCstateMachineController = NPCstateMachineController;
		_initialRotationY = transform.eulerAngles.y;
		_cachedPlayer = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.Player);
		_navMeshAgent = navMeshAgent;
		CurrentGoalAnchorPointIndex = -1;
	}

	public void TurnNavmeshOn()
	{
		_navMeshAgent.enabled = true;
	}

	public void TurnNavmeshOff()
	{
		_navMeshAgent.enabled = false;
	}

	public void StartAnchorMove()
	{
		_currentMovementCoroutine = StartCoroutine(MoveBetweenAnchorPointsCoroutine());

		Debug.Log(CurrentGoalAnchorPointIndex);
	}

	public void StopAnchorMove()
	{
		if (_currentMovementCoroutine != null)
		{
			StopCoroutine(_currentMovementCoroutine);
			_currentMovementCoroutine = null;
		}

		Debug.Log(CurrentGoalAnchorPointIndex);
	}

	private void Update()
	{
		//Debug.Log(CurrentAnchorPointIndex);
		//Debug.Log(_anchorData.Count);
	}

	public IEnumerator MoveBetweenAnchorPointsCoroutine()
	{
		if (CurrentGoalAnchorPointIndex == -1)
		{
			CurrentGoalAnchorPointIndex = 0;
		}

		while (true)
		{

			GameObject targetPoint = _anchorData[CurrentGoalAnchorPointIndex].NPCanchorPoint;

			_navMeshAgent.destination = targetPoint.transform.position;

			
			while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
			{
				yield return null;
			}
			

			//while (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
			//{
			//	yield return null;
			//}

			
			Debug.Log(gameObject.name);
			Debug.Log(CurrentGoalAnchorPointIndex);
			float waitDuration = _anchorData[CurrentGoalAnchorPointIndex].NPCwaitDuration;
			CurrentGoalAnchorPointIndex++;
			if (waitDuration > 0)
			{
				StartCoroutine(WaitFor(waitDuration));
				_NPCstateMachineController.SetNPCState(NPCstateTypes.StationaryAction);

				yield return null;
			}
			else
			{
				if (CurrentGoalAnchorPointIndex >= _anchorData.Count)
				{
					CurrentGoalAnchorPointIndex = 0;
				}
			}

		}
	}

	private IEnumerator WaitFor(float howMuch)
	{
		yield return new WaitForSeconds(howMuch);

		_NPCstateMachineController.SetNPCState(NPCstateTypes.Patrolling);

		yield return null;
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

	public void RotateTowardsPlayer()
	{
		if (currentRotationCoroutine != null)
		{
			StopCoroutine(currentRotationCoroutine);
		}

		currentRotationCoroutine = StartCoroutine(RotateTowardsPlayerCoroutine());
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
			{
				break;
			}
			float step = rotationSpeed * Time.unscaledDeltaTime;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, endRotation, step);
			yield return null;
		}

		transform.rotation = endRotation;
	}



	public void RotateTowardsInitialRotation()
	{
		if (currentRotationCoroutine != null)
			StopCoroutine(currentRotationCoroutine);
		currentRotationCoroutine = StartCoroutine(RotateTowardsInitialRotation(_initialRotationY));
	}

	private IEnumerator RotateTowardsInitialRotation(float targetYAngle)
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
}

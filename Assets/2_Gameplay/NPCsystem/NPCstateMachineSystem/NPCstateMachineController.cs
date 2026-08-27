using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCstateMachineController : MonoBehaviour
{
	public delegate void NPCstateHandler(NPCstateTypes newState);
	public event NPCstateHandler OnNewNPCstate;

	[SerializeField] private NPCstateTypes _initialState = NPCstateTypes.StationaryAction;
	private float _animationDuration = 99999f;
	private float _initialRotationY;
	private GameObject _cachedPlayer;

	//[SerializeField] private List<GameObject> _anchorPoints = new List<GameObject>();
	[SerializeField] private List<NPCanchorData> _anchorData = new List<NPCanchorData>();

	private NPCstateAbstract _NPCstate;
	private NPCstateTypes _NPCstateType;
	private NPCabstract _NPCabstract;
	private NavMeshAgent _navMeshAgent;
	private int _nextIndex = 0;
	private GameObject _lastVisitedStopPoint;
	private Coroutine _currentMovementCoroutine;

	public NPCstateTypes CurrentNPCState { get; private set; }
	public List<NPCanchorData> AnchorData => _anchorData;
	//public List<GameObject> AnchorPoints => _anchorPoints;
	public float AnimationDuration => _animationDuration;
	public Coroutine currentRotationCoroutine { get; private set; }

	public void Initialize(
		NPCabstract NPCabstract,
		NavMeshAgent navMeshAgent)
	{
		_NPCabstract = NPCabstract;
		_initialRotationY = transform.eulerAngles.y;
	_cachedPlayer = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.GameObjectPlayer);
		_navMeshAgent = navMeshAgent;
		//Debug.Log(_initialState);
		SetNPCState(_initialState);

		/*
		if (_initialState == NPCStateTypes.Dead)
			TurnNavmeshOff();
		else
			TurnNavmeshOn();
		*/
	}

	public bool IsAtPosition(Vector3 position, float tolerance = 1f)
	{
		return Vector3.Distance(transform.position, position) <= tolerance;
	}

	public int FindLastVisitedStopIndex()
	{
		if (_lastVisitedStopPoint == null)
		{
			return -1;
		}

		for (int i = 0; i < _anchorData.Count; i++)
		{
			// Сравниваем поле AnchorPoint структуры с нашим объектом
			if (_anchorData[i].NPCanchorPoint == _lastVisitedStopPoint)
			{
				return i;
			}
		}
		return -1;
	}

	public void SetLastVisitedStopPoint(GameObject point)
	{
		_lastVisitedStopPoint = point;
	}

	public GameObject GetLastVisitedStopPoint()
	{
		return _lastVisitedStopPoint;
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

	public IEnumerator MoveBetweenAnchorPointsCoroutine()
	{
		// Инициализируем индекс, если он вышел за пределы списка
		if (_nextIndex >= _anchorData.Count)
		{
			_nextIndex = 0;
		}

		while (true)
		{
			if (_anchorData.Count > 0)
			{
				// 1. Получаем цель для движения
				// _anchorData[_nextIndex] - это структура NPCAnchorData
				// .AnchorPoint - это поле GameObject внутри этой структуры
				GameObject targetPoint = _anchorData[_nextIndex].NPCanchorPoint;

				// 2. Устанавливаем точку назначения для NavMeshAgent
				if (targetPoint != null) // Проверка на случай, если AnchorPoint не назначен
				{
					_navMeshAgent.destination = targetPoint.transform.position;

					// 3. Ждем, пока агент не достигнет точки назначения
					while (_navMeshAgent.pathPending || _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
					{
						yield return null;
					}

					// 4. Сохраняем последнюю посещенную точку
					// Сохраняем всю структуру, а не только GameObject, если она может понадобиться позже
					_lastVisitedStopPoint = targetPoint;

					// 5. Выполняем действие в точке (например, ожидание)
					float waitDuration = _anchorData[_nextIndex].NPCwaitDuration;
					yield return new WaitForSeconds(waitDuration);
				}

				// 6. Переходим к следующей точке в списке
				_nextIndex++;

				// 7. Если индекс превышает количество точек, возвращаемся к началу (0)
				if (_nextIndex >= _anchorData.Count)
				{
					_nextIndex = 0;
				}
			}
			else
			{
				// Если список пуст, просто ждем, чтобы не нагружать цикл
				yield return null;
			}
		}
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

	public void SetNPCState(NPCstateTypes stateType, float animDuration)
	{
		_animationDuration = animDuration;
		SetNPCState(stateType);
	}

	public void SetNPCState(NPCstateTypes NPCstateType)
	{
		NPCstateAbstract newState;

		if (NPCstateType == NPCstateTypes.StationaryAction)
		{
			newState = new NPCstateStationaryAction(this, _animationDuration);
			CurrentNPCState = NPCstateTypes.StationaryAction;
			_NPCabstract.gameObject.tag = "Interactable";
		}
		else if (NPCstateType == NPCstateTypes.Patrolling)
		{
			newState = new NPCstatePatrolling(this);
			CurrentNPCState = NPCstateTypes.Patrolling;
			_NPCabstract.gameObject.tag = "Interactable";
		}
		else if (NPCstateType == NPCstateTypes.Interested)
		{
			newState = new NPCstateInterested();
		}
		else if (NPCstateType == NPCstateTypes.Searching)
		{
			newState = new NPCstateSearching();
		}
		else if (NPCstateType == NPCstateTypes.Alarmed)
		{
			newState = new NPCstateAlarmed();
		}
		else if (NPCstateType == NPCstateTypes.Chasing)
		{
			newState = new NPCstateChasing();
		}
		else if (NPCstateType == NPCstateTypes.Attacking)
		{
			newState = new NPCstateAttacking();
		}
		else if (NPCstateType == NPCstateTypes.Reloading)
		{
			newState = new NPCstateReloading();
		}
		else if (NPCstateType == NPCstateTypes.Huddled)
		{
			newState = new NPCstateHuddled();
			//CurrentNPCState = "Scared";
			_NPCabstract.gameObject.tag = "Untagged";
		}
		else if (NPCstateType == NPCstateTypes.Hysteric)
		{
			newState = new NPCstateHysteric();
		}
		else if (NPCstateType == NPCstateTypes.Fleeing)
		{
			newState = new NPCstateFleeing();
		}
		else if (NPCstateType == NPCstateTypes.Strangled)
		{
			newState = new NPCstateStrangled(this);
		}
		else if (NPCstateType == NPCstateTypes.Hooked)
		{
			newState = new NPCstateHooked(this);
			CurrentNPCState = NPCstateTypes.Hooked;
		}
		else if (NPCstateType == NPCstateTypes.ElectroShocked)
		{
			newState = new NPCstateElectroShocked();
		}
		else if (NPCstateType == NPCstateTypes.KnockedOff)
		{
			newState = new NPCstateKnockedOff();
		}
		else if (NPCstateType == NPCstateTypes.BlownAway)
		{
			newState = new NPCstateBlownAway();
		}
		else if (NPCstateType == NPCstateTypes.Falling)
		{
			newState = new NPCstateFalling();
		}
		else if (NPCstateType == NPCstateTypes.StandingUp)
		{
			newState = new NPCstateStandingUp();
		}
		else if (NPCstateType == NPCstateTypes.Unconscious)
		{
			newState = new NPCstateUnconscious();
		}
		else if (NPCstateType == NPCstateTypes.Dead)
		{
			newState = new NPCstateDead(this);

			_NPCabstract.ConvertToPickableObject();

			CurrentNPCState = NPCstateTypes.Dead;
		}
		else
		{
			Debug.Log("Invalid state type!");
			return;
		}

		_NPCstate = newState;

		OnNewNPCstate?.Invoke(NPCstateType);
		//_NPCabstract.ShowNPCcurrentState(CurrentNPCState);
	}
}
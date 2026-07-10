using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCStateMachineController : MonoBehaviour
{
	public delegate void NPCstateHandler();
	public event NPCstateHandler OnNPCstateDead;

	[SerializeField] private NPCStateTypes _initialState = NPCStateTypes.StationaryAction;
	private float _animationDuration = 99999f;
	private float _initialRotationY;
	private GameObject _cachedPlayer;

	//[SerializeField] private List<GameObject> _anchorPoints = new List<GameObject>();
	[SerializeField] private List<NPCAnchorData> _anchorData = new List<NPCAnchorData>();

	private NPCStateAbstract _NPCstate;
	private NPCStateTypes _NPCstateType;
	private NPCAbstract _NPCabstract;
	private NavMeshAgent _navMeshAgent;
	private int _nextIndex = 0;
	private GameObject _lastVisitedStopPoint;
	private Coroutine _currentMovementCoroutine;

	public string CurrentNPCState { get; private set; } = "StationaryAction";
	public List<NPCAnchorData> AnchorData => _anchorData;
	//public List<GameObject> AnchorPoints => _anchorPoints;
	public float AnimationDuration => _animationDuration;
	public Coroutine currentRotationCoroutine { get; private set; }

	public void Initialize()
	{
		_navMeshAgent = GetComponent<NavMeshAgent>();
		_initialRotationY = transform.eulerAngles.y;
		_cachedPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		_NPCabstract = GetComponent<NPCAbstract>();
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
			if (_anchorData[i].AnchorPoint == _lastVisitedStopPoint)
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
				GameObject targetPoint = _anchorData[_nextIndex].AnchorPoint;

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

	public void SetNPCState(NPCStateTypes stateType, float animDuration)
	{
		_animationDuration = animDuration;
		SetNPCState(stateType);
	}

	public void SetNPCState(NPCStateTypes NPCstateType)
	{
		NPCStateAbstract newState;

		if (NPCstateType == NPCStateTypes.StationaryAction)
		{
			newState = new NPCStateStationaryAction(this, _animationDuration);
			CurrentNPCState = "StationaryAction";
			_NPCabstract.gameObject.tag = "Interactable";
		}
		else if (NPCstateType == NPCStateTypes.Patrolling)
		{
			newState = new NPCStatePatrolling(this);
			CurrentNPCState = "Patrolling";
			_NPCabstract.gameObject.tag = "Interactable";
		}
		else if (NPCstateType == NPCStateTypes.Interested)
		{
			newState = new NPCStateInterested();
		}
		else if (NPCstateType == NPCStateTypes.Searching)
		{
			newState = new NPCStateSearching();
		}
		else if (NPCstateType == NPCStateTypes.Alarmed)
		{
			newState = new NPCStateAlarmed();
		}
		else if (NPCstateType == NPCStateTypes.Chasing)
		{
			newState = new NPCStateChasing();
		}
		else if (NPCstateType == NPCStateTypes.Attacking)
		{
			newState = new NPCStateAttacking();
		}
		else if (NPCstateType == NPCStateTypes.Reloading)
		{
			newState = new NPCStateReloading();
		}
		else if (NPCstateType == NPCStateTypes.Scared)
		{
			newState = new NPCStateScared();
			CurrentNPCState = "Scared";
			_NPCabstract.gameObject.tag = "Untagged";
		}
		else if (NPCstateType == NPCStateTypes.Hysteric)
		{
			newState = new NPCStateHysteric();
		}
		else if (NPCstateType == NPCStateTypes.Fleeing)
		{
			newState = new NPCStateFleeing();
		}
		else if (NPCstateType == NPCStateTypes.BeingStrangled)
		{
			newState = new NPCStateBeingStrangled(this);
		}
		else if (NPCstateType == NPCStateTypes.BeingHooked)
		{
			newState = new NPCStateBeingHooked(this);
			CurrentNPCState = "BeingHooked";
		}
		else if (NPCstateType == NPCStateTypes.BeingElectroShocked)
		{
			newState = new NPCStateBeingElectroShocked();
		}
		else if (NPCstateType == NPCStateTypes.KnockedOff)
		{
			newState = new NPCStateKnockedOff();
		}
		else if (NPCstateType == NPCStateTypes.BlownAway)
		{
			newState = new NPCStateBlownAway();
		}
		else if (NPCstateType == NPCStateTypes.Falling)
		{
			newState = new NPCStateFalling();
		}
		else if (NPCstateType == NPCStateTypes.StandingUp)
		{
			newState = new NPCStateStandingUp();
		}
		else if (NPCstateType == NPCStateTypes.Unconscious)
		{
			newState = new NPCStateUnconscious();
		}
		else if (NPCstateType == NPCStateTypes.Dead)
		{
			newState = new NPCStateDead(this);
			
			_NPCabstract.ObjectIsFullyDamaged();
			OnNPCstateDead?.Invoke();
			//_NPCabstract.ConvertToPickableObject();
			//Debug.Log("BRUH!");
			CurrentNPCState = "Dead";
		}
		else
		{
			Debug.Log("Invalid state type!");
			return;
		}

		_NPCstate = newState;
		_NPCabstract.ShowNPCcurrentState(CurrentNPCState);
	}
}
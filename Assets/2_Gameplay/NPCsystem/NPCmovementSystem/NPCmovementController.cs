using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCmovementController : MonoBehaviour
{
	private int _nextIndex = 0;
	private GameObject _lastVisitedStopPoint;
	private Coroutine _currentMovementCoroutine;
	private NavMeshAgent _navMeshAgent;
	[SerializeField] private List<NPCanchorData> _anchorData = new List<NPCanchorData>();
	public List<NPCanchorData> AnchorData => _anchorData;
	public float AnimationDuration => _animationDuration;
	private float _animationDuration = 99999f;
	public Coroutine currentRotationCoroutine { get; private set; }

	private float _initialRotationY;
	private GameObject _cachedPlayer;

	public void Initialize(NavMeshAgent navMeshAgent)
	{
		_initialRotationY = transform.eulerAngles.y;
		_cachedPlayer = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.Player);
		_navMeshAgent = navMeshAgent;
	}

	public void TurnNavmeshOn()
	{
		_navMeshAgent.enabled = true;
	}

	public void TurnNavmeshOff()
	{
		_navMeshAgent.enabled = false;
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

	public bool IsAtPosition(Vector3 position, float tolerance = 1f)
	{
		return Vector3.Distance(transform.position, position) <= tolerance;
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

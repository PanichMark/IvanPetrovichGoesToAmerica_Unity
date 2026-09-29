using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class NPCdetectionVisualController : MonoBehaviour
{
	[Header("Field of View Settings")]
	public float viewRadius = 10f;
	[SerializeField] private float viewHeightTotal = 4f;
	[Range(0, 360)] public float viewAngle = 90f;

	public bool RaycastHitPlayerInsideViewZone { get; private set; }
	private Transform _playerEyes;
	private LayerMask _targetMask;
	private LayerMask _obstacleMask;
	public float RaycastAngleFromXAxis { get; private set; }

	private NPCdetectionManager _npcDetectionManager;
	private Transform _raycastStartPosition;

	private Coroutine _scanRoutine;
	private Transform _NPCeyesPosition;

	[Space]
	[Header("Detection Speed Settings")]
	[SerializeField] private float BaseGainPerSecond = 20f;
	[SerializeField] private float BaseLossPerSecond = 15f;

	private float _currentSpeed;
	private float _meterBuffer;
	private Transform _visibleTarget;

	private float HalfHeight => viewHeightTotal / 2f;

	public void Initialize(NPCdetectionManager detectionManager)
	{
		_playerEyes = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerEyes).transform;
		_npcDetectionManager = detectionManager;

		_targetMask = LayerMask.GetMask("Player");
		_obstacleMask = LayerMask.GetMask("Default");

		_NPCeyesPosition = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/Spine/Neck/Head");
		_raycastStartPosition = _NPCeyesPosition;

		if (Application.isPlaying)
		{
			StartCoroutine(FindTargetsWithDelay(0.1f));
		}
	}

	private IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			if (!Application.isPlaying) yield break;
			FindVisibleTargets();
		}
	}

	private void FindVisibleTargets()
	{
		_visibleTarget = null;
		bool isVisibleByCenter = false;
		bool isVisibleByEyes = false;

		Collider[] targetsInRadius = Physics.OverlapSphere(_raycastStartPosition.position, viewRadius, _targetMask);

		// Направление "переда" NPC (с учетом вашего "костыля" с минусом)
		Vector3 flatForward = new Vector3(transform.TransformDirection(Vector3.forward).x, 0, transform.TransformDirection(Vector3.forward).z).normalized;

		float viewHalfAngleRad = (viewAngle * 0.5f) * Mathf.Deg2Rad;
		float minDot = Mathf.Cos(viewHalfAngleRad);

		for (int i = 0; i < targetsInRadius.Length; i++)
		{
			Transform target = targetsInRadius[i].transform;

			float heightDifference = Mathf.Abs(target.position.y - transform.position.y);
			if (heightDifference > HalfHeight) continue;

			// Вектор к центру коллайдера (для анимации UpDown и первой проверки)
			Vector3 dirToTargetCenter = (target.position - _raycastStartPosition.position).normalized;
			Vector3 flatDirectionCenter = new Vector3(dirToTargetCenter.x, 0, dirToTargetCenter.z);

			// Вектор к глазам игрока (для второй проверки)
			Vector3 dirToTargetEyes = (_playerEyes.position - _raycastStartPosition.position).normalized;
			Vector3 flatDirectionEyes = new Vector3(dirToTargetEyes.x, 0, dirToTargetEyes.z);

			// Проверяем, находится ли цель в конусе (по центру, так как это стабильнее для угла)
			float dot = Vector3.Dot(flatForward, flatDirectionCenter);
			if (dot > minDot)
			{
				float dstToTargetCenter = Vector3.Distance(_raycastStartPosition.position, target.position);
				float dstToTargetEyes = Vector3.Distance(_raycastStartPosition.position, _playerEyes.position);

				// === ПРОВЕРКА 1: Луч в центр игрока ===
				if (!Physics.Raycast(_raycastStartPosition.position, dirToTargetCenter, dstToTargetCenter, _obstacleMask))
				{
					isVisibleByCenter = true;
				}

				// === ПРОВЕРКА 2: Луч в глаза игрока ===
				if (!Physics.Raycast(_raycastStartPosition.position, dirToTargetEyes, dstToTargetEyes, _obstacleMask))
				{
					isVisibleByEyes = true;
				}

				// Если хоть один луч прошел — игрок виден
				if (isVisibleByCenter || isVisibleByEyes)
				{
					_visibleTarget = target;
					RaycastHitPlayerInsideViewZone = true;

					// Для параметра аниматора наклона головы считаем угол до глаз
					RaycastAngleFromXAxis = Vector3.SignedAngle(
						new Vector3(dirToTargetEyes.x, 0f, dirToTargetEyes.z),
						dirToTargetEyes,
						_raycastStartPosition.up);

					break;
				}
				else
				{
					RaycastHitPlayerInsideViewZone = false;
				}
			}
		}

		// Если цикл закончился, а цели в радиусе не было
		if (_visibleTarget == null)
		{
			RaycastHitPlayerInsideViewZone = false;
		}

		UpdateDetectionFlow();
	}

	private void UpdateDetectionFlow()
	{
		if (_scanRoutine != null)
		{
			StopCoroutine(_scanRoutine);
			_scanRoutine = null;
		}

		if (_visibleTarget != null)
		{
			_currentSpeed = BaseGainPerSecond;
		}
		else
		{
			_currentSpeed = -BaseLossPerSecond;
		}

		if (Application.isPlaying)
		{
			_scanRoutine = StartCoroutine(UpdateMeterOverTime());
		}
	}

	private IEnumerator UpdateMeterOverTime()
	{
		while (true)
		{
			yield return null;

			if (_npcDetectionManager != null && Mathf.Abs(_currentSpeed) > 0f)
			{
				_meterBuffer += _currentSpeed * Time.deltaTime;
				int amountToSend = 0;

				if (_currentSpeed > 0)
				{
					if (_meterBuffer >= 1f)
					{
						amountToSend = Mathf.FloorToInt(_meterBuffer);
						_meterBuffer -= amountToSend;
						_npcDetectionManager.IncreaseMeter(amountToSend);
					}
				}
				else
				{
					if (_meterBuffer <= -1f)
					{
						amountToSend = Mathf.CeilToInt(-_meterBuffer);
						_meterBuffer += amountToSend;
						_npcDetectionManager.DecreaseMeter(amountToSend);
					}
				}
			}
			else
			{
				yield break;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (_raycastStartPosition == null) _raycastStartPosition = transform;
		if (_playerEyes == null) return;
		Color oldColor = Gizmos.color;

		DrawWireCylinder(transform.position, viewRadius, viewHeightTotal);
		DrawViewAngleLines();

		if (_visibleTarget != null)
		{
			Gizmos.color = Color.red;
			// Рисуем первый луч (в центр игрока)
			Gizmos.DrawLine(_raycastStartPosition.position, _visibleTarget.position);

			// Рисуем второй луч (в глаза игрока)
			Gizmos.DrawLine(_raycastStartPosition.position, _playerEyes.position);
		}

		Gizmos.color = oldColor;
	}

	private void DrawWireCylinder(Vector3 center, float radius, float height)
	{
		Gizmos.color = Color.white;
		Vector3 topCenter = center + Vector3.up * (height / 2f);
		Vector3 bottomCenter = center - Vector3.up * (height / 2f);
		int segments = 20;
		float angleStep = 360f / segments;
		for (int i = 0; i < segments; i++)
		{
			float angleRad1 = i * angleStep * Mathf.Deg2Rad;
			float angleRad2 = ((i + 1) % segments) * angleStep * Mathf.Deg2Rad;
			Vector3 p1Top = topCenter + new Vector3(Mathf.Sin(angleRad1) * radius, 0, Mathf.Cos(angleRad1) * radius);
			Vector3 p2Top = topCenter + new Vector3(Mathf.Sin(angleRad2) * radius, 0, Mathf.Cos(angleRad2) * radius);
			Vector3 p1Bottom = bottomCenter + new Vector3(Mathf.Sin(angleRad1) * radius, 0, Mathf.Cos(angleRad1) * radius);
			Vector3 p2Bottom = bottomCenter + new Vector3(Mathf.Sin(angleRad2) * radius, 0, Mathf.Cos(angleRad2) * radius);
			Gizmos.DrawLine(p1Top, p2Top);
			Gizmos.DrawLine(p1Bottom, p2Bottom);
			Gizmos.DrawLine(p1Top, p1Bottom);
			Gizmos.DrawLine(p2Top, p2Bottom);
		}
	}

	private void DrawViewAngleLines()
	{
		Gizmos.color = Color.yellow;
		Vector3 forwardFlat = -new Vector3(transform.TransformDirection(Vector3.forward).x, 0, transform.TransformDirection(Vector3.forward).z).normalized;
		Quaternion rotation = Quaternion.LookRotation(forwardFlat);

		Vector3 viewAngleA = rotation * DirFromAngle(-viewAngle / 2, false) * viewRadius;
		Vector3 viewAngleB = rotation * DirFromAngle(viewAngle / 2, false) * viewRadius;

		Vector3 startPointTop = transform.position + Vector3.up * HalfHeight;
		Vector3 startPointBottom = transform.position - Vector3.up * HalfHeight;

		Gizmos.DrawLine(startPointTop, startPointTop + viewAngleA);
		Gizmos.DrawLine(startPointTop, startPointTop + viewAngleB);
		Gizmos.DrawLine(startPointBottom, startPointBottom + viewAngleA);
		Gizmos.DrawLine(startPointBottom, startPointBottom + viewAngleB);
		Gizmos.DrawLine(startPointTop + viewAngleA, startPointBottom + viewAngleA);
		Gizmos.DrawLine(startPointTop + viewAngleB, startPointBottom + viewAngleB);
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal) angleInDegrees += transform.eulerAngles.y;
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
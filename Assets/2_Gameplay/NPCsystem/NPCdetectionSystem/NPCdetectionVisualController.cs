using UnityEngine;
using System.Collections;

[ExecuteAlways] // Позволяет Gizmos работать в редакторе без запуска игры
public class NPCdetectionVisualController : MonoBehaviour
{
	// --- Настройки видимости (Цилиндр + Конус) ---
	[Header("Field of View Settings")]
	public float viewRadius = 10f;

	[Tooltip("Общая высота цилиндра обнаружения. Центр всегда в Pivot объекта.")]
	[SerializeField] private float viewHeightTotal = 4f;

	[Range(0, 360)]
	public float viewAngle = 90f;

	public bool RaycastHitPlayerInsideViewZone { get; private set; }

	private LayerMask _targetMask;
	private LayerMask _obstacleMask;
	public float RaycastAngleFromXAxis {  get; private set; }
	// --- Ссылки на другие системы ---
	private NPCdetectionManager _npcDetectionManager;
	private Transform _raycastStartPosition;

	// --- Корутины управления счетом ---
	private Coroutine _scanRoutine;
	private Transform _NPCeyesPosition;
	// --- Скорости изменения meter ---
	[Space]
	[Header("Detection Speed Settings")]
	[SerializeField] private float BaseGainPerSecond = 20f;
	[SerializeField] private float BaseLossPerSecond = 15f;

	private float _currentSpeed;
	private float _meterBuffer;

	// --- Кэш цели ---
	private Transform _visibleTarget;

	private float HalfHeight => viewHeightTotal / 2f;

	public void Initialize(NPCdetectionManager detectionManager)
	{
		_npcDetectionManager = detectionManager;

		_targetMask = LayerMask.GetMask("Player");
		//_obstacleMask = LayerMask.GetMask("Default", "Outline", "HitboxBody_Organism", "HitboxBody_Robot", "HitboxHead_Organism", "HitboxHead_Robot");
		_obstacleMask = LayerMask.GetMask("Default");

		_NPCeyesPosition = transform.Find("NPC_3Dmodel/HitboxArmature/Armature_Humanoid/Root/Spine/Neck/Head");

		_raycastStartPosition = _NPCeyesPosition;

		// Логика поиска запускается ТОЛЬКО во время игры
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

			if (!Application.isPlaying)
				yield break;

			FindVisibleTargets();
		}
	}

	private void FindVisibleTargets()
	{
		bool wasSeeingPlayer = (_visibleTarget != null);
		_visibleTarget = null;

		Collider[] targetsInRadius = Physics.OverlapSphere(_raycastStartPosition.position, viewRadius, _targetMask);

		for (int i = 0; i < targetsInRadius.Length; i++)
		{
			Transform target = targetsInRadius[i].transform;

			// ПРОВЕРКА ПО ВЫСОТЕ (CYLINDER HEIGHT)
			float heightDifference = Mathf.Abs(target.position.y - _raycastStartPosition.position.y);
			if (heightDifference > HalfHeight)
			{
				continue;
			}

			Vector3 dirToTarget = (target.position - _raycastStartPosition.position).normalized;

			RaycastAngleFromXAxis = Vector3.SignedAngle(
				new Vector3(dirToTarget.x, 0f, dirToTarget.z), // Горизонтальное направление к цели
				dirToTarget,                                   // Реальное направление к цели (вверх или вниз)
				_raycastStartPosition.right                             // Ось наклона: вбок
);

			Vector3 flatDirection = new Vector3(dirToTarget.x, 0, dirToTarget.z);
			Vector3 flatForward = new Vector3(_raycastStartPosition.forward.x, 0, _raycastStartPosition.forward.z);

			if (Vector3.Angle(flatForward, flatDirection) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(_raycastStartPosition.position, target.position);

				if (!Physics.Raycast(_raycastStartPosition.position, dirToTarget, dstToTarget, _obstacleMask))
				{
					_visibleTarget = target;
					RaycastHitPlayerInsideViewZone = true;

					//Debug.Log(RaycastAngleFromXAxis);
				}
				else
				{
					RaycastHitPlayerInsideViewZone = false;
				}

				break;
			}
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

	// --- GIZMOS & DEBUG (ОТРИСОВКА БЕЗ UNITYEDITOR DEFINE) ---

	private void OnDrawGizmos()
	{
		// Обновляем ссылку на трансформ для работы в Edit Mode
		if (_raycastStartPosition == null) _raycastStartPosition = transform;

		Color oldColor = Gizmos.color;

		// Рисуем ЦИЛИНДР белыми линиями
		//DrawWireCylinder(_myTransform.position, viewRadius, viewHeightTotal);

		// Рисуем УГОЛ ОБЗОРА желтым
		//DrawViewAngleLines();

		// Если игрок найден — рисуем линию КРАСНЫМ (только если есть ссылка)
		if (_visibleTarget != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(_raycastStartPosition.position, _visibleTarget.position);
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

			Gizmos.DrawLine(p1Top, p2Top);      // Верхнее кольцо
			Gizmos.DrawLine(p1Bottom, p2Bottom);// Нижнее кольцо
			Gizmos.DrawLine(p1Top, p1Bottom);   // Стойка 1
			Gizmos.DrawLine(p2Top, p2Bottom);   // Стойка 2
		}
	}

	private void DrawViewAngleLines()
	{
		Gizmos.color = Color.yellow;

		Vector3 forwardFlat = new Vector3(_raycastStartPosition.forward.x, 0, _raycastStartPosition.forward.z).normalized;
		Quaternion rotation = Quaternion.LookRotation(forwardFlat);

		Vector3 viewAngleA = rotation * DirFromAngle(-viewAngle / 2, false) * viewRadius;
		Vector3 viewAngleB = rotation * DirFromAngle(viewAngle / 2, false) * viewRadius;

		Vector3 startPointTop = _raycastStartPosition.position + Vector3.up * HalfHeight;
		Vector3 startPointBottom = _raycastStartPosition.position - Vector3.up * HalfHeight;

		Gizmos.DrawLine(startPointTop, startPointTop + viewAngleA);
		Gizmos.DrawLine(startPointTop, startPointTop + viewAngleB);

		Gizmos.DrawLine(startPointBottom, startPointBottom + viewAngleA);
		Gizmos.DrawLine(startPointBottom, startPointBottom + viewAngleB);

		Gizmos.DrawLine(startPointTop + viewAngleA, startPointBottom + viewAngleA);
		Gizmos.DrawLine(startPointTop + viewAngleB, startPointBottom + viewAngleB);
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += _raycastStartPosition.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
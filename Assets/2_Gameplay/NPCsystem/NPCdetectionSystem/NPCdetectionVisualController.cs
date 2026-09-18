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

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	// --- Ссылки на другие системы ---
	private NPCdetectionManager _npcDetectionManager;
	private Transform _myTransform;

	// --- Корутины управления счетом ---
	private Coroutine _scanRoutine;

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

	/// <summary>
	/// Инициализация через DI. Заменяет OnEnable для игровой логики.
	/// </summary>
	public void Initialize(NPCdetectionManager detectionManager)
	{
		_npcDetectionManager = detectionManager;
		_myTransform = transform;

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

		Collider[] targetsInRadius = Physics.OverlapSphere(_myTransform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInRadius.Length; i++)
		{
			Transform target = targetsInRadius[i].transform;

			// ПРОВЕРКА ПО ВЫСОТЕ (CYLINDER HEIGHT)
			float heightDifference = Mathf.Abs(target.position.y - _myTransform.position.y);
			if (heightDifference > HalfHeight)
			{
				continue;
			}

			Vector3 dirToTarget = (target.position - _myTransform.position).normalized;
			Vector3 flatDirection = new Vector3(dirToTarget.x, 0, dirToTarget.z);
			Vector3 flatForward = new Vector3(_myTransform.forward.x, 0, _myTransform.forward.z);

			if (Vector3.Angle(flatForward, flatDirection) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(_myTransform.position, target.position);
				if (!Physics.Raycast(_myTransform.position, dirToTarget, dstToTarget, obstacleMask))
				{
					_visibleTarget = target;
					break;
				}
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

	private void OnDrawGizmosSelected()
	{
		// Обновляем ссылку на трансформ для работы в Edit Mode
		if (_myTransform == null) _myTransform = transform;

		Color oldColor = Gizmos.color;

		// Рисуем ЦИЛИНДР белыми линиями
		DrawWireCylinder(_myTransform.position, viewRadius, viewHeightTotal);

		// Рисуем УГОЛ ОБЗОРА желтым
		DrawViewAngleLines();

		// Если игрок найден — рисуем линию КРАСНЫМ (только если есть ссылка)
		if (_visibleTarget != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(_myTransform.position, _visibleTarget.position);
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

		Vector3 forwardFlat = new Vector3(_myTransform.forward.x, 0, _myTransform.forward.z).normalized;
		Quaternion rotation = Quaternion.LookRotation(forwardFlat);

		Vector3 viewAngleA = rotation * DirFromAngle(-viewAngle / 2, false) * viewRadius;
		Vector3 viewAngleB = rotation * DirFromAngle(viewAngle / 2, false) * viewRadius;

		Vector3 startPointTop = _myTransform.position + Vector3.up * HalfHeight;
		Vector3 startPointBottom = _myTransform.position - Vector3.up * HalfHeight;

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
			angleInDegrees += _myTransform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}
using UnityEngine;
using System.Collections;

public class NPCdetectionVisualController : MonoBehaviour
{
	// --- Настройки видимости ---
	[Header("Field of View Settings")]
	public float viewRadius = 10f;
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
	[Tooltip("Скорость роста шкалы обнаружения, когда игрок виден.")]
	[SerializeField] private float BaseGainPerSecond = 20f;

	[Tooltip("Скорость падения шкалы обнаружения, когда игрок скрыт.")]
	[SerializeField] private float BaseLossPerSecond = 15f;

	// Текущая активная скорость (+gain или -loss)
	private float _currentSpeed;

	// Буфер для накопления дробных значений перед отправкой в Manager
	private float _meterBuffer;

	// --- Кэш цели ---
	private Transform _visibleTarget;

	// Метод для инициализации через DI
	public void Initialize(NPCdetectionManager detectionManager)
	{
		_npcDetectionManager = detectionManager;
		_myTransform = transform;

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

		Collider[] targetsInViewRadius = Physics.OverlapSphere(_myTransform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - _myTransform.position).normalized;

			if (Vector3.Angle(_myTransform.forward, dirToTarget) < viewAngle / 2)
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

		_scanRoutine = StartCoroutine(UpdateMeterOverTime());
	}

	private IEnumerator UpdateMeterOverTime()
	{
		while (true)
		{
			yield return null;

			if (_npcDetectionManager != null && Mathf.Abs(_currentSpeed) > 0f)
			{
				// Накапливаем изменение за кадр в буфере
				_meterBuffer += _currentSpeed * Time.deltaTime;

				int amountToSend = 0;

				if (_currentSpeed > 0)
				{
					// Если растем: проверяем, набралось ли целое число для передачи
					if (_meterBuffer >= 1f)
					{
						amountToSend = Mathf.FloorToInt(_meterBuffer);
						_meterBuffer -= amountToSend;

						// Используем новый метод Increase
						_npcDetectionManager.IncreaseMeter(amountToSend);
					}
				}
				else
				{
					// Если падаем: используем CeilToInt для отрицательных чисел (-1.7 станет -1)
					if (_meterBuffer <= -1f)
					{
						// Берем модуль, так как Decrease принимает положительное число "сколько убрать"
						amountToSend = Mathf.CeilToInt(-_meterBuffer);
						_meterBuffer += amountToSend; // Прибавляем, так как buffer отрицательный

						// Используем новый метод Decrease
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

	// --- GIZMOS & DEBUG ---

	private void OnDrawGizmosSelected()
	{
		Transform gizmoTransform = transform;

		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(gizmoTransform.position, viewRadius);

		Vector3 viewAngleA = DirFromAngleForEditor(-viewAngle / 2, gizmoTransform);
		Vector3 viewAngleB = DirFromAngleForEditor(viewAngle / 2, gizmoTransform);

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(gizmoTransform.position, gizmoTransform.position + viewAngleA * viewRadius);
		Gizmos.DrawLine(gizmoTransform.position, gizmoTransform.position + viewAngleB * viewRadius);

		if (_visibleTarget != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(gizmoTransform.position, _visibleTarget.position);
		}
	}

	private Vector3 DirFromAngleForEditor(float angleInDegrees, Transform referenceTransform)
	{
		float currentYRotation = referenceTransform.eulerAngles.y;
		angleInDegrees += currentYRotation;

		return new Vector3(
			Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
			0,
			Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	private void OnDestroy()
	{
		if (_scanRoutine != null) StopCoroutine(_scanRoutine);
	}
}
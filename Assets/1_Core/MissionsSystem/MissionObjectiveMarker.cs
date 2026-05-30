using UnityEngine;

public class MissionObjectiveMarker : MonoBehaviour
{
	// --- Зависимости ---
	private MissionsManager _missionsManager;
	private WorldToUISpace _worldToUISpace;

	// Смещение маркера над объектом в мировом пространстве
	private readonly Vector3 _worldOffset = new Vector3(0, 2f, 0);

	// Кэшированная цель для отслеживания
	private Transform _currentTarget;

	public void Initialize(MissionsManager missionsManager, WorldToUISpace worldToUISpace)
	{
		_missionsManager = missionsManager;
		_worldToUISpace = worldToUISpace;

		if (_missionsManager != null)
		{
			// Подписываемся на событие смены шага миссии,
			// чтобы сбросить кэш цели при необходимости
			MissionsManager.OnCurrentStepChanged += HandleStepChanged;

			// Ищем цель сразу после инициализации
			FindAndSetNewTarget();
		}
	}

	/// <summary>
	/// Вызывается при смене шага миссии. Сбрасывает текущую цель.
	/// </summary>
	private void HandleStepChanged()
	{
		// При смене шага старую цель нужно забыть,
		// чтобы найти новую на следующем Update
		_currentTarget = null;
	}

	/// <summary>
	/// Основной метод, вызываемый каждый кадр.
	/// Ищет цель, если ее нет, и обновляет позицию.
	/// </summary>
	private void Update()
	{
		// Если компонент UI не готов - выходим
		if (_worldToUISpace == null) return;

		// Если цели нет, пытаемся ее найти
		if (_currentTarget == null)
		{
			FindAndSetNewTarget();

			// Если цель так и не нашлась, выходим
			if (_currentTarget == null)
			{
				return;
			}
		}

		// Если цель есть - обновляем позицию маркера
		UpdateMarkerPosition(_currentTarget.position);
	}

	/// <summary>
	/// Находит новую цель и сохраняет ее в переменную _currentTarget.
	/// </summary>
	private void FindAndSetNewTarget()
	{
		_currentTarget = FindActiveTarget();
	}

	/// <summary>
	/// Находит первый активный объект-цель из условий текущего шага.
	/// </summary>
	private Transform FindActiveTarget()
	{
		if (_missionsManager?.ActiveMission == null) return null;

		var currentStep = _missionsManager.ActiveMission.Steps[_missionsManager.CurrentStepIndex] as ICurrentMissionStep;
		if (currentStep == null || currentStep.Conditions.Count == 0) return null;

		foreach (var condition in currentStep.Conditions)
		{
			if (condition.Owner != null && !condition.IsMet())
			{
				return condition.Owner.transform;
			}
		}
		return null;
	}

	/// <summary>
	/// Вычисляет финальную позицию и обновляет UI-маркер.
	/// Содержит отладочный вывод.
	/// </summary>
	private void UpdateMarkerPosition(Vector3 targetWorldPos)
	{
		// Применяем мировое смещение к позиции цели
		Vector3 finalWorldPos = targetWorldPos + _worldOffset;

		// Передаем вычисленную позицию в компонент UI для проецирования на экран
		_worldToUISpace.UpdatePosition(finalWorldPos);

		// DEBUG LOG: Получаем позицию маркера из его компонента RectTransform
		Vector2 uiAnchoredPos = _worldToUISpace.GetComponent<RectTransform>().anchoredPosition;
		//Debug.Log($"[DEBUG][UPDATE] Цель (World): {targetWorldPos} | Маркер (Canvas AnchoredPos): {uiAnchoredPos}");
		Debug.Log($"Маркер: {uiAnchoredPos}");
	}
}
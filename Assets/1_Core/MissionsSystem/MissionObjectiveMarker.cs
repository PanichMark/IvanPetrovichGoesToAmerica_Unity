using UnityEngine;

public class MissionObjectiveMarker : MonoBehaviour
{
	// --- Приватные зависимости ---
	private MissionsManager _missionsManager;
	private Transform _playerCameraTransform; // Для позиции объекта в мире
	private RectTransform _canvasRectTransform;
	private Camera _worldCamera;            // Камера для проекции на экран
	private WorldToUISpace _uiImageLogicComponent;
	private Vector3 _worldOffset = new Vector3(0, 2f, 0); // Смещение маркера над объектом

	public void Initialize(
		MissionsManager missionsManager,
		GameObject playerCamera,
		RectTransform canvas,
		WorldToUISpace uiImageLogicComponent)
	{
		_missionsManager = missionsManager;
		_playerCameraTransform = playerCamera.transform;
		_canvasRectTransform = canvas;
		_worldCamera = playerCamera.GetComponent<Camera>();
		_uiImageLogicComponent = uiImageLogicComponent;

		// Подписываемся на глобальное событие смены шага миссии
		MissionsManager.OnCurrentStepChanged += HandleStepChanged;

		// Проверяем состояние сразу после инициализации
		HandleStepChanged();
	}

	private void OnDestroy()
	{
		// Обязательно отписываемся, чтобы избежать ошибок памяти
		if (_missionsManager != null) // Проверка на случай, если менеджер уничтожится раньше
		{
			MissionsManager.OnCurrentStepChanged -= HandleStepChanged;
		}
	}

	private void HandleStepChanged()
	{
		bool isTarget = IsThisObjectTheTarget();
		UpdateMarkerState(isTarget);
	}

	private bool IsThisObjectTheTarget()
	{
		// Если активной миссии нет или индекс шага вышел за пределы массива
		if (_missionsManager.ActiveMission == null || _missionsManager.CurrentStepIndex >= _missionsManager.ActiveMission.Steps.Length)
		{
			return false;
		}

		var currentStepBase = _missionsManager.ActiveMission.Steps[_missionsManager.CurrentStepIndex];

		// Безопасное приведение к интерфейсу
		if (currentStepBase is ICurrentMissionStep currentStep)
		{
			foreach (var condition in currentStep.Conditions)
			{
				// Проверяем, является ли этот объект владельцем условия И условие еще не выполнено
				if (condition.Owner == this.gameObject && !condition.IsMet())
				{
					return true;
				}
			}
		}
		return false;
	}

	private void UpdateMarkerState(bool shouldBeVisible)
	{
		if (shouldBeVisible && _uiImageLogicComponent != null)
		{
			ShowAndUpdateMarker();
		}
		else
		{
			//HideMarker();
		}
	}

	private void ShowAndUpdateMarker()
	{
		// Вычисляем позицию с учетом смещения
		Vector3 targetPositionForMarker = transform.position + _worldOffset;

		// Передаем данные в компонент логики UI
		_uiImageLogicComponent.Initialize(_canvasRectTransform, _worldCamera);
		_uiImageLogicComponent.UpdatePosition(targetPositionForMarker);
	}

	private void HideMarker()
	{
		if (_uiImageLogicComponent != null && _uiImageLogicComponent.gameObject.activeSelf)
		{
			_uiImageLogicComponent.gameObject.SetActive(false);
		}
	}
}
using UnityEngine;

public class MissionObjectiveMarker : MonoBehaviour
{
	private MissionsManager _missionsManager;
	private Transform _playerCameraTransform;
	private RectTransform _canvasRectTransform;
	private Camera _worldCamera;
	private WorldToUISpace _uiImageLogicComponent;
	private Vector3 _worldOffset;

	public void Initialize(
		MissionsManager missionsManager,
		Transform playerCamera,
		RectTransform canvas,
		Camera worldCamera,
		WorldToUISpace uiImageLogicComponent,
		Vector3 worldOffset)
	{
		_missionsManager = missionsManager;
		_playerCameraTransform = playerCamera;
		_canvasRectTransform = canvas;
		_worldCamera = worldCamera;
		_uiImageLogicComponent = uiImageLogicComponent;
		_worldOffset = worldOffset;

		MissionsManager.OnCurrentStepChanged += HandleStepChanged;
		HandleStepChanged();
	}

	private void OnDestroy()
	{
		MissionsManager.OnCurrentStepChanged -= HandleStepChanged;
	}

	private void HandleStepChanged()
	{
		bool isTarget = IsThisObjectCurrentStepTarget();
		UpdateMarkerState(isTarget);
	}

	private bool IsThisObjectCurrentStepTarget()
	{
		if (_missionsManager.ActiveMission == null ||
			_missionsManager.CurrentStepIndex >= _missionsManager.ActiveMission.Steps.Length)
		{
			return false;
		}

		var currentStepBase = _missionsManager.ActiveMission.Steps[_missionsManager.CurrentStepIndex];

		// Приводим базовый шаг к интерфейсу для безопасной работы
		if (currentStepBase is ICurrentMissionStep currentStep)
		{
			foreach (var condition in currentStep.Conditions)
			{
				// Проверяем владельца и выполнение условия через интерфейс
				if (condition.Owner == this.gameObject && condition.IsMet())
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
			HideMarker();
		}
	}

	private void ShowAndUpdateMarker()
	{
		Vector3 targetPositionForMarker = transform.position + _worldOffset;

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
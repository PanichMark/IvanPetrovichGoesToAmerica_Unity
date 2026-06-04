using UnityEngine;

public class MissionGoalMarkerController : MonoBehaviour
{
	private MissionsManager _missionsManager;
	private GameObject _imageMissionGoalMarker;
	private RectTransform _imageMissionGoalMarkerRectTransform;
	private Camera _playerCameraComponent;
	private GameObject _gameObjectMissionGoal;

	private float markerOffset = 20f;
	private float markerHeight;

	private ICurrentMissionStep _currentMissionStepCache;

	public void Initialize(
		MissionsManager missionsManager,
		GameObject playerCamera,
		GameObject imageMissionGoalMarker)
	{
		_missionsManager = missionsManager;
		_imageMissionGoalMarker = imageMissionGoalMarker;
		_imageMissionGoalMarkerRectTransform = _imageMissionGoalMarker.GetComponent<RectTransform>();
		_playerCameraComponent = playerCamera.GetComponent<Camera>();

		MissionsManager.OnCurrentStepChanged += HandleStepChanged;
		//HandleStepChanged();
		Invoke(nameof(RequestRecheck), 0.1f);

		markerHeight = _imageMissionGoalMarkerRectTransform.rect.height;

		Debug.Log("MissionGoalMarkerController Initialized");
	}

	private void HandleStepChanged()
	{
		UpdateCurrentStepCache();

		GameObject targetObject = FindActiveTargetObject();

		bool shouldBeVisible = targetObject != null;
	}


	private GameObject FindActiveTargetObject()
	{
		if (_currentMissionStepCache == null) return null;

		foreach (var condition in _currentMissionStepCache.Conditions)
		{
			string ownerName = condition.Owner ? condition.Owner.name : "NULL";
			bool conditionMet = condition.IsMet();

			Debug.Log($"[MissionMarker] Условие: '{condition.GetType().Name}'. Владелец: {ownerName}. Выполнено: {conditionMet}");

			if (condition.Owner != null && !conditionMet)
			{
				Debug.Log($"[MissionMarker] Найдена активная цель: {condition.Owner.name}");
				_gameObjectMissionGoal = condition.Owner;
				return condition.Owner;
			}
		}
		Debug.Log("[MissionMarker] Активных целей для отслеживания не найдено.");
		return null;
	}

	private void Update()
	{
		if (_gameObjectMissionGoal == null)
		{
			return; 
		}

		Vector3 screenPoint = _playerCameraComponent.WorldToViewportPoint(_gameObjectMissionGoal.transform.position);

		if (screenPoint.z <= 0)
		{
			return;
		}

		bool isOnScreenX = screenPoint.x >= 0 && screenPoint.x <= 1;
		bool isOnScreenY = screenPoint.y >= 0 && screenPoint.y <= 1;

		float xPos;
		if (!isOnScreenX)
		{
			if (screenPoint.x < 0)
			{
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width + 50;
			}
			else
			{
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width - 50;
			}
		}
		else
		{
			xPos = screenPoint.x * Screen.width;
		}

		float yPos;
		if (!isOnScreenY)
		{
			if (screenPoint.y < 0)
			{
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height + 50;
			}
			else
			{
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height - 50;
			}
		}
		else
		{
			yPos = screenPoint.y * Screen.height;
		}

		if (isOnScreenX)
		{
			if (screenPoint.x < 0)
			{
				xPos -= markerOffset;
			}
			else if (screenPoint.x > 1)
			{
				xPos += markerOffset;
			}
		}

		if (isOnScreenY)
		{
			if (screenPoint.y < 0)
			{
				yPos -= markerOffset;
			}
			else if (screenPoint.y > 1)
			{
				yPos += markerHeight + markerOffset;
			}
		}

		_imageMissionGoalMarkerRectTransform.anchoredPosition = new Vector2(xPos, yPos);
	}

	public void RequestRecheck()
	{
		Debug.Log("[MissionMarker] Задержанная проверка цели...");
		HandleStepChanged();
	}

	private void UpdateCurrentStepCache()
	{
		if (_missionsManager?.ActiveMission == null || _missionsManager.CurrentStepIndex < 0)
		{
			_currentMissionStepCache = null;
			return;
		}

		try
		{
			_currentMissionStepCache = _missionsManager.ActiveMission.Steps[_missionsManager.CurrentStepIndex] as ICurrentMissionStep;
		}
		catch (System.IndexOutOfRangeException)
		{
			_currentMissionStepCache = null;
		}
	}
}
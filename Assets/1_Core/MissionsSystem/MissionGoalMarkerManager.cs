using UnityEngine;

public class MissionGoalMarkerManager : MonoBehaviour
{
	private MissionsManager _missionsManager;
	private GameObject _imageMissionGoalMarker;
	private RectTransform _imageMissionGoalMarkerRectTransform;
	private Camera _playerCameraComponent;
	private GameObject _gameObjectMissionGoal;

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
		HandleStepChanged();
		Invoke(nameof(RequestRecheck), 0.1f);
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
		if (_gameObjectMissionGoal != null)
		{
			Vector3 screenPoint = _playerCameraComponent.WorldToViewportPoint(_gameObjectMissionGoal.transform.position);

			float xPos = screenPoint.x * Screen.width;
			float yPos = screenPoint.y * Screen.height;

			_imageMissionGoalMarkerRectTransform.anchoredPosition = new Vector2(xPos, yPos);

			//Debug.Log($"[UI Marker] World Pos: {_TARGETGAMEOBJECT.transform.position}");

			//Debug.Log($"[UI Marker] UI Position (px): ({xPos}, {yPos})");

			//Debug.Log($"[UI Marker] Anchored Position set to: {_MISSION_MARKER_RectTransform.anchoredPosition}");
		}
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
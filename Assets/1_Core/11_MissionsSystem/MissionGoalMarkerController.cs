using UnityEngine;

public class MissionGoalMarkerController : MonoBehaviour
{
	private MissionsManager _missionsManager;
	private GameObject _imageMissionGoalMarker;
	private RectTransform _imageMissionGoalMarkerRectTransform;
	private Camera _playerCameraComponent;
	private GameObject _gameObjectMissionGoal;
	private GameScenesManager _gameSceneManager;
	private float _markerOffset = 20f;
	private float _markerHeight;
	private Bootstrap _bootstrap;
	private IMissionStep _currentMissionStepCache;
	private float _markerBorderOffset = 45f;

	public void Initialize(
		Bootstrap bootstrap,
		GameScenesManager gameSceneManager,
		MissionsManager missionsManager,
		GameObject playerCamera,
		GameObject imageMissionGoalMarker)
	{
		_bootstrap = bootstrap;
		_missionsManager = missionsManager;
		_imageMissionGoalMarker = imageMissionGoalMarker;
		_imageMissionGoalMarkerRectTransform = _imageMissionGoalMarker.GetComponent<RectTransform>();
		_playerCameraComponent = playerCamera.GetComponent<Camera>();
		_gameSceneManager = gameSceneManager;
		_missionsManager.OnCurrentStepChanged += HandleStepChanged;
		//HandleStepChanged();
		_gameSceneManager.OnEndLoadingGameplayScene += CheckMissionStep;
		//Invoke(nameof(RequestRecheck), 0.1f);

		_markerHeight = _imageMissionGoalMarkerRectTransform.rect.height;

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

			//Debug.Log(condition.Owner);
			//Debug.Log(conditionMet);

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
		if (!_bootstrap.IsBootstrapInitialized || _gameObjectMissionGoal == null)
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
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width + _markerBorderOffset;
			}
			else
			{
				xPos = Mathf.Clamp01(screenPoint.x) * Screen.width - _markerBorderOffset;
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
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height + _markerBorderOffset;
			}
			else
			{
				yPos = Mathf.Clamp01(screenPoint.y) * Screen.height - _markerBorderOffset;
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
				xPos -= _markerOffset;
			}
			else if (screenPoint.x > 1)
			{
				xPos += _markerOffset;
			}
		}

		if (isOnScreenY)
		{
			if (screenPoint.y < 0)
			{
				yPos -= _markerOffset;
			}
			else if (screenPoint.y > 1)
			{
				yPos += _markerHeight + _markerOffset;
			}
		}

		_imageMissionGoalMarkerRectTransform.anchoredPosition = new Vector2(xPos, yPos);
	}

	public void CheckMissionStep()
	{
		Debug.Log("[MissionMarker] Checking for MissionStep Objective");
		HandleStepChanged();
	}

	private void UpdateCurrentStepCache()
	{
		if (_missionsManager.ActiveMission == null || _missionsManager.CurrentStepIndex < 0)
		{
			_currentMissionStepCache = null;
			return;
		}

		try
		{
			_currentMissionStepCache = _missionsManager.ActiveMission.Steps[_missionsManager.CurrentStepIndex] as IMissionStep;
		}
		catch (System.IndexOutOfRangeException)
		{
			_currentMissionStepCache = null;
		}
	}
}
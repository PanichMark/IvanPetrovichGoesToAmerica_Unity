using UnityEngine;
using System.Collections.Generic; // Для List<>

public class MissionObjectiveMarker : MonoBehaviour
{
	// --- Приватные зависимости ---
	private MissionsManager _missionsManager;
	private Transform _playerCameraTransform;
	private RectTransform _UIimageRectTransform;
	private Camera _worldCamera;
	private WorldToUISpace _uiImageLogicComponent;
	private Vector3 _worldOffset = new Vector3(0, 2f, 0);
	private GameObject _targetGameObject;
	// Кэшируем текущий шаг для оптимизации
	private ICurrentMissionStep _currentMissionStepCache;

	public void Initialize(
		MissionsManager missionsManager,
		GameObject playerCamera,
		RectTransform canvas,
		WorldToUISpace uiImageLogicComponent)
	{
		_missionsManager = missionsManager;
		_playerCameraTransform = playerCamera.transform;
		_UIimageRectTransform = canvas;
		_worldCamera = playerCamera.GetComponent<Camera>();
		_uiImageLogicComponent = uiImageLogicComponent;

		if (_missionsManager != null)
		{
			MissionsManager.OnCurrentStepChanged += HandleStepChanged;

			Debug.Log("[MissionMarker] Первая проверка цели (может быть неудачной).");
			//HandleStepChanged();
			//Invoke(nameof(RequestRecheck), 0.1f);
		}
	}
	/*
	private void Update()
	{
		Vector3 screenPoint = _worldCamera.WorldToViewportPoint(_targetGameObject.transform.position);
	//	bool isVisible = screenPoint.z > 0f && IsOnScreen(screenPoint);
	//	_uiElement.gameObject.SetActive(isVisible);

	//	if (!isVisible) return;

		float xPos = screenPoint.x * Screen.width;
		float yPos = screenPoint.y * Screen.height;

		_UIimageRectTransform.anchoredPosition = new Vector2(xPos, yPos);

		// --- ОТЛАДОЧНЫЕ ЛОГИ ---
		// Выводим позицию целевого объекта в мировом пространстве
		Debug.Log($"[UI Marker] World Pos: {_targetGameObject.transform.position}");

		// Выводим рассчитанную позицию UI элемента (в пикселях относительно экрана)
		Debug.Log($"[UI Marker] UI Position (px): ({xPos}, {yPos})");

		// Выводим итоговую позицию, которая присваивается RectTransform
		Debug.Log($"[UI Marker] Anchored Position set to: {_UIimageRectTransform.anchoredPosition}");
	}
	*/
	/// <summary>
	/// Реакция на смену шага миссии или внешний запрос.
	/// </summary>
	/// 
	
	private void HandleStepChanged()
	{
		UpdateCurrentStepCache();

		// Ищем ЛЮБОЙ объект в мире, который является владельцем невыполненного условия
		GameObject targetObject = FindActiveTargetObject();

		bool shouldBeVisible = targetObject != null;
		UpdateMarkerState(shouldBeVisible, targetObject?.transform.position ?? Vector3.zero);
	}
	

	/// <summary>
	/// Ищет первый попавшийся объект в мире, который является целью текущей миссии.
	/// </summary>
	/// <returns>GameObject-цель или null, если целей нет.</returns>
	private GameObject FindActiveTargetObject()
	{
		if (_currentMissionStepCache == null) return null;

		foreach (var condition in _currentMissionStepCache.Conditions)
		{
			string ownerName = condition.Owner ? condition.Owner.name : "NULL";
			bool conditionMet = condition.IsMet();

			Debug.Log($"[MissionMarker] Условие: '{condition.GetType().Name}'. Владелец: {ownerName}. Выполнено: {conditionMet}");

			// Если у условия есть владелец И оно НЕ выполнено - этот объект наша цель!
			if (condition.Owner != null && !conditionMet)
			{
				Debug.Log($"[MissionMarker] Найдена активная цель: {condition.Owner.name}");
				_targetGameObject = condition.Owner;
				return condition.Owner;
			}
		}
		Debug.Log("[MissionMarker] Активных целей для отслеживания не найдено.");
		return null;
	}

	/// <summary>
	/// Обновляет состояние видимости и позицию маркера.
	/// </summary>
	/// 
	
	private void UpdateMarkerState(bool shouldBeVisible, Vector3 worldPositionOfTarget)
	{
		if (shouldBeVisible && _uiImageLogicComponent != null)
		{
			ShowAndUpdateMarker(worldPositionOfTarget);
		}
		else
		{
			HideMarker();
		}
	}
	
	
	private void ShowAndUpdateMarker(Vector3 worldPositionOfTarget)
	{
		if (_uiImageLogicComponent == null || _worldCamera == null) return;

		Vector3 targetPositionForMarker = worldPositionOfTarget + _worldOffset;

		//_uiImageLogicComponent.Initialize(_UIimageRectTransform, _worldCamera);
		_uiImageLogicComponent.UpdatePosition(targetPositionForMarker);

		if (!_uiImageLogicComponent.gameObject.activeSelf)
		{
			_uiImageLogicComponent.gameObject.SetActive(true);
		}
	}

	private void HideMarker()
	{
		if (_uiImageLogicComponent != null && _uiImageLogicComponent.gameObject.activeSelf)
		{
			_uiImageLogicComponent.gameObject.SetActive(false);
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
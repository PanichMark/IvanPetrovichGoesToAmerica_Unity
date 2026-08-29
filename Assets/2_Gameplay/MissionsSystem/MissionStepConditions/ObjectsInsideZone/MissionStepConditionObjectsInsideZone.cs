using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StepConditionObjectsInZone", menuName = "Missions/MissionStepConditions/StepConditionObjectsInsideZone")]
public class MissionStepConditionObjectsInsideZone : MissionStepConditionAbstract, IMissionStepConditionWithProgress
{
	// Основной список: кто должен быть внутри (заполняется Required-скриптами только если пуст)
	private readonly List<GameObject> _registeredObjects = new();

	// Временный список: кто сейчас физически внутри (сбрасывается всегда)
	private readonly List<GameObject> _objectsCurrentlyInside = new();


	public event Action<int, int> OnStepConditionProgressUpdated;

	/*
	public override bool IsConditionMet()
	{
		return _isConditionMet;
	}
	*/

	public override void ResetStepCondition()
	{
		Debug.Log($"[Условие][RESET] Сброс шага. Очистка временных списков.");

		_isConditionMet = false;

		// ВАЖНО: Не очищаем _registeredObjects здесь насовсем, иначе потеряем данные дизайнера.
		// Но очищаем текущее состояние для теста.
		_registeredObjects.Clear();
		_objectsCurrentlyInside.Clear();
	}

	internal void RegisterTrackedObject(GameObject obj)
	{
		if (_isConditionMet) return;

		// Защита от дубликатов при многократных стартах в редакторе
		if (!_registeredObjects.Contains(obj))
		{
			_registeredObjects.Add(obj);
			Debug.Log($"[Условие][Init] Зарегистрирован Required-объект: {obj.name}. Всего в списке ожидания: {_registeredObjects.Count}");
		}
	}

	internal void InitializeWithZone(GameObject zoneObj)
	{
	

		_stepConditionOwner = zoneObj;
		Debug.Log($"[Условие][Init] Владелец (ZoneMain) зафиксирован: {_stepConditionOwner.name}");
	}

	internal void ReportObjectEntered(GameObject obj)
	{
		if (_isConditionMet) return;

		if (_registeredObjects.Contains(obj) && !_objectsCurrentlyInside.Contains(obj))
		{
			_objectsCurrentlyInside.Add(obj);

			int remaining = _registeredObjects.Count - _objectsCurrentlyInside.Count;
			OnStepConditionProgressUpdated?.Invoke(_objectsCurrentlyInside.Count, _registeredObjects.Count);
			Debug.Log($"[Условие][Вход] Объект: {obj.name}. Внутри: {_objectsCurrentlyInside.Count}/{_registeredObjects.Count}. Осталось: {remaining}");

			if (remaining == 0 && _registeredObjects.Count > 0)
			{
				CompleteMission();
			}
		}
	}

	internal void ReportObjectExited(GameObject obj)
	{
		if (_isConditionMet) return;

		if (_objectsCurrentlyInside.Contains(obj))
		{
			_objectsCurrentlyInside.Remove(obj);

			int remaining = _registeredObjects.Count - _objectsCurrentlyInside.Count;
			OnStepConditionProgressUpdated?.Invoke(_objectsCurrentlyInside.Count, _registeredObjects.Count);
			Debug.Log($"[Условие][Выход] Объект: {obj.name}. Внутри: {_objectsCurrentlyInside.Count}/{_registeredObjects.Count}. Осталось: {remaining}");
		}
	}

	private void CompleteMission()
	{
		_isConditionMet = true;
		_missionStepAbstract.OnStepCompleted(GoToNextStep);
		Debug.Log($"[Условие] УСПЕХ! Все объекты внутри {_stepConditionOwner.name}. Шаг выполнен.");
		//NotifyMissionManagerOnStepCompletion();
	}
}
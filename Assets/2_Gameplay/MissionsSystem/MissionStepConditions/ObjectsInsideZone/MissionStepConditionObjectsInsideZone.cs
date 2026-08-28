using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StepConditionObjectsInZone", menuName = "Missions/MissionStepConditions/StepConditionObjectsInsideZone")]
public class MissionStepConditionObjectsInsideZone : MissionStepConditionAbstract, IMissionStepCondition
{
	// Основной список: кто должен быть внутри (заполняется Required-скриптами только если пуст)
	private readonly List<GameObject> _registeredObjects = new();

	// Временный список: кто сейчас физически внутри (сбрасывается всегда)
	private readonly List<GameObject> _objectsCurrentlyInside = new();

	private bool _isCompleted;
	private GameObject _zoneOwner;

	public bool IsMet() => _isCompleted;
	public GameObject Owner => _zoneOwner;

	public override bool IsConditionMet()
	{
		return _isCompleted;
	}

	public void ResetStepConditionMetStateInEditMode()
	{
		Debug.Log($"[Условие][RESET] Сброс шага. Очистка временных списков.");

		_isCompleted = false;

		// ВАЖНО: Не очищаем _registeredObjects здесь насовсем, иначе потеряем данные дизайнера.
		// Но очищаем текущее состояние для теста.
		_objectsCurrentlyInside.Clear();
	}

	internal void RegisterTrackedObject(GameObject obj)
	{
		if (_isCompleted) return;

		// Защита от дубликатов при многократных стартах в редакторе
		if (!_registeredObjects.Contains(obj))
		{
			_registeredObjects.Add(obj);
			Debug.Log($"[Условие][Init] Зарегистрирован Required-объект: {obj.name}. Всего в списке ожидания: {_registeredObjects.Count}");
		}
	}

	internal void InitializeWithZone(GameObject zoneObj)
	{
		_zoneOwner = zoneObj;
		Debug.Log($"[Условие][Init] Владелец (ZoneMain) зафиксирован: {_zoneOwner.name}");
	}

	internal void ReportObjectEntered(GameObject obj)
	{
		if (_isCompleted) return;

		if (_registeredObjects.Contains(obj) && !_objectsCurrentlyInside.Contains(obj))
		{
			_objectsCurrentlyInside.Add(obj);

			int remaining = _registeredObjects.Count - _objectsCurrentlyInside.Count;
			Debug.Log($"[Условие][Вход] Объект: {obj.name}. Внутри: {_objectsCurrentlyInside.Count}/{_registeredObjects.Count}. Осталось: {remaining}");

			if (remaining == 0 && _registeredObjects.Count > 0)
			{
				CompleteMission();
			}
		}
	}

	internal void ReportObjectExited(GameObject obj)
	{
		if (_isCompleted) return;

		if (_objectsCurrentlyInside.Contains(obj))
		{
			_objectsCurrentlyInside.Remove(obj);

			int remaining = _registeredObjects.Count - _objectsCurrentlyInside.Count;
			Debug.Log($"[Условие][Выход] Объект: {obj.name}. Внутри: {_objectsCurrentlyInside.Count}/{_registeredObjects.Count}. Осталось: {remaining}");
		}
	}

	private void CompleteMission()
	{
		_isCompleted = true;
		Debug.Log($"[Условие] УСПЕХ! Все объекты внутри {_zoneOwner.name}. Шаг выполнен.");
		NotifyMissionManager();
	}
}
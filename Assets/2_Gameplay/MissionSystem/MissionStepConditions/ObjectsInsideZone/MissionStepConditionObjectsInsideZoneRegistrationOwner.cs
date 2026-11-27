using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MissionStepConditionObjectsInsideZoneRegistrationOwner : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;
	private GameObject _playerSpine;

	[SerializeField] private bool _updateZoneStatus;
	[SerializeField] private bool _isSubjectPlayer;
	private GameObject _player;
	private MissionsManager _missionsManager;

	private void Start()
	{
		_missionsManager = ServiceLocator.Resolve<MissionsManager>();
		_playerSpine = ServiceLocator.Resolve(ServiceLocatorPlayerGameObjectsEnum.PlayerSpineBone);
		if (_isSubjectPlayer)
		{
			_player = ServiceLocator.Resolve(ServiceLocatorPlayerGameObjectsEnum.Player);
			RegisterTrackedSubject(_player);
		}

		// Говорим условию: "Я — твоя зона, зафиксируй меня как Owner"
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			condition.InitializeWithZone(gameObject, _updateZoneStatus);
		}
	}

	public void RegisterTrackedSubject(GameObject subject)
	{
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			condition.RegisterTrackedSubject(subject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			// Передаем того, кто реально вышел из триггера
			var target = HasRequiredComponent(other.gameObject);

			if (target != null)
			{
				condition.ReportSubjectEntered(target); // Условие получит именно тот объект, где лежит скрипт
				Debug.Log(_missionsManager.ActiveMission.MissionSteps[_missionsManager.CurrentStepIndex].Conditions[0].GetType());
			}

			

			if (_missionsManager.ActiveMission.MissionSteps[_missionsManager.CurrentStepIndex].Conditions[0] is not MissionStepConditionObjectInteracted)
			{
				// so basically the thing is -
				// its for the returning to releasing ChickenCage in BistroOwner MissionSteps
				// if player has to pick up up AGAIN 
				// and if he triggers the ObjectsInsideZone withOUT the cage
				// it DOESNT count!

				if (_isSubjectPlayer && other.gameObject == _player)
				{
					condition.ReportSubjectEntered(_player);
				}
			}

		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			// Передаем того, кто реально вышел из триггера
			var target = HasRequiredComponent(other.gameObject);

			if (target != null)
			{
				condition.ReportSubjectExited(target); // Условие получит именно тот объект, где лежит скрипт
			}

			if (_missionsManager.ActiveMission.MissionSteps[_missionsManager.CurrentStepIndex].Conditions[0] is not MissionStepConditionObjectInteracted)
			{
				// so basically the thing is -
				// its for the returning to releasing ChickenCage in BistroOwner MissionSteps
				// if player has to pick up up AGAIN 
				// and if he triggers the ObjectsInsideZone withOUT the cage
				// it DOESNT count!

				if (_isSubjectPlayer && other.gameObject == _player)
				{
					condition.ReportSubjectExited(_player);
				}
			}
		}
	}

	// Теперь ищем Transform (так быстрее) и возвращаем его GameObject
	private GameObject HasRequiredComponent(GameObject obj)
	{
		// 1. Проверяем сам объект, который пересек границу (например, ящик в руках игрока)
		if (obj.TryGetComponent<MissionStepConditionObjectsInsideZoneRegistrationSubject>(out _))
		{
			//Debug.Log($"[ZoneMain] Found on self: {obj.name}");
			return obj;
		}

		// 2. Проверяем родителя этого объекта (если это деталь механизма)
		var parent = obj.transform.parent;
		if (parent != null)
		{
			if (parent.TryGetComponent<MissionStepConditionObjectsInsideZoneRegistrationSubject>(out _))
			{
				//Debug.Log($"[ZoneMain] Found on direct parent: {parent.name}");
				return parent.gameObject;
			}
		}

		// 3. Только если на них не нашли — лезем проверять ВСЕХ детей кости Spine у игрока
		if (_playerSpine == null) return null;

		var allChildren = _playerSpine.GetComponentsInChildren<Transform>(includeInactive: true);

		foreach (var child in allChildren)
		{
			//Debug.Log($"[ZoneMain] Checking child of Spine: {child.name}"); // Закомментировано для чистоты лога

			if (child.TryGetComponent<MissionStepConditionObjectsInsideZoneRegistrationSubject>(out _))
			{
				//Debug.Log($"[ZoneMain] SUCCESS! Found required component on Player's child: {child.name}");
				return child.gameObject;
			}
		}

		// Ничего не найдено нигде
		return null;
	}
}
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MissionStepObjectRegistrationObjectZoneMain : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;
	private GameObject _playerSpine;

	private void Start()
	{
		_playerSpine = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerSpineBone);

		var collider = GetComponent<Collider>();
		if (!collider.isTrigger)
		{
			Debug.LogError($"{name}: Коллайдер должен быть типа Trigger!");
			enabled = false;
			return;
		}

		// Говорим условию: "Я — твоя зона, зафиксируй меня как Owner"
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			condition.InitializeWithZone(gameObject);
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
				condition.ReportObjectEntered(target); // Условие получит именно тот объект, где лежит скрипт
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
				condition.ReportObjectExited(target); // Условие получит именно тот объект, где лежит скрипт
			}
		}
	}

	// Теперь ищем Transform (так быстрее) и возвращаем его GameObject
	private GameObject HasRequiredComponent(GameObject obj)
	{
		// 1. Проверяем сам объект, который пересек границу (например, ящик в руках игрока)
		if (obj.TryGetComponent<MissionStepObjectRegistrationObjectZoneRequired>(out _))
		{
			//Debug.Log($"[ZoneMain] Found on self: {obj.name}");
			return obj;
		}

		// 2. Проверяем родителя этого объекта (если это деталь механизма)
		var parent = obj.transform.parent;
		if (parent != null)
		{
			if (parent.TryGetComponent<MissionStepObjectRegistrationObjectZoneRequired>(out _))
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

			if (child.TryGetComponent<MissionStepObjectRegistrationObjectZoneRequired>(out _))
			{
				//Debug.Log($"[ZoneMain] SUCCESS! Found required component on Player's child: {child.name}");
				return child.gameObject;
			}
		}

		// Ничего не найдено нигде
		return null;
	}
}
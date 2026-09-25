using UnityEngine;

public class MissionStepObjectRegistrationObjectZoneRequired : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	private void Start()
	{
		// Проверка 1: Присоединен ли вообще ScriptableObject в инспекторе
		if (_linkedMissionCondition == null)
		{
			Debug.LogWarning($"[Required Reg] На объекте {gameObject.name} висит скрипт регистрации, но не задано условие (Linked Mission Condition).", gameObject);
			return;
		}

		// Проверка 2: Является ли переданный объект именно нашим типом условия?
		if (_linkedMissionCondition is MissionStepConditionObjectsInsideZone condition)
		{
			// Проверка 3: Есть ли у самого объекта Collider? (Опционально, но полезно для физики триггеров)
			if (TryGetComponent<Collider>(out var ownCollider) && !ownCollider.isTrigger)
			{
				Debug.LogWarning($"[Required Reg] У объекта {gameObject.name} есть коллайдер, но он НЕ Trigger. Объект может не сработать при входе в зону.", gameObject);
			}

			condition.RegisterTrackedObject(gameObject);
			Debug.Log($"[Required Reg] Объект {gameObject.name} успешно зарегистрирован в условии {condition.name}.");
		}
		else
		{
			// Если дизайнер ошибся и перетащил сюда другой тип условия (например, OnInteraction)
			Debug.LogError($"[Required Reg] Ошибка настройки! На объекте {gameObject.name} в поле Linked Mission Condition лежит '{_linkedMissionCondition.GetType().Name}', " +
						   $"а требуется 'MissionStepConditionObjectsInsideZone'. Регистрация пропущена.", gameObject);
		}
	}
}
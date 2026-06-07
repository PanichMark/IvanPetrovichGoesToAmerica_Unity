// Файл: MissionStepConditionOnInteraction.cs (в сборке Gameplay)
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionOnInteraction", menuName = "Missions/MissionConditions/ConditionOnInteraction")]
public class MissionStepConditionOnInteraction : MissionStepConditionAbstract, ICurrentMissionCondition
{
	private bool _isCompleted = false;
	// --- РЕАЛИЗАЦИЯ ИНТЕРФЕЙСА ---
	public bool IsMet() => _isCompleted;
	public GameObject Owner => base.OwnerObject;
	// ---------------------------

	// ... остальной код класса остается БЕЗ ИЗМЕНЕНИЙ ...
	public override bool IsConditionMet()
	{
		return _isCompleted;
	}

	// Этот метод будет вызываться из скрипта на объекте (например, DoorScript)
	public void OnPlayerInteracted(GameObject interactedObject)
	{
		// Проверяем, что у нас есть владелец и что взаимодействовали именно с ним
		if (OwnerObject != null && interactedObject == OwnerObject && !_isCompleted)
		{
			_isCompleted = true;
			Debug.Log($"[Условие] Взаимодействие с {OwnerObject.name} засчитано.");
			NotifyMissionManager(); // Сообщаем менеджеру о завершении шага
		}
	}
}
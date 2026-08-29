// Файл: MissionStepConditionOnInteraction.cs (в сборке Gameplay)
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StepConditionOnInteraction", menuName = "Missions/MissionStepConditions/StepConditionOnInteraction")]
public class MissionStepConditionInteractable : MissionStepConditionAbstract
{

	// --- РЕАЛИЗАЦИЯ ИНТЕРФЕЙСА ---
	
	// ---------------------------

	// ... остальной код класса остается БЕЗ ИЗМЕНЕНИЙ ...
	//public override bool IsConditionMet()
	//{
		//return _isConditionMet;
	//}

	// Этот метод будет вызываться из скрипта на объекте (например, DoorScript)
	public void OnPlayerInteracted(GameObject interactedObject)
	{
		// Проверяем, что у нас есть владелец и что взаимодействовали именно с ним
		if (_stepConditionOwner != null && interactedObject == _stepConditionOwner && !_isConditionMet)
		{
			_isConditionMet = true;
			_missionStepAbstract.OnStepCompleted(GoToNextStep);
			Debug.Log($"[Условие] Взаимодействие с {_stepConditionOwner.name} засчитано.");
			//NotifyMissionManagerOnStepCompletion(); // Сообщаем менеджеру о завершении шага
		}
	}

	public override void ResetStepCondition()
	{
		_isConditionMet = false;
	}
}
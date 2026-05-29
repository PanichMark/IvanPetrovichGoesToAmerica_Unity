using UnityEngine;

public class MissionsManager : MonoBehaviour
{
	private GameMissions _allMissions;
	public MissionAbstract ActiveMission { get; private set; }
	private int _currentStepIndex = 0;

	private PauseMenuController _pauseMenuController;

	public delegate void InteractionEventHandler(GameObject interactedObject);
	public static event InteractionEventHandler OnAnyObjectInteracted;

	public delegate void DestructionEventHandler(GameObject destroyedObject, bool wasLethal);
	public static event DestructionEventHandler OnAnyObjectDestroyed;

	public void Initialize(PauseMenuController pauseMenuController, GameMissions gameMissions)
	{
		_pauseMenuController = pauseMenuController;
		_allMissions = gameMissions;

		if (_allMissions == null || _allMissions.MissionsInOrder.Length == 0)
		{
			Debug.LogError("Ошибка: Список миссий не задан или пуст!");
			return;
		}

		ActiveMission = _allMissions.MissionsInOrder[0];
		_currentStepIndex = 0;

		Debug.Log("Система миссий инициализирована.");
		Debug.Log($"Активирована первая миссия: {ActiveMission.name}");

		if (ActiveMission.Steps.Length > 0)
		{
			// Получаем текущий шаг (на старте это всегда первый шаг, индекс 0)
			MissionStepAbstract currentStep = ActiveMission.Steps[_currentStepIndex];

			// Передаем текст цели в UI-меню
			_pauseMenuController.SetCurrentMissionGoalText(currentStep.MissionStepGoal);

			Debug.Log($"Миссия: {ActiveMission.name} - Шаг 1");
		}
	}

	public void CheckAndCompleteCurrentStep()
	{
		if (ActiveMission == null) return;
		if (_currentStepIndex >= ActiveMission.Steps.Length) return;

		ActiveMission.Steps[_currentStepIndex].OnStepCompleted();
	}

	public void CompleteCurrentStep()
	{
		_currentStepIndex++;

		// --- ДОБАВЬТЕ ЭТОТ БЛОК ---
		// Проверяем, есть ли следующий шаг, прежде чем пытаться получить его текст
		if (_currentStepIndex < ActiveMission.Steps.Length)
		{
			MissionStepAbstract nextStep = ActiveMission.Steps[_currentStepIndex];
			_pauseMenuController.SetCurrentMissionGoalText(nextStep.MissionStepGoal);
		}
		else
		{
			// Если шагов больше нет, можно очистить текст или вывести "Миссия завершена"
			_pauseMenuController.SetCurrentMissionGoalText("");
		}
		// ---------------------------

		Debug.Log($"ШАГ МИССИИ ВЫПОЛНЕН! Переход к шагу {_currentStepIndex + 1}.");

		if (_currentStepIndex < ActiveMission.Steps.Length)
		{
			Debug.Log($"Миссия: {ActiveMission.name} - Шаг {_currentStepIndex + 1}");
		}
		else
		{
			EndMission();
		}
	}

	private void StartNextMission()
	{
		int currentMissionIndex = System.Array.IndexOf(_allMissions.MissionsInOrder, ActiveMission);

		if (currentMissionIndex + 1 < _allMissions.MissionsInOrder.Length)
		{
			ActiveMission = _allMissions.MissionsInOrder[currentMissionIndex + 1];
			_currentStepIndex = 0;

			Debug.Log($"Начато прохождение следующей миссии: {ActiveMission.name}");
			Debug.Log($"Миссия: {ActiveMission.name} - Шаг 1");
		}
		else
		{
			Debug.Log("ВСЕ МИССИИ ЗАВЕРШЕНЫ! Игра пройдена.");
		}
	}

	private void EndMission()
	{
		Debug.Log("Текущая миссия завершена!");
		StartNextMission();
	}
}
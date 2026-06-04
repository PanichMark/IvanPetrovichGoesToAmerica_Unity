using TMPro;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
	private GameMissions _allMissions;
	public MissionAbstract ActiveMission { get; private set; }
	public int CurrentStepIndex { get; private set; } = 0;

	private PauseMenuController _pauseMenuController;
	private LocalizationManager _localizationManager;

	// --- СОБЫТИЕ ДЛЯ УВЕДОМЛЕНИЯ ---
	public delegate void OnStepChangedDelegate();
	public static event OnStepChangedDelegate OnCurrentStepChanged;

	public delegate void InteractionEventHandler(GameObject interactedObject);
	public static event InteractionEventHandler OnAnyObjectInteracted;

	public delegate void DestructionEventHandler(GameObject destroyedObject, bool wasLethal);
	public static event DestructionEventHandler OnAnyObjectDestroyed;
	private GameObject _textCurrentMissionGoal;
	private TextMeshProUGUI _textComponentCurrentMissionGoal;
	public void Initialize(LocalizationManager localizationManager, PauseMenuController pauseMenuController, GameMissions gameMissions, ViewModelPauseMenu viewModelPauseMenu)
	{
		_localizationManager = localizationManager;
		_pauseMenuController = pauseMenuController;
		_allMissions = gameMissions;
		_textCurrentMissionGoal = viewModelPauseMenu.TextCurrentMissionGoalDisplay;
		_textComponentCurrentMissionGoal = _textCurrentMissionGoal.GetComponent<TextMeshProUGUI>();
		if (_allMissions == null || _allMissions.MissionsInOrder.Length == 0)
		{
			Debug.LogError("Ошибка: Список миссий не задан или пуст!");
			return;
		}

		ActiveMission = _allMissions.MissionsInOrder[0];
		CurrentStepIndex = 0;

		//Debug.Log("Система миссий инициализирована.");
		//Debug.Log($"Активирована первая миссия: {ActiveMission.name}");

		if (ActiveMission.Steps.Length > 0)
		{
			// Получаем текущий шаг (на старте это всегда первый шаг, индекс 0)
			MissionStepAbstract currentStep = ActiveMission.Steps[CurrentStepIndex];

			// --- ИЗМЕНЕНИЕ ---
			// Вызываем наш новый метод для получения локализованного текста
			string localizedGoalText = GetLocalizedGoalText(currentStep);
			SetCurrentMissionGoalText(localizedGoalText);
			// ----------------

			//Debug.Log($"Миссия: {ActiveMission.name} - Шаг 1");
		}

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		Debug.Log("MissionsManager Initialized");
	}

	public void CheckAndCompleteCurrentStep()
	{
		if (ActiveMission == null) return;
		if (CurrentStepIndex >= ActiveMission.Steps.Length) return;

		ActiveMission.Steps[CurrentStepIndex].OnStepCompleted();
	}

	public void CompleteCurrentStep()
	{
		CurrentStepIndex++;

		if (CurrentStepIndex < ActiveMission.Steps.Length)
		{
			string localizedGoalText = GetLocalizedGoalText(ActiveMission.Steps[CurrentStepIndex]);
			SetCurrentMissionGoalText(localizedGoalText);
		}
		else
		{
			SetCurrentMissionGoalText("");
		}

		// --- УВЕДОМЛЕНИЕ ЧЕРЕЗ DELEGATE ---
		OnCurrentStepChanged?.Invoke();

		if (CurrentStepIndex >= ActiveMission.Steps.Length)
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
			CurrentStepIndex = 0;
		}
	}

	private void EndMission()
	{
		StartNextMission();
	}

	private string GetLocalizedGoalText(MissionStepAbstract step)
	{
		if (step == null || _localizationManager == null) return string.Empty;

		if (_localizationManager.CurrentLanguage == LanguagesEnum.Russian)
		{
			return step.MissionStepGoal_RU;
		}
		else
		{
			return step.MissionStepGoal_EN;
		}
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (ActiveMission != null && ActiveMission.Steps.Length > 0)
		{
			string localizedGoalText = GetLocalizedGoalText(ActiveMission.Steps[CurrentStepIndex]);
			SetCurrentMissionGoalText(localizedGoalText);
		}
	}

	public void SetCurrentMissionGoalText(string textGoal)
	{
		_textComponentCurrentMissionGoal.text = textGoal;
	}
}
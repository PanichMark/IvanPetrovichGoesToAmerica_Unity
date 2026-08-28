using System.Collections;
using UnityEngine;

public class MissionsManager : MonoBehaviour, IJsonSaveLoad
{
	private IMissionStepConditionWithProgress _missionStepConditionWithProgress;

	private GameMissionsList _gameMissions;
	public MissionAbstract ActiveMission { get; private set; }
	public int CurrentStepIndex { get; private set; }
	private LocalizationManager _localizationManager;
	private HUDmissionsController _HUDmissionsController;
	
	public delegate void OnStepChangedDelegate();
	public event OnStepChangedDelegate OnCurrentStepChanged;
	private GameScenesManager _gameSceneManager;
	public delegate void InteractionEventHandler(GameObject interactedObject);
	public event InteractionEventHandler OnAnyObjectInteracted;

	public delegate void DestructionEventHandler(GameObject destroyedObject, bool wasLethal);
	public event DestructionEventHandler OnAnyObjectDestroyed;


	public void Initialize(
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		HUDmissionsController HUDmissionsController,
		GameMissionsList gameMissions)
	{
		_localizationManager = localizationManager;
		_gameSceneManager = gameSceneManager;
		_HUDmissionsController = HUDmissionsController;

		_gameMissions = gameMissions;

		ActiveMission = _gameMissions.MissionsInOrder[0];

		CurrentStepIndex = 0;
		Debug.Log(ActiveMission);
		if (ActiveMission.Steps.Length > 0)
		{
			string localizedGoalText = GetLocalizedGoalText(ActiveMission.Steps[CurrentStepIndex]);
			_HUDmissionsController.SetCurrentMissionGoalText(localizedGoalText);
		}

		_localizationManager.OnLanguageChanged += ChangeLanguage;
		_gameSceneManager.OnEndLoadingGameplayScene += ShowMissionGoalHUDonSceneLoad;

		Debug.Log("MissionsManager Initialized");
	}

	private void ResetStepConditionMetStateInEditMode()
	{
		if (ActiveMission == null || CurrentStepIndex < 0 || CurrentStepIndex >= ActiveMission.Steps.Length)
			return;

		var currentStep = ActiveMission.Steps[CurrentStepIndex];

		// Приводим шаг к интерфейсу, чтобы получить доступ к списку условий
		if (currentStep is IMissionStep typedStep)
		{
			foreach (var condition in typedStep.Conditions)
			{
				if (condition is IMissionStepCondition resettableCondition)
				{
					resettableCondition.ResetStepConditionMetStateInEditMode();
				}
			}
		}
	}

	public void CheckAndCompleteCurrentStep()
	{
		if (ActiveMission == null) return;
		if (CurrentStepIndex >= ActiveMission.Steps.Length) return;

		ActiveMission.Steps[CurrentStepIndex].OnStepCompleted();
	}

	private void ShowMissionGoalHUDonSceneLoad()
	{
		string localizedGoalText = GetLocalizedGoalText(ActiveMission.Steps[CurrentStepIndex]);


		_HUDmissionsController.ShowNewMissionGoalHUDnotification(localizedGoalText);
	}

	public void CompleteCurrentStep(bool isCalledByLoadSafeFile)
	{
		if (!isCalledByLoadSafeFile)
		{
			CurrentStepIndex++;
		}

		ResetStepConditionMetStateInEditMode();

		if (CurrentStepIndex < ActiveMission.Steps.Length)
		{
			string localizedGoalText = GetLocalizedGoalText(ActiveMission.Steps[CurrentStepIndex]);

			_HUDmissionsController.ShowNewMissionGoalHUDnotification(localizedGoalText);
			_HUDmissionsController.SetCurrentMissionGoalText(localizedGoalText);
		}
		else
		{
			_HUDmissionsController.SetCurrentMissionGoalText("");
		}

		OnCurrentStepChanged?.Invoke();

		Debug.Log(CurrentStepIndex);
		Debug.Log(ActiveMission.Steps[CurrentStepIndex].Conditions[0].GetType());
		if (ActiveMission.Steps[CurrentStepIndex].Conditions[0] is IMissionStepConditionWithProgress stepWithProgress)
		{
			SubscribeToStepProgress(stepWithProgress);
		}


		if (CurrentStepIndex >= ActiveMission.Steps.Length)
		{
			EndMission();
		}
	}

	private void SubscribeToStepProgress(IMissionStepConditionWithProgress condition)
	{
		if (condition == null) return;

		// Отписываемся от предыдущего (на случай, если это повторный вызов), 
		// хотя лучше делать полную отписку при смене шага.
		condition.OnProgressUpdated -= HandleStepProgress;

		// Подписываемся на новое событие
		condition.OnProgressUpdated += HandleStepProgress;

		// Сразу рисуем UI текущими данными из этого объекта
		//HandleStepProgress(condition.CurrentProgress, condition.MaxProgress);
	}

	private void HandleStepProgress(int currentAmount, int requiredAmount)
	{
		Debug.Log(currentAmount);
		Debug.Log(requiredAmount);

		string localizedGoalText = GetLocalizedGoalText(ActiveMission.Steps[CurrentStepIndex]);

		_HUDmissionsController.ShowNewMissionGoalHUDnotification($"{localizedGoalText}: {currentAmount}/{requiredAmount}");
	}

	private void StartNextMission()
	{
		int currentMissionIndex = System.Array.IndexOf(_gameMissions.MissionsInOrder, ActiveMission);

		if (currentMissionIndex < _gameMissions.MissionsInOrder.Length)
		{
			ActiveMission = _gameMissions.MissionsInOrder[currentMissionIndex];
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
			//Debug.Log(CurrentStepIndex);
			string localizedGoalText = GetLocalizedGoalText(ActiveMission.Steps[CurrentStepIndex]);
			_HUDmissionsController.SetCurrentMissionGoalText(localizedGoalText);
		}
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		data.MissionData.Mission = ActiveMission.MissionName;
		data.MissionData.MissionStep = CurrentStepIndex;
		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		CurrentStepIndex = data.MissionData.MissionStep + 2;
		CompleteCurrentStep(true);

		//Debug.Log(CurrentStepIndex);

		yield return null;
	}
}
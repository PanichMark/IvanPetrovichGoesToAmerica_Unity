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

		ResetAllStepConditions();

		Debug.Log("MissionsManager Initialized");
	}

	private void ResetAllStepConditions()
	{
		if (_gameMissions == null || _gameMissions.MissionsInOrder == null)
			return;

		foreach (var mission in _gameMissions.MissionsInOrder)
		{
			if (mission == null || mission.Steps == null) continue;

			foreach (var step in mission.Steps)
			{
				if (step is IMissionStep typedStep)
				{
					foreach (var condition in typedStep.Conditions)
					{
						if (condition is IMissionStepCondition resettableCondition)
						{
							resettableCondition.ResetStepCondition();
						}
					}
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

		if (ActiveMission.Steps[CurrentStepIndex].Conditions[0] is IMissionStepConditionWithProgress)
		{
			_missionStepConditionWithProgress = ActiveMission.Steps[CurrentStepIndex].Conditions[0] as IMissionStepConditionWithProgress;

			_missionStepConditionWithProgress.OnProgressUpdated += HandleStepProgress;
		}
		else if (_missionStepConditionWithProgress != null)
		{
			_missionStepConditionWithProgress.OnProgressUpdated -= HandleStepProgress;
		}


		if (CurrentStepIndex >= ActiveMission.Steps.Length)
		{
			EndMission();
		}
	}

	private void OnDestroy()
	{
		if (_missionStepConditionWithProgress != null)
		{
			_missionStepConditionWithProgress.OnProgressUpdated -= HandleStepProgress;
		}
	}

	private void HandleStepProgress(int currentAmount, int requiredAmount)
	{
		//Debug.Log(currentAmount);
		//Debug.Log(requiredAmount);

		Debug.Log(CurrentStepIndex);

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
		CurrentStepIndex = data.MissionData.MissionStep;
		CompleteCurrentStep(true);

		//Debug.Log(CurrentStepIndex);

		yield return null;
	}
}
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionsManager : MonoBehaviour, IJsonSaveLoad
{
	private IMissionStepConditionWithProgress _missionStepConditionWithProgress;

	private GameMissionsList _gameMissions;
	public Mission ActiveMission { get; private set; }
	public int ActiveMissionIndex { get; private set; }
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

	public string LocalizedGoalText { get; private set; }

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

		

		ActiveMissionIndex = 0;
		CurrentStepIndex = 0;

		ActiveMission = _gameMissions.MissionsInOrder[ActiveMissionIndex];

		Debug.Log(ActiveMission);
		if (ActiveMission.MissionSteps.Length > 0)
		{
			LocalizedGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);
			_HUDmissionsController.SetCurrentMissionGoalText(LocalizedGoalText);

			//ActiveMission.MissionSteps[CurrentStepIndex].OnStepStarted();
		}

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_gameSceneManager.OnEndLoadingGameplayScene += ShowMissionGoalHUDonSceneLoad;

		_gameSceneManager.OnBeginLoadingGameplayScene += () => ResetAllStepTurnOnOffLists();

		ResetAllStepConditions();

		Debug.Log("MissionsManager Initialized");
	}

	private void ResetAllStepConditions()
	{
		if (_gameMissions == null || _gameMissions.MissionsInOrder == null)
			return;

		foreach (var mission in _gameMissions.MissionsInOrder)
		{
			if (mission == null || mission.MissionSteps == null) continue;

			foreach (var step in mission.MissionSteps)
			{
				step.Initialize(this);

				if (step is MissionStep typedStep)
				{
					foreach (var condition in typedStep.Conditions)
					{
						if (condition is MissionStepConditionAbstract resettableCondition)
						{
							resettableCondition.ResetStepCondition();
						}
					}
				}
			}
		}
	}

	private void ResetAllStepTurnOnOffLists()
	{
		if (_gameMissions == null || _gameMissions.MissionsInOrder == null)
			return;

		foreach (var mission in _gameMissions.MissionsInOrder)
		{
			if (mission == null || mission.MissionSteps == null) continue;

			foreach (var step in mission.MissionSteps)
			{
				step.ClearTurnOnOffLists();
			}
		}
	}



	/*
	public void CheckAndCompleteCurrentStep()
	{
		if (ActiveMission == null) return;
		if (CurrentStepIndex >= ActiveMission.MissionSteps.Length) return;

		ActiveMission.MissionSteps[CurrentStepIndex].OnStepCompleted();
	}
	*/

	private void ShowMissionGoalHUDonSceneLoad()
	{
		LocalizedGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);

		if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString())
		{
			_HUDmissionsController.ShowNewMissionGoalHUDnotification(LocalizedGoalText, false);
		}
	}

	public void GoToNextStep(int goToNextStep)
	{

		CurrentStepIndex = goToNextStep;
	
		if (CurrentStepIndex >= ActiveMission.MissionSteps.Length)
		{
			StartNextMission();
		}
		

		if (CurrentStepIndex < ActiveMission.MissionSteps.Length)
		{
			//ActiveMission.MissionSteps[CurrentStepIndex].OnMissionStepStartDoSomwthing();

			ActiveMission.MissionSteps[CurrentStepIndex].OnStepStarted();

			LocalizedGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);

			_HUDmissionsController.ShowNewMissionGoalHUDnotification(LocalizedGoalText, true);
			_HUDmissionsController.SetCurrentMissionGoalText(LocalizedGoalText);
		}
		else
		{
			_HUDmissionsController.SetCurrentMissionGoalText("");
		}

		OnCurrentStepChanged?.Invoke();

		//Debug.Log(CurrentStepIndex);
		//Debug.Log(ActiveMission.MissionSteps[CurrentStepIndex]);
		//Debug.Log(ActiveMission.MissionSteps[CurrentStepIndex].Conditions[0]);
		
		//Debug.Log(ActiveMission.MissionSteps[CurrentStepIndex].Conditions[0].GetType());

		if (ActiveMission.MissionSteps[CurrentStepIndex].Conditions[0] is IMissionStepConditionWithProgress)
		{
			_missionStepConditionWithProgress = ActiveMission.MissionSteps[CurrentStepIndex].Conditions[0] as IMissionStepConditionWithProgress;

			_missionStepConditionWithProgress.OnStepConditionProgressUpdated += HandleStepProgress;
		}
		else if (_missionStepConditionWithProgress != null)
		{
			_missionStepConditionWithProgress.OnStepConditionProgressUpdated -= HandleStepProgress;
		}
		//Debug.Log(CurrentStepIndex);
		//Debug.Log(ActiveMission.MissionSteps.Length);
		//Debug.Log(ActiveMission);
	}

	private void OnDestroy()
	{
		if (_missionStepConditionWithProgress != null)
		{
			_missionStepConditionWithProgress.OnStepConditionProgressUpdated -= HandleStepProgress;
		}
	}

	private void HandleStepProgress(int currentAmount, int requiredAmount)
	{
		//Debug.Log(currentAmount);
		//Debug.Log(requiredAmount);

		Debug.Log(CurrentStepIndex);

		LocalizedGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);

		_HUDmissionsController.ShowNewMissionGoalHUDnotification($"{LocalizedGoalText}: {currentAmount}/{requiredAmount}", false);
	}

	private void StartNextMission()
	{
		//int currentMissionIndex = System.Array.IndexOf(_gameMissions.MissionsInOrder, ActiveMission);

		Debug.Log("NEXT MISSION!");

		if (ActiveMissionIndex <= _gameMissions.MissionsInOrder.Length)
		{
			ActiveMissionIndex++;

			ActiveMission = _gameMissions.MissionsInOrder[ActiveMissionIndex];
			Debug.Log(ActiveMission);
			CurrentStepIndex = 0;
			GoToNextStep(CurrentStepIndex);
		}
	}

	private string GetLocalizedGoalText(MissionStep step)
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

	public void ApplyBootstrapMissionConfigs(Mission setMission, int setMissionStep)
	{
		ActiveMission = setMission;
		GoToNextStep(setMissionStep);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (ActiveMission != null && ActiveMission.MissionSteps.Length > 0)
		{
			//Debug.Log(CurrentStepIndex);
			LocalizedGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);
			_HUDmissionsController.SetCurrentMissionGoalText(LocalizedGoalText);
		}
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		int currentMissionIndex = System.Array.IndexOf(_gameMissions.MissionsInOrder, ActiveMission);
		data.MissionData.Mission = float.Parse(_gameMissions.MissionsInOrder[currentMissionIndex].name.Replace("Mission_", ""), System.Globalization.CultureInfo.InvariantCulture);
		data.MissionData.MissionStep = CurrentStepIndex;

		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		string targetString = data.MissionData.Mission.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
		bool isFound = false;

		for (int i = 0; i < _gameMissions.MissionsInOrder.Length; i++)
		{
			// Убираем префикс "Mission_" и сравниваем остаток строки
			string fileNameWithoutPrefix = _gameMissions.MissionsInOrder[i].name.Replace("Mission_", "");

			Debug.Log(fileNameWithoutPrefix); // Выводим название без префикса
			Debug.Log(targetString);

			if (fileNameWithoutPrefix == targetString)
			{
				ActiveMissionIndex = i;
				ActiveMission = _gameMissions.MissionsInOrder[ActiveMissionIndex];
				isFound = true;

				Debug.Log(ActiveMission);
				break;
			}
		}

		CurrentStepIndex = data.MissionData.MissionStep;
		GoToNextStep(CurrentStepIndex);

		yield return null;
	}
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionsManager : MonoBehaviour, IJsonSaveLoad
{
	private IMissionStepConditionWithProgress _missionStepConditionWithProgress;
	private IMissionStepConditionWithCountdown _missionStepConditionWithCountdown;

	private GameMissionsList _gameMissions;
	public GameMissionsList GameMissions => _gameMissions;
	public Mission ActiveMission { get; private set; }
	public int ActiveMissionIndex { get; private set; }
	public bool IsInitialized {  get; private set; }
	public int CurrentStepIndex { get; private set; }
	private LocalizationManager _localizationManager;
	private HUDmissionsController _HUDmissionsController;
	private JsonSaveLoadController _jsonSaveLoadController;
	public delegate void OnStepChangedDelegate();
	public event OnStepChangedDelegate OnCurrentStepChanged;
	private GameScenesManager _gameSceneManager;

	private int _previousMissionStepIndex;

	private JournalMenuSectionMissionInfoController _journalMenuSectionMissionInfoController;
	public string LocalizedStepGoalText { get; private set; }

	public void Initialize(
		LocalizationManager localizationManager,
		GameScenesManager gameSceneManager,
		JsonSaveLoadController jsonSaveLoadController,
		HUDmissionsController HUDmissionsController,
		JournalMenuSectionMissionInfoController journalMenuSectionMissionInfoController,
		GameMissionsList gameMissions)
	{
		_journalMenuSectionMissionInfoController = journalMenuSectionMissionInfoController;
		_jsonSaveLoadController = jsonSaveLoadController;
		_localizationManager = localizationManager;
		_gameSceneManager = gameSceneManager;
		_HUDmissionsController = HUDmissionsController;

		_gameMissions = gameMissions;

		

		ActiveMissionIndex = 0;
		CurrentStepIndex = 0;

		ActiveMission = _gameMissions.MissionsInOrder[ActiveMissionIndex];
		_journalMenuSectionMissionInfoController.SetMissionInfo(ActiveMission);

		//Debug.Log(ActiveMission);
		if (ActiveMission.MissionSteps.Length > 0)
		{
			LocalizedStepGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);
			_journalMenuSectionMissionInfoController.SetMissionStep(LocalizedStepGoalText);

			//ActiveMission.MissionSteps[CurrentStepIndex].OnStepStarted();
		}

		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_gameSceneManager.OnEndLoadingGameplayScene += ShowMissionGoalHUD;

		_gameSceneManager.OnEndLoadingGameplayScene += ReapplyAllCurrentGameplaySceneActions;


		_gameSceneManager.OnBeginLoadingGameplayScene += ResetAllMissionStepsActionListsAndStuff;



		ResetAllStepConditions();
		IsInitialized = true;
		//Debug.Log("current index" + CurrentStepIndex);
		Debug.Log("MissionsManager Initialized");
	}

	private void Update()
	{
		//Debug.Log(CurrentStepIndex);
	}

	private void ReapplyAllCurrentGameplaySceneActions()
	{
		//if (_isInitialized)
		//{
			for (int i = 0; i < _gameMissions.MissionsInOrder.Length; i++)
			{
				var mission = _gameMissions.MissionsInOrder[i];
				if (i > ActiveMissionIndex)
				{
					break;
				}
				Debug.Log($"Mission index: {i}");

				// Если индексы нужны и для шагов:
				for (int j = 0; j < mission.MissionSteps.Length; j++)
				{
					var step = mission.MissionSteps[j];
					if (j > CurrentStepIndex)
					{
						break;
					}
					Debug.Log($"  Step index: {j}");
					Debug.Log("current index" + CurrentStepIndex);

					step.ApplyAllCurrentGameplaySceneActions();

					

					
				}
			}
		//}
	}

	private void ResetAllStepConditions()
	{
		foreach (var mission in _gameMissions.MissionsInOrder)
		{
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

	private void ResetAllMissionStepsActionListsAndStuff()
	{
		foreach (var mission in _gameMissions.MissionsInOrder)
		{
			foreach (var step in mission.MissionSteps)
			{
				step.ClearAllMissionStepActionsLists();

				foreach (var condition in step.Conditions)
				{
					condition.ResetStepCondition();

					// maybe in the case with back and forth
					// like with crates
					// itsll break progression on LOAD
					//well see...
				}
			}
		}
	}

	private void ShowMissionGoalHUD()
	{
		LocalizedStepGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);

		if (SceneManager.sceneCount > 1)
		{
			if (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString() &&
				SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_MainMenu.ToString() &&
				SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_EndGameTitles.ToString())
			{
				if (_jsonSaveLoadController.IsLoadingFromSaveFile)
				{
					//Debug.Log("IsLoadingFromSaveFile");
					_HUDmissionsController.ShowNewMissionGoalHUDnotification(LocalizedStepGoalText, false);
				}
				else
				{
					//Debug.Log(_previousMissionStepIndex);
					//Debug.Log(CurrentStepIndex);

					if (_previousMissionStepIndex == -1)
					{
						//Debug.Log("_previousMissionStep !=");
						
						_HUDmissionsController.ShowNewMissionGoalHUDnotification(LocalizedStepGoalText, false);
					}
					else
					{
						//Debug.Log("Else");
						_HUDmissionsController.ShowNewMissionGoalHUDnotification(LocalizedStepGoalText, true);
					}
				}
			}
		}
	}

	public void GoToNextStep(int goToNextStep)
	{
		if ((ActiveMission != null && CurrentStepIndex >= 0 && CurrentStepIndex < ActiveMission.MissionSteps.Length))
		{
			if (goToNextStep == -1)
			{
				_previousMissionStepIndex = -1;

				return;
			}
		}

		if (ActiveMission != null && CurrentStepIndex >= 0 && CurrentStepIndex < ActiveMission.MissionSteps.Length)
		{
			_previousMissionStepIndex = CurrentStepIndex;
		}
		CurrentStepIndex = goToNextStep;

		//Debug.Log("PREVIOUS");
		//Debug.Log(_previousMissionStepIndex);
		//Debug.Log("CURRENT");
		//Debug.Log(CurrentStepIndex);

		if (CurrentStepIndex >= ActiveMission.MissionSteps.Length)
		{
			StartNextMission();
			//return;
		}
		

		if (CurrentStepIndex < ActiveMission.MissionSteps.Length)
		{
			//ActiveMission.MissionSteps[CurrentStepIndex].OnMissionStepStartDoSomwthing();

			ActiveMission.MissionSteps[CurrentStepIndex].OnStepStarted();

			LocalizedStepGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);



			ShowMissionGoalHUD();
			_journalMenuSectionMissionInfoController.SetMissionStep(LocalizedStepGoalText);
		}
		else
		{
			_journalMenuSectionMissionInfoController.SetMissionStep("");
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

		if (ActiveMission.MissionSteps[CurrentStepIndex].Conditions[0] is IMissionStepConditionWithCountdown)
		{
			_missionStepConditionWithCountdown = ActiveMission.MissionSteps[CurrentStepIndex].Conditions[0] as IMissionStepConditionWithCountdown;

			_missionStepConditionWithCountdown.OnStepConditionTimeTick += HandleStepCountdown;
		}
		else if (_missionStepConditionWithCountdown != null)
		{
			_missionStepConditionWithCountdown.OnStepConditionTimeTick -= HandleStepCountdown;
		}
		//Debug.Log(CurrentStepIndex);
		//Debug.Log(ActiveMission.MissionSteps.Length);
		//Debug.Log(ActiveMission);
	}


	// PERSISTENCE OBJECTS UNSUBS BECAUSE ITS SCRIPTABLE OBJECT!!!!!!
	private void OnDestroy()
	{
		if (_missionStepConditionWithProgress != null)
		{
			_missionStepConditionWithProgress.OnStepConditionProgressUpdated -= HandleStepProgress;
		}

		if (_missionStepConditionWithCountdown != null)
		{
			_missionStepConditionWithCountdown.OnStepConditionTimeTick -= HandleStepCountdown;
		}
	}

	private void HandleStepProgress(int currentAmount, int requiredAmount)
	{
		LocalizedStepGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);
		_HUDmissionsController.ShowNewMissionGoalHUDnotification($"{LocalizedStepGoalText}: {currentAmount}/{requiredAmount}", false);
	}

	private void HandleStepCountdown(float remainingTime)
	{
		LocalizedStepGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);

		string formattedTime = remainingTime.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
		Debug.Log(_HUDmissionsController);
		_HUDmissionsController.ShowNewMissionGoalHUDnotification($"{LocalizedStepGoalText}: {formattedTime}", false);
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
			_journalMenuSectionMissionInfoController.SetMissionInfo(ActiveMission);
			_journalMenuSectionMissionInfoController.SetMissionInfo(ActiveMission);
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

	public void ApplyMissionConfig(Mission setMission, int setMissionStep)
	{
		ActiveMission = setMission;
		_journalMenuSectionMissionInfoController.SetMissionInfo(ActiveMission);
		GoToNextStep(setMissionStep);
	}

	private void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		if (ActiveMission != null && ActiveMission.MissionSteps.Length > 0)
		{
			if (SceneManager.sceneCount > 1 && (SceneManager.GetSceneAt(1).name != GameScenesSystemEnum.Scene_System_Test.ToString()))
			{
				LocalizedStepGoalText = GetLocalizedGoalText(ActiveMission.MissionSteps[CurrentStepIndex]);
				_journalMenuSectionMissionInfoController.SetMissionStep(LocalizedStepGoalText);
			}
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

			//Debug.Log(fileNameWithoutPrefix); // Выводим название без префикса
			//Debug.Log(targetString);

			if (fileNameWithoutPrefix == targetString)
			{
				ActiveMissionIndex = i;
				ActiveMission = _gameMissions.MissionsInOrder[ActiveMissionIndex];
				isFound = true;

				//Debug.Log(ActiveMission);
				break;
			}
		}

		CurrentStepIndex = data.MissionData.MissionStep;
		GoToNextStep(CurrentStepIndex);

		yield return null;
	}
}
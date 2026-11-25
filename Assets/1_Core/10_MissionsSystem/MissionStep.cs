using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionStep", menuName = "Missions/MissionStep")]
public class MissionStep : ScriptableObject
{
	public string MissionStepGoal_RU;
	public string MissionStepGoal_EN;
	private MissionsManager _missionsManager;
	public bool ShowMissionMarker => showMissionMarker;

	// 1. Структуры данных заменены на списки кортежей (ValueTuple)
	private List<(GameObject Object, bool IsEnabled)> _gameObjectTurnOn = new List<(GameObject, bool)>();
	private List<(GameObject Object, bool IsEnabled)> _NPCdialoguesEnable = new List<(GameObject, bool)>();

	public GameObject MissionStepGoalMarkerObject { get; private set; }

	[TextArea(3, 10)]
	public string StepDescription;
	[SerializeField] private bool showMissionMarker = true;

	public IReadOnlyList<MissionStepConditionAbstract> Conditions
	{
		get { return StepConditions.ConvertAll(c => (MissionStepConditionAbstract)c); }
	}

	//[SerializeField] private int _minStepIndexIgnore;
	//[SerializeField] private int _maxStepIndexIgnore;

	public List<MissionStepConditionAbstract> StepConditions = new List<MissionStepConditionAbstract>();

	public void ApplyAllCurrentGameplaySceneActions()
	{
		//Debug.Log("REAPPLY gameObjects" + _gameObjectTurnOn.Count);
		//Debug.Log("REAPPLY Dialogues" + _NPCdialoguesEnable.Count);

		if (_gameObjectTurnOn.Count > 0)
		{
			foreach (var tuple in _gameObjectTurnOn)
			{

				tuple.Object.SetActive(tuple.IsEnabled);
				// Логируем фактическое действие
				//Debug.Log($"SetActive для {tuple.Object.name}: {tuple.IsEnabled}");
				
			}
		}

		if (_NPCdialoguesEnable.Count > 0)
		{
			foreach (var tuple in _NPCdialoguesEnable)
			{
				if (tuple.Object != null)
				{
					var dialogueComponent = tuple.Object.GetComponent<IMissionStepActionNPCdialogueEnable>();
					if (dialogueComponent != null)
					{
						// Если нужно включить — вызываем Enable, если выключить — Disable
						if (tuple.IsEnabled)
						{
							dialogueComponent.EnableDialogue();
						}
						else
						{
							dialogueComponent.DisableDialogue();
						}
						//Debug.Log($"Dialogue для {tuple.Object.name}: {(tuple.IsEnabled ? "Enabled" : "Disabled")}");
					}
				}
			}
		}
	}

	public void OnStepStarted()
	{
		ApplyAllCurrentGameplaySceneActions();
	}

	public void OnStepCompleted(int goToNextStep)
	{
		//if (_missionsManager.CurrentStepIndex <= _maxStepIndexIgnore && _missionsManager.CurrentStepIndex >= _minStepIndexIgnore)
		//{
			_missionsManager.GoToNextStep(goToNextStep);
		//}
	}

	public void Initialize(MissionsManager missionsManager)
	{
		_missionsManager = missionsManager;
		ClearAllMissionStepActionsLists();

		foreach (var condition in StepConditions)
		{
			condition.Initialize(this);
		}
	}

	public void ClearAllMissionStepActionsLists()
	{
		if (_gameObjectTurnOn.Count > 0)
		{
			_gameObjectTurnOn.Clear();
		}
		if (_NPCdialoguesEnable.Count > 0)
		{
			_NPCdialoguesEnable.Clear();
		}
	}

	// 3. Методы регистрации теперь сохраняют значение bool
	public void RegisterObjectsTurnOn(GameObject objectTurnOn, bool enable)
	{
		_gameObjectTurnOn.Add((objectTurnOn, enable));

		//Debug.Log("REGISTER" + objectTurnOn);
	}

	public void RegisterNPCdialoguesEnable(GameObject NPCwithDialogue, bool enable)
	{
		_NPCdialoguesEnable.Add((NPCwithDialogue, enable));
	}

	public void RegisterMissionStepGoalMarkerObject(GameObject markerObject)
	{
		MissionStepGoalMarkerObject = markerObject;
	}
}
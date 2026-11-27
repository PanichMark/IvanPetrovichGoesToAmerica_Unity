using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionStep", menuName = "Missions/MissionStep")]
public class MissionStep : ScriptableObject
{
	public string MissionStepGoal_RU;
	public string MissionStepGoal_EN;
	private MissionsManager _missionsManager;
	public bool ShowMissionMarker => showMissionMarker;
	public bool IsCurrentStep => _missionsManager.ActiveMission.MissionSteps[_missionsManager.CurrentStepIndex] == this;

	private readonly List<MonoBehaviour> _registeredActions = new List<MonoBehaviour>();

	public GameObject MissionStepGoalMarkerObject { get; private set; }

	[TextArea(3, 10)]
	public string StepDescription;
	[SerializeField] private bool showMissionMarker = true;
	[SerializeField] private bool _canStepBeReturnedTo;

	public List<MissionStepConditionAbstract> Conditions => StepConditions;

	//[SerializeField] private int _minStepIndexIgnore;
	//[SerializeField] private int _maxStepIndexIgnore;

	[SerializeField] private List<MissionStepConditionAbstract> StepConditions = new List<MissionStepConditionAbstract>();

	private void ResetAllReturnableStepConditions()
	{
		for (int i = 0; i < StepConditions.Count; i++)
		{
			StepConditions[i].ResetStepConditionByReturning();
		}
	}

	public void ApplyAllCurrentGameplaySceneActions()
	{
		for (int i = 0; i < _registeredActions.Count; i++)
		{
			var actionComponent = _registeredActions[i];
			if (actionComponent != null && actionComponent is IMissionStepAction action)
			{
				action.PerformAction();
			}
		}
	}

	public void OnStepStarted()
	{
		ApplyAllCurrentGameplaySceneActions();

		if (_canStepBeReturnedTo)
		{
			ResetAllReturnableStepConditions();
		}
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
		_registeredActions.Clear();
	}

	public void RegisterAction(MonoBehaviour actionComponent)
	{
		if (actionComponent != null && actionComponent is IMissionStepAction && !_registeredActions.Contains(actionComponent))
		{
			_registeredActions.Add(actionComponent);
		}
	}

	public void RegisterMissionStepGoalMarkerObject(GameObject markerObject)
	{
		MissionStepGoalMarkerObject = markerObject;
	}
}
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionStepAbstract : ScriptableObject, IMissionStep
{
	public string MissionStepGoal_RU;
	public string MissionStepGoal_EN;
	private MissionsManager _missionsManager;
	public bool ShowMissionMarker => true;

	[SerializeField] private List<GameObject> GameObjectTurnOn;
	[SerializeField] private List<GameObject> GameObjectTurnOff;


	public IReadOnlyList<IMissionStepCondition> Conditions
	{
		get { return StepConditions.ConvertAll(c => (IMissionStepCondition)c); }
	}

	public List<MissionStepConditionAbstract> StepConditions = new List<MissionStepConditionAbstract>();

	public void OnStepCompleted(int goToNextStep)
	{
		_missionsManager.GoToNextStep(goToNextStep);
	}

	public void Initialize(MissionsManager missionsManager)
	{
		_missionsManager = missionsManager;

		foreach (var condition in StepConditions)
		{
			condition.Initialize(this);
		}
	}

	public void OnMissionStepStartDoSomwthing()
	{
		Debug.Log($"Do Somthing on Step {_missionsManager.CurrentStepIndex}");
	}
}
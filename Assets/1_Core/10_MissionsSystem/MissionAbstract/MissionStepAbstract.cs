using System.Collections.Generic;
using UnityEngine;

public abstract class MissionStepAbstract : ScriptableObject, IMissionStep
{
	public string MissionStepGoal_RU;
	public string MissionStepGoal_EN;
	private MissionsManager _missionsManager;
	public bool ShowMissionMarker => true;

	private List<GameObject> GameObjectTurnOn;
	private List<GameObject> GameObjectTurnOff;


	public IReadOnlyList<IMissionStepCondition> Conditions
	{
		get { return StepConditions.ConvertAll(c => (IMissionStepCondition)c); }
	}

	public List<MissionStepConditionAbstract> StepConditions = new List<MissionStepConditionAbstract>();

	public void OnStepStarted()
	{
		if (GameObjectTurnOn.Count > 0)
		{
			foreach (var obj in GameObjectTurnOn)
			{
				Debug.Log("TurnON");
				Debug.Log(obj.name);
				obj.SetActive(true);
			}
		}

		if (GameObjectTurnOff.Count > 0)
		{
			
			foreach (var obj in GameObjectTurnOff)
			{
				Debug.Log("TurnOFF");
				Debug.Log(obj.name);
				obj.SetActive(false);
			}
		}
	}

	public void OnStepCompleted(int goToNextStep)
	{
		_missionsManager.GoToNextStep(goToNextStep);
	}

	public void Initialize(MissionsManager missionsManager)
	{
		_missionsManager = missionsManager;

		GameObjectTurnOn.Clear();
		GameObjectTurnOff.Clear();

		foreach (var condition in StepConditions)
		{
			condition.Initialize(this);
		}
	}

	public void RegisterObjectsTurnOn(GameObject objectTurnOn)
	{
		GameObjectTurnOn.Add(objectTurnOn);
	}

	public void RegisterObjectsTurnOff(GameObject objectTurnOff)
	{
		GameObjectTurnOff.Add(objectTurnOff);
	}
}
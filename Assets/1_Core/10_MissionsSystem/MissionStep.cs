using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionStep", menuName = "Missions/MissionStep")]
public class MissionStep : ScriptableObject
{
	public string MissionStepGoal_RU;
	public string MissionStepGoal_EN;
	private MissionsManager _missionsManager;
	public bool ShowMissionMarker => true;

	private List<GameObject> GameObjectTurnOn;
	private List<GameObject> GameObjectTurnOff;

	[TextArea(3, 10)]
	public string StepDescription;
	[SerializeField] private bool showMissionMarker = true;

	public IReadOnlyList<MissionStepConditionAbstract> Conditions
	{
		get { return StepConditions.ConvertAll(c => (MissionStepConditionAbstract)c); }
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


		ClearTurnOnOffLists();

		foreach (var condition in StepConditions)
		{
			condition.Initialize(this);
		}
	}

	public void ClearTurnOnOffLists()
	{
		//Debug.Log(GameObjectTurnOn.Count);
		if (GameObjectTurnOn.Count > 0)
		{
			GameObjectTurnOn.Clear();
		}
		if (GameObjectTurnOff.Count > 0)
		{
			GameObjectTurnOff.Clear();
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
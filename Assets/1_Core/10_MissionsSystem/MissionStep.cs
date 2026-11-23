using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionStep", menuName = "Missions/MissionStep")]
public class MissionStep : ScriptableObject
{
	public string MissionStepGoal_RU;
	public string MissionStepGoal_EN;
	private MissionsManager _missionsManager;
	public bool ShowMissionMarker => showMissionMarker;

	private List<GameObject> _gameObjectTurnOn = new List<GameObject>();
	private List<GameObject> _gameObjectTurnOff = new List<GameObject>();

	private List<GameObject> _NPCdialoguesEnable = new List<GameObject>();
	private List<GameObject> _NPCdialoguesDisable = new List<GameObject>();

	public GameObject MissionStepGoalMarkerObject {  get; private set; }

	[TextArea(3, 10)]
	public string StepDescription;
	[SerializeField] private bool showMissionMarker = true;

	public IReadOnlyList<MissionStepConditionAbstract> Conditions
	{
		get { return StepConditions.ConvertAll(c => (MissionStepConditionAbstract)c); }
	}

	[SerializeField] private int _minStepIndexIgnore;
	[SerializeField] private int _maxStepIndexIgnore;

	public List<MissionStepConditionAbstract> StepConditions = new List<MissionStepConditionAbstract>();

	public void OnStepStarted()
	{
		if (_gameObjectTurnOn.Count > 0)
		{
			foreach (var obj in _gameObjectTurnOn)
			{
				Debug.Log("TurnON");
				Debug.Log(obj.name);
				obj.SetActive(true);
			}
		}

		if (_gameObjectTurnOff.Count > 0)
		{
			foreach (var obj in _gameObjectTurnOff)
			{
				Debug.Log("TurnOFF");
				Debug.Log(obj.name);
				obj.SetActive(false);
			}
		}

		if (_NPCdialoguesEnable.Count > 0)
		{
			foreach (var obj in _NPCdialoguesEnable)
			{
				obj.GetComponent<IMissionStepActionNPCdialogueAvailability>().EnableDialogue();
			}
		}

		if (_NPCdialoguesDisable.Count > 0)
		{
			foreach (var obj in _NPCdialoguesDisable)
			{
				obj.GetComponent<IMissionStepActionNPCdialogueAvailability>().DisableDialogue();
			}
		}
	}

	public void OnStepCompleted(int goToNextStep)
	{
		if (_missionsManager.CurrentStepIndex <= _maxStepIndexIgnore && _missionsManager.CurrentStepIndex >= _minStepIndexIgnore)
		{
			_missionsManager.GoToNextStep(goToNextStep);
		}
	
	}

	public void Initialize(MissionsManager missionsManager)
	{
		_missionsManager = missionsManager;


		ClearGameObjectsTurnOnOffLists();
		ClearNPCdialoguesAvailabilityLists();


		foreach (var condition in StepConditions)
		{
			condition.Initialize(this);
		}
	}

	public void ClearGameObjectsTurnOnOffLists()
	{

		//Debug.Log(GameObjectTurnOn.Count);
		if (_gameObjectTurnOn.Count > 0)
		{
			_gameObjectTurnOn.Clear();
		}
		if (_gameObjectTurnOff.Count > 0)
		{
			_gameObjectTurnOff.Clear();
		}
	}

	public void ClearNPCdialoguesAvailabilityLists()
	{

		//Debug.Log(GameObjectTurnOn.Count);
		if (_NPCdialoguesEnable.Count > 0)
		{
			_NPCdialoguesEnable.Clear();
		}
		if (_NPCdialoguesDisable.Count > 0)
		{
			_NPCdialoguesDisable.Clear();
		}
	}

	public void RegisterObjectsTurnOn(GameObject objectTurnOn)
	{
		_gameObjectTurnOn.Add(objectTurnOn);
	}

	public void RegisterObjectsTurnOff(GameObject objectTurnOff)
	{
		_gameObjectTurnOff.Add(objectTurnOff);
	}

	public void RegisterNPCdialoguesEnable(GameObject objectTurnOn)
	{
		_gameObjectTurnOn.Add(objectTurnOn);
	}

	public void RegisterNPCdialoguesDisable(GameObject objectTurnOff)
	{
		_gameObjectTurnOff.Add(objectTurnOff);
	}

	public void RegisterMissionStepGoalMarkerObject(GameObject markerObject)
	{
		MissionStepGoalMarkerObject = markerObject;
	}
}
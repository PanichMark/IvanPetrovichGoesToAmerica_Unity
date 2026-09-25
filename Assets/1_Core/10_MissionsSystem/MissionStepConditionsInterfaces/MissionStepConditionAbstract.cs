using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionStepConditionAbstract : ScriptableObject
{
	[SerializeField] protected int GoToNextStep;


	protected GameObject _stepConditionOwner;
	protected bool _isConditionMet;
	public GameObject StepConditionOwner => _stepConditionOwner;
	public bool IsConditionMet => _isConditionMet;

	protected MissionsManager _missionsManager;
	protected MissionStep _missionStepAbstract;
	public void RegisterOwner(GameObject owner)
	{
		_stepConditionOwner = owner;
		_missionsManager = ServiceLocator.Resolve<MissionsManager>();
	}

	public abstract void ResetStepCondition();

	public void Initialize(MissionStep missionStepAbstract)
	{
		_missionStepAbstract = missionStepAbstract;
	}
}
using UnityEngine;

public abstract class MissionStepConditionAbstract : ScriptableObject
{
	[SerializeField] protected int GoToNextStep;


	protected GameObject _stepConditionOwner;
	protected bool _isConditionMet;
	public bool IsConditionMet => _isConditionMet;

	protected MissionsManager _missionsManager;
	protected MissionStep _missionStepAbstract;

	public MissionStep MissionStepAbstract => _missionStepAbstract;
	public bool IsActive => _missionStepAbstract.IsCurrentStep;
	public void RegisterOwner(GameObject owner)
	{
		_stepConditionOwner = owner;
		_missionsManager = ServiceLocator.Resolve<MissionsManager>();
	}

	public abstract void ResetStepConditionBySystem();
	public abstract void ResetStepConditionByReturning();

	public void Initialize(MissionStep missionStepAbstract)
	{
		_missionStepAbstract = missionStepAbstract;
	}
}
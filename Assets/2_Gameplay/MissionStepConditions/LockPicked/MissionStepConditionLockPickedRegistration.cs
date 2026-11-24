using UnityEngine;

public class MissionStepConditionLockPickedRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	// Ссылка на конкретный замок в инспекторе
	[SerializeField] private InteractionObjectLockAbstract _lockObject;

	private void Start()
	{

			_lockObject = GetComponent<InteractionObjectLockAbstract>();
		

		

		_lockObject.OnUnlockLock += HandleLockUnlocked;
	}

	private void HandleLockUnlocked()
	{
		if (_linkedMissionCondition is MissionStepConditionLockPicked lockCondition)
		{
			lockCondition.CompleteCondition();
		}
	}

	private void OnDestroy()
	{
		
			_lockObject.OnUnlockLock -= HandleLockUnlocked;
		
	}
}
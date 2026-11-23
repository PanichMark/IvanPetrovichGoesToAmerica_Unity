using UnityEngine;

public class MissionStepConditionLockPickedRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	// Ссылка на конкретный замок в инспекторе
	[SerializeField] private InteractionObjectLockAbstract _lockObject;

	private void Start()
	{
		// Если ссылка не задана в инспекторе, пытаемся найти на этом же объекте
		if (_lockObject == null)
		{
			_lockObject = GetComponent<InteractionObjectLockAbstract>();
		}

		if (_lockObject == null) return;

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
		if (_lockObject != null)
		{
			_lockObject.OnUnlockLock -= HandleLockUnlocked;
		}
	}
}
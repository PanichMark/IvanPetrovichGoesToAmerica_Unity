using UnityEngine;

[CreateAssetMenu(fileName = "STepConditionOnDestroyed", menuName = "Missions/MissionStepConditions/StepConditionOnDestroyed")]
public class MissionStepConditionOnDestroyed : MissionStepConditionAbstract
{
	public GameObject TargetDamageableObject;
	private bool _isCompleted = false;

	public override bool IsConditionMet()
	{
		return _isCompleted;
	}

	private void OnEnable()
	{
		_missionsManager.OnAnyObjectDestroyed += HandleObjectDestroyed;
	}

	private void OnDisable()
	{
		_missionsManager.OnAnyObjectDestroyed -= HandleObjectDestroyed;
	}

	private void HandleObjectDestroyed(GameObject destroyedObject, bool wasLethal)
	{
		if (destroyedObject == TargetDamageableObject && !_isCompleted)
		{
			_isCompleted = true;
			FindObjectOfType<MissionsManager>().CheckAndCompleteCurrentStep();
		}
	}
}
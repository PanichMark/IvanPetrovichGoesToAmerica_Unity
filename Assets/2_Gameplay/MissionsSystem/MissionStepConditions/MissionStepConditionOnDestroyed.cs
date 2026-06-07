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
		MissionsManager.OnAnyObjectDestroyed += HandleObjectDestroyed;
	}

	private void OnDisable()
	{
		MissionsManager.OnAnyObjectDestroyed -= HandleObjectDestroyed;
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
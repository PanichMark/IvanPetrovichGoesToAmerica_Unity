using UnityEngine;

[CreateAssetMenu(fileName = "MissionStepConditionOnDestroyed", menuName = "Missions/MissionConditions/MissionStepConditionOnDestroyed")]
public class MissionStepConditionOnDestroyed : MissionStepConditionAbstract
{
	public GameObject targetDamageableObject;
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
		if (destroyedObject == targetDamageableObject && !_isCompleted)
		{
			_isCompleted = true;
			FindObjectOfType<MissionsManager>().CheckAndCompleteCurrentStep();
		}
	}
}
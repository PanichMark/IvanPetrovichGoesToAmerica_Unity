using UnityEngine;

[CreateAssetMenu(fileName = "MissionStepConditionOnInteraction", menuName = "Missions/MissionConditions/MissionStepConditionOnInteraction")]
public class MissionStepConditionOnInteraction : MissionStepConditionAbstract
{
	public GameObject targetInteractableObject;
	private bool _isCompleted = false;

	public override bool IsConditionMet()
	{
		return _isCompleted;
	}

	private void OnEnable()
	{
		MissionsManager.OnAnyObjectInteracted += HandleObjectInteracted;
	}

	private void OnDisable()
	{
		MissionsManager.OnAnyObjectInteracted -= HandleObjectInteracted;
	}

	private void HandleObjectInteracted(GameObject interactedObject)
	{
		if (interactedObject == targetInteractableObject && !_isCompleted)
		{
			_isCompleted = true;
			FindObjectOfType<MissionsManager>().CheckAndCompleteCurrentStep();
		}
	}
}
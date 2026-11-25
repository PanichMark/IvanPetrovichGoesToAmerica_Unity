using UnityEngine;

public class MissionStepConditionPickableObjectReleasedRegistration : MonoBehaviour
{
	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	private InteractionObjectPickableAbstract _pickableObject;

	private void Start()
	{
		_pickableObject = GetComponent<InteractionObjectPickableAbstract>();
		if (_pickableObject == null) return;

		_pickableObject.OnPickableObjectReleased += HandleObjectDropped;
	}

	private void HandleObjectDropped()
	{
		if (_linkedMissionCondition is MissionStepConditionPickableObjectReleased droppedCondition)
		{
			droppedCondition.CompleteCondition();
		}
	}

	private void OnDestroy()
	{
		if (_pickableObject != null)
		{
			_pickableObject.OnPickableObjectReleased -= HandleObjectDropped;
		}
	}
}
using UnityEngine;

public class MissionStepObjectRegistrationInteractable : MonoBehaviour
{
	private IInteractable _interactable;

	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	private void Start()
	{
		_linkedMissionCondition.RegisterOwner(gameObject);
		Debug.Log($"{gameObject.name} зарегистрировал себя в условии {_linkedMissionCondition.name}");

		_interactable = GetComponent<IInteractable>();
		_interactable.OnInteract += TriggerInteraction;
	}

	public void TriggerInteraction()
	{
		if (_linkedMissionCondition is MissionStepConditionInteractable interactionCondition)
		{
			interactionCondition.OnPlayerInteracted(gameObject);
		}
	}
}
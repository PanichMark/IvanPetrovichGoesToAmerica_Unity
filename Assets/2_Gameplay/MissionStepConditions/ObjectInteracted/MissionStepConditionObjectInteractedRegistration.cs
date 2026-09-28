using UnityEngine;

public class MissionStepConditionObjectInteractedRegistration : MonoBehaviour
{
	private IInteractable _interactable;

	[SerializeField] private MissionStepConditionAbstract _linkedMissionCondition;

	private void Start()
	{
		_linkedMissionCondition.RegisterOwner(gameObject);
		//Debug.Log($"{gameObject.name} зарегистрировал себя в условии {_linkedMissionCondition.name}");

		_interactable = GetComponent<IInteractable>();
		_interactable.OnInteract += TriggerInteraction;
	}

	public void TriggerInteraction()
	{
		//Debug.Log("INTERACTED!!!!!");

		if (_linkedMissionCondition is MissionStepConditionObjectInteracted interactionCondition)
		{
			interactionCondition.OnPlayerInteracted(gameObject);
		}
	}
}
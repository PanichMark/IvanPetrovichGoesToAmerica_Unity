using UnityEngine;

public class MissionStepInteractionObject : MonoBehaviour
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
		if (_linkedMissionCondition is MissionStepConditionOnInteraction interactionCondition)
		{
			interactionCondition.OnPlayerInteracted(gameObject);
		}
	}
}
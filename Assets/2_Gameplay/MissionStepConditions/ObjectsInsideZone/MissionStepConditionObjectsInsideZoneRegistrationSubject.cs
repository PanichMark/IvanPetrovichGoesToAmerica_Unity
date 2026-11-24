using UnityEngine;

public class MissionStepConditionObjectsInsideZoneRegistrationSubject : MonoBehaviour
{
	[SerializeField] private MissionStepConditionObjectsInsideZoneRegistrationOwner _owner;

	private void Start()
	{
		_owner.RegisterTrackedSubject(gameObject);
		
	}
}
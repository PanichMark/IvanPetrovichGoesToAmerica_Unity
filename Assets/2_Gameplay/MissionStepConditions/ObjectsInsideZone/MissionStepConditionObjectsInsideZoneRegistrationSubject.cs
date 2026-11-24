using UnityEngine;

public class MissionStepConditionObjectsInsideZoneRegistrationSubject : MonoBehaviour
{
	[SerializeField] private MissionStepConditionObjectsInsideZoneRegistrationOwner _owner;
	[SerializeField] private bool _isSubjectPlayer;
	private GameObject _playerCollider;

	private void Start()
	{
		// Проверка 1: Присоединен ли вообще ScriptableObject в инспекторе
		if (_isSubjectPlayer)
		{
			_playerCollider = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.PlayerCollider);
			_owner.RegisterTrackedSubject(_playerCollider);
		}
		else
		{
			_owner.RegisterTrackedSubject(gameObject);
		}
	}
}
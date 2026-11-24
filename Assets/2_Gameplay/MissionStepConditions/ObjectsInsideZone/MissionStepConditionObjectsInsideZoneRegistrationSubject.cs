using UnityEngine;

public class MissionStepConditionObjectsInsideZoneRegistrationSubject : MonoBehaviour
{
	[SerializeField] private MissionStepConditionObjectsInsideZoneRegistrationOwner _owner;
	[SerializeField] private bool _isSubjectPlayer;
	private GameObject _player;

	private void Start()
	{
		// Проверка 1: Присоединен ли вообще ScriptableObject в инспекторе
		if (_isSubjectPlayer)
		{
			_player = ServiceLocator.Resolve(ServiceLocatorPlayerGameObjectsEnum.Player);
			_owner.RegisterTrackedSubject(_player);
		}
		else
		{
			_owner.RegisterTrackedSubject(gameObject);
		}
	}
}
using UnityEngine;

public class HintMessageController : MonoBehaviour
{
	[SerializeField] private InteractionObjectNote _noteObject;
	private Collider _triggerZone;

	private GameObject _playerCollider;

	private void Awake()
	{

		_playerCollider = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCollider");

		_triggerZone = GetComponent<Collider>();

		_triggerZone.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == _playerCollider)
		{
			Debug.Log("SHOW HINT!");

			_noteObject.Interact();

			_triggerZone.enabled = false;
		}
	}
}
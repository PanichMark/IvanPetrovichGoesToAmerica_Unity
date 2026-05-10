using UnityEngine;

public class HintMessageController : MonoBehaviour
{
	[SerializeField] private GameObject _noteObject;
	private Collider _triggerZone;

	private InteractionObjectNote _interactionNote;
	private GameObject _playerCollider;

	private void Awake()
	{
		_interactionNote = _noteObject.GetComponent<InteractionObjectNote>();

		_playerCollider = ServiceLocator.Resolve<GameObject>("PlayerColliderGameObject");

		_triggerZone = GetComponent<Collider>();

		_triggerZone.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == _playerCollider)
		{
			_interactionNote.Interact();

			_triggerZone.enabled = false;
		}
	}
}
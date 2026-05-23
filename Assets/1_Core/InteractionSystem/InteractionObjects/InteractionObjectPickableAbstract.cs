using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectPickableAbstract : MonoBehaviour, IInteractable, ISaveLoad, IPickable
{
	protected LocalizationManager _localizationManager;

	[SerializeField] protected string _interactionObjectNameSystem;

	protected Collider _playerCollider;
	protected bool _isCollisionIgnored = false;
	protected bool _isPlayerInsideTrigger = false;

	protected GameObject _playerColliderGameObject;
	protected int _pickableLayer;
	protected int _playerLayer;

	public GameObject CachedPlayer { get; protected set; }
	public Collider Collider { get; protected set; }
	public Rigidbody RigidBody { get; protected set; }

	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI { get; protected set; }

	public string InteractionHintMessageAction { get; protected set; }
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";

	public virtual string InteractionHintMessageFail => null;
	public virtual bool IsInteractionHintMessageFailActive => false;

	public bool IsObjectPickedUp { get; protected set; }

	void Start()
	{
		_pickableLayer = LayerMask.NameToLayer("Pickable");
		_playerLayer = LayerMask.NameToLayer("Player");
		_playerColliderGameObject = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCollider");
		_playerCollider = _playerColliderGameObject.GetComponent<Collider>();

		Collider = GetComponent<Collider>();
		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUDInteraction_HintAction_Pickable");
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == CachedPlayer)
			_isPlayerInsideTrigger = true;
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == CachedPlayer)
		{
			_isPlayerInsideTrigger = false;
			if (_isCollisionIgnored && _playerCollider != null)
			{
				Physics.IgnoreCollision(Collider, _playerCollider, false);
				_isCollisionIgnored = false;
			}
		}
	}

	public void ChangeLanguage()
	{
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUDInteraction_HintAction_Pickable");
	}

	public void Interact()
	{
		PickUpObject();
	}

	public virtual void PickUpObject()
	{
		if (!IsObjectPickedUp)
		{
			if (CachedPlayer != null)
			{
				Debug.Log($"Picked up {InteractionObjectNameSystem}");
				gameObject.tag = "Untagged";
				Collider.enabled = false;
				RigidBody.isKinematic = true;

				StartCoroutine(MoveTowardsInFrontOfPlayer());

				transform.parent = CachedPlayer.transform;
				transform.rotation = Quaternion.Euler(0, CachedPlayer.transform.localEulerAngles.y + 180, 0);
				IsObjectPickedUp = true;
			}
			else
			{
				Debug.Log("Player not found!");
			}
		}
	}

	public virtual void DropOffObject()
	{
		Debug.Log($"Dropped off {InteractionObjectNameSystem}");
		gameObject.tag = "Interactable";
		Collider.enabled = true;
		RigidBody.isKinematic = false;
		IsObjectPickedUp = false;

		transform.parent = null;

		Physics.IgnoreCollision(Collider, _playerCollider, true);
		_isCollisionIgnored = true;

		StartCoroutine(EnableCollisionAfterDelay(0.05f));

		SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex(1));
	}

	private IEnumerator EnableCollisionAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);

		if (Collider != null && _playerCollider != null)
		{
			Physics.IgnoreCollision(Collider, _playerCollider, false);
			_isCollisionIgnored = false;
		}
	}

	IEnumerator MoveTowardsInFrontOfPlayer()
	{
		while (true)
		{
			Vector3 targetPosition = CachedPlayer.transform.position + CachedPlayer.transform.forward * 0.5f + Vector3.up * 1f;
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
				break;

			yield return null;
		}

		transform.position = CachedPlayer.transform.position + CachedPlayer.transform.forward * 0.5f + Vector3.up * 1f;
	}

	public void SaveData(ref GameData data)
	{

	}

	public void LoadData(GameData data)
	{

	}
}
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectPickableAbstract : MonoBehaviour, IInteractable, ISaveLoad, IPickable
{
	protected LocalizationManager localizationManager;

	protected Collider playerCollider;
	protected bool isCollisionIgnored = false;
	protected bool isPlayerInsideTrigger = false;

	protected GameObject playerColliderGameObject;
	protected int pickableLayer;
	protected int playerLayer;
	public GameObject CachedPlayer { get; protected set; }
	public Collider Collider { get; protected set; }
	public Rigidbody RigidBody { get; protected set; }

	[SerializeField] protected string interactionObjectNameSystem;
	public virtual string InteractionObjectNameSystem => interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI { get; protected set; }
	public string InteractionHintMessageMain => $"{InteractionHintAction} {InteractionObjectNameUI}?";

	public string InteractionHintAction { get; protected set; }

	public virtual string InteractionHintMessageAdditional => null;
	public virtual bool IsInteractionHintMessageAdditionalActive => false;

	public bool IsObjectPickedUp { get; protected set; }

	void Start()
	{
		pickableLayer = LayerMask.NameToLayer("Pickable");
		playerLayer = LayerMask.NameToLayer("Player");
		playerColliderGameObject = ServiceLocator.Resolve<GameObject>("PlayerColliderGameObject");
		playerCollider = playerColliderGameObject.GetComponent<Collider>();

		Collider = GetComponent<Collider>();
		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("PlayerGameObject");

		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Pickable");
		localizationManager.OnLanguageChangeEvent += ChangeLanguage;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == CachedPlayer)
			isPlayerInsideTrigger = true;
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject == CachedPlayer)
		{
			isPlayerInsideTrigger = false;
			if (isCollisionIgnored && playerCollider != null)
			{
				Physics.IgnoreCollision(Collider, playerCollider, false);
				isCollisionIgnored = false;
			}
		}
	}

	public void ChangeLanguage()
	{
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Pickable");
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

		Physics.IgnoreCollision(Collider, playerCollider, true);
		isCollisionIgnored = true;

		StartCoroutine(EnableCollisionAfterDelay(0.05f));

		SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex(1));
	}

	private IEnumerator EnableCollisionAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);

		if (Collider != null && playerCollider != null)
		{
			Physics.IgnoreCollision(Collider, playerCollider, false);
			isCollisionIgnored = false;
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
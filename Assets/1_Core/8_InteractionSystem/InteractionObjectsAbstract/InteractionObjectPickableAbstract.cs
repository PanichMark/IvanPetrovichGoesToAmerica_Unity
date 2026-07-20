using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectPickableAbstract : MonoBehaviour, IInteractable, ISaveLoad, IPickable
{
	protected LocalizationManager _localizationManager;

	[SerializeField] protected string _interactionObjectNameSystem;

	protected Collider _playerCollider;
	protected bool _isCollisionIgnored = false;
	protected bool _isPlayerInsideTrigger = false;
	private Transform _pickableObjectTransform;
	protected GameObject _playerColliderGameObject;
	protected int _pickableLayer;
	protected int _playerLayer;
	public int PickableObjectIndex { get; protected set; }
	public event IInteractable.InteractableObjectHandler OnInteract;

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
		_pickableObjectTransform = GetComponent<Transform>();
		_pickableLayer = LayerMask.NameToLayer("Pickable");
		_playerLayer = LayerMask.NameToLayer("Player");
		_playerColliderGameObject = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCollider");
		_playerCollider = _playerColliderGameObject.GetComponent<Collider>();

		Collider = GetComponent<Collider>();
		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Pickup");
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	public void AssignPickableObjectsIndexes(int index)
	{
		PickableObjectIndex = index;
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

	public void ChangeLanguage(LocalizationManager localizationManager)
	{
		_localizationManager = localizationManager;

		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Pickup");
	}

	public void Interact()
	{
		PickUpObject();
	}

	public void InteractCutscene()
	{
		gameObject.tag = "Untagged";
		Collider.enabled = false;
		RigidBody.isKinematic = true;
		transform.parent = CachedPlayer.transform;
		transform.rotation = Quaternion.Euler(0, CachedPlayer.transform.localEulerAngles.y + 180, 0);
		IsObjectPickedUp = true;
	}

	public virtual void PickUpObject()
	{
		if (!IsObjectPickedUp)
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
	}

	public virtual void DropOffObject()
	{
		Debug.Log($"Dropped off {InteractionObjectNameSystem}");
		gameObject.tag = "Interactable";
		Collider.enabled = true;
		RigidBody.isKinematic = false;
		IsObjectPickedUp = false;

		transform.parent = null;
		transform.localPosition += transform.forward * 0.3f;
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

	public virtual void SaveData(ref GameData data)
	{
		List<PickableObjectData> targetList = null;

		if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_0_Test))
		{
			targetList = data.PickableObjects_Scene_0_Test;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Church))
		{
			targetList = data.PickableObjects_Scene_1_Church;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_Street))
		{
			targetList = data.PickableObjects_Scene_1_Street;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_RevenueHouse))
		{
			targetList = data.PickableObjects_Scene_1_RevenueHouse;
		}
		else if (SceneManager.GetSceneAt(1).name == nameof(GameScenesEnum.Scene_1_InnerYard))
		{
			targetList = data.PickableObjects_Scene_1_InnerYard;
		}

		if (targetList != null)
		{
			int indexInList = targetList.FindIndex(item => item.PickableObjectIndex == PickableObjectIndex);

			if (indexInList != -1)
			{
				PickableObjectData updatedItem = new PickableObjectData
				{
					PickableObjectIndex = PickableObjectIndex,
					PickableObjectNameSystem = InteractionObjectNameSystem,
					PickableObjecPosition = _pickableObjectTransform.position,
					PickableObjecRotation = _pickableObjectTransform.rotation,
					WasPickableObjectPickedUp = IsObjectPickedUp
				};

				targetList[indexInList] = updatedItem;
			}
			else
			{
				targetList.Add(new PickableObjectData
				{
					PickableObjectIndex = PickableObjectIndex,
					PickableObjectNameSystem = InteractionObjectNameSystem,
					PickableObjecPosition = _pickableObjectTransform.position,
					PickableObjecRotation = _pickableObjectTransform.rotation,
					WasPickableObjectPickedUp = IsObjectPickedUp
				});
			}
		}
	}

	public virtual void LoadData(GameData data)
	{

	}
}
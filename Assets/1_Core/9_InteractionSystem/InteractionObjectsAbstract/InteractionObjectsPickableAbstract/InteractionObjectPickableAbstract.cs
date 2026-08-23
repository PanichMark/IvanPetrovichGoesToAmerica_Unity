using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectPickableAbstract : MonoBehaviour, IInteractable, ISaveLoad, IPickable, IBreakable
{
	protected LocalizationManager _localizationManager;
	[Header("Object Info")]
	[SerializeField] protected string _interactionObjectNameSystem;

	[SerializeField] protected InteractionObjectPickableData _interactionObjectPickableType;

	public InteractionObjectsPickableTypes PickableType => _interactionObjectPickableType.PickableType;

	[Header("Object Health")]
	[SerializeField] protected float _health;
	[SerializeField] protected bool _canBeBroken;
	[SerializeField] private float _breakingThreshold;

	protected PlayerInteractionController _playerInteractionController;
	protected GameController _gameController;
	protected Collider _playerCollider;
	protected bool _isCollisionIgnored = false;
	protected bool _isPlayerInsideTrigger = false;
	
	protected GameObject _playerColliderGameObject;
	protected int _pickableLayer;
	protected int _playerLayer;
	public int PickableObjectIndex { get; protected set; }
	public event IInteractable.InteractableObjectHandler OnInteract;
	protected bool _isCreatedAsBody;
	public GameObject CachedPlayer { get; protected set; }
	public Collider Collider { get; protected set; }
	public Rigidbody RigidBody { get; protected set; }
	private GameObject _gameObjectSpineSlot;
	public virtual string InteractionObjectNameSystem => _interactionObjectNameSystem;
	public virtual string InteractionObjectNameUI { get; protected set; }

	public string InteractionHintMessageAction { get; protected set; }
	public string InteractionHintMessageMain => $"{InteractionHintMessageAction} {InteractionObjectNameUI}?";

	public virtual string InteractionHintMessageFail => null;
	public virtual bool IsInteractionHintMessageFailActive => false;

	public bool IsObjectPickedUp { get; protected set; }

	public bool CanObjectBeBroken => _canBeBroken;

	public bool IsObjectDestroyed => _isObjectDestroyed;

	protected bool _isObjectDestroyed;
	public float CurrentDurability => _health;

	public float DuribilityThreshold => _breakingThreshold;

	protected virtual void InitializePickable()
	{

	}

	void Awake()
	{
		//_pickableObjectTransform = GetComponent<Transform>();
		_pickableLayer = LayerMask.NameToLayer("Pickable");
		_playerLayer = LayerMask.NameToLayer("Player");
		_playerColliderGameObject = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCollider");
		_playerCollider = _playerColliderGameObject.GetComponent<Collider>();
		_gameController = ServiceLocator.Resolve<GameController>("GameController");
		_playerInteractionController = ServiceLocator.Resolve<PlayerInteractionController>("InteractionController");
		Collider = GetComponent<BoxCollider>();

		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		_gameObjectSpineSlot = ServiceLocator.Resolve<GameObject>("GameObjectSpineSlot");

		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");

		if (!_isCreatedAsBody && _interactionObjectNameSystem != null)
		{
			InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		}

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Pickup");
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		InitializePickable();
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
		PickUpObject(false);
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

	public virtual void PickUpObject(bool isPickedUpByLoadSafeFile)
	{
		if (!IsObjectPickedUp)
		{
			Debug.Log($"Picked up {InteractionObjectNameSystem}");
			gameObject.tag = "Untagged";
			Collider.enabled = false;
			RigidBody.isKinematic = true;

			if (!isPickedUpByLoadSafeFile)
			{
				StartCoroutine(MoveTowardsPlayer());
			}
			else
			{
				SetPickableObjectTransformAtPlayerArms();
			}

			//transform.rotation = Quaternion.Euler(0, CachedPlayer.transform.localEulerAngles.y + 180, 0);
			IsObjectPickedUp = true;
		}
	}

	public virtual void DropOffObject()
	{
		StopAllCoroutines();
		Debug.Log($"Dropped off {InteractionObjectNameSystem}");
		gameObject.tag = "Interactable";
		gameObject.layer = LayerMask.NameToLayer("Default");
		Collider.enabled = true;
		RigidBody.isKinematic = false;
		IsObjectPickedUp = false;

		transform.parent = null;
		transform.localPosition += transform.forward * 0.3f;
		Physics.IgnoreCollision(Collider, _playerCollider, true);
		_isCollisionIgnored = true;

		StartCoroutine(EnableCollisionAfterDelay(0.05f));

		SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneAt(1));
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

	private IEnumerator MoveTowardsPlayer()
	{
		while (true)
		{
			Vector3 targetPosition = CachedPlayer.transform.TransformPoint(_interactionObjectPickableType.Position);

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			Quaternion targetRotation = Quaternion.LookRotation(CachedPlayer.transform.forward, Vector3.up) * _interactionObjectPickableType.Rotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
				break;

			yield return null;
		}

		SetPickableObjectTransformAtPlayerArms();
	}

	protected virtual void SetPickableObjectTransformAtPlayerArms()
	{
		transform.parent = _gameObjectSpineSlot.transform;
		transform.position = CachedPlayer.transform.TransformPoint(_interactionObjectPickableType.Position);
		transform.rotation = Quaternion.LookRotation(CachedPlayer.transform.forward, Vector3.up) * _interactionObjectPickableType.Rotation;
	}

	public virtual void TakeBreakDamage(float amount)
	{
		if (CanObjectBeBroken)
		{
			if (amount >= DuribilityThreshold)
			{
				_health -= amount;

				if (_health <= 0)
				{
					ObjectIsFullyBroken();
				}
			}
		}
	}

	public void ObjectIsFullyBroken()
	{
		_isObjectDestroyed = true;

		Destroy(gameObject);
	}

	public IEnumerator SaveData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.PickableObjectsData == null)
		{
			data.PickableObjectsData = new Dictionary<GameScenesGameplayDataEnum, List<PickableObjectData>>();
		}
		if (!data.PickableObjectsData.ContainsKey(currentScene))
		{
			data.PickableObjectsData[currentScene] = new List<PickableObjectData>();
		}

		var targetList = data.PickableObjectsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.PickableObjectIndex == PickableObjectIndex);

		var updatedItem = new PickableObjectData
		{
			PickableObjectIndex = PickableObjectIndex,
			PickableObjectNameSystem = InteractionObjectNameSystem,
			PickableObjectPosition = new Vector3(
				Mathf.Round(gameObject.transform.position.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.position.z * 100f) / 100f),
			PickableObjectRotation = new Quaternion(
				Mathf.Round(gameObject.transform.rotation.x * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.y * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.z * 100f) / 100f,
				Mathf.Round(gameObject.transform.rotation.w * 100f) / 100f),
			IsPickableObjectPickedUp = IsObjectPickedUp,
			IsPickableObjectDestroyed = _isObjectDestroyed
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public virtual IEnumerator LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.PickableObjectsData == null || !data.PickableObjectsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.PickableObjectIndex == PickableObjectIndex);

		if (savedState.Equals(default(PickableObjectData))) yield break;

		IsObjectPickedUp = savedState.IsPickableObjectPickedUp;

		if (IsObjectPickedUp)
		{
			IsObjectPickedUp = false;

			PickUpObject(true);
			_playerInteractionController.PickUpObjectOnLoadData(gameObject);
		}
		else
		{
			gameObject.transform.position = savedState.PickableObjectPosition;
			gameObject.transform.rotation = savedState.PickableObjectRotation;
		}

		yield return null;
	}
}
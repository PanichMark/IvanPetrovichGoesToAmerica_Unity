using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class InteractionObjectPickableAbstract : GameplayObjectJsonSaveLoad, IInteractable, IPickable
{
	protected LocalizationManager _localizationManager;
	[Header("Object Info")]
	[SerializeField] protected string _interactionObjectNameSystem;

	[SerializeField] protected InteractionObjectPickableData _interactionObjectPickableType;

	public InteractionObjectsPickableTypes PickableType => _interactionObjectPickableType.PickableType;

	protected PlayerInteractionController _playerInteractionController;
	protected GameController _gameController;
	protected Collider _playerCollider;
	protected bool _isCollisionIgnored = false;
	protected bool _isPlayerInsideTrigger = false;
	
	protected GameObject _playerColliderGameObject;
	protected int _pickableLayer;
	protected int _playerLayer;
	public event IInteractable.InteractableObjectHandler OnInteract;
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


	public bool IsObjectDestroyed => _isObjectDestroyed;

	protected bool _isObjectDestroyed;

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

		if (_interactionObjectNameSystem != null)
		{
			InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		}

		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Pickup");
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		InitializePickable();
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

	public virtual void PickUpObject(bool isPickedUpByLoadSafeFile)
	{
		if (!IsObjectPickedUp)
		{
			Debug.Log($"Picked up {InteractionObjectNameSystem}");
			gameObject.tag = "Untagged";

			// Рекурсивно отключаем все коллайдеры в иерархии
			var allColliders = GetComponentsInChildren<Collider>(true);
			foreach (var col in allColliders)
			{
				col.enabled = false;
			}

			// Рекурсивно переводим все Rigidbody в кинематический режим
			var allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
			foreach (var rb in allRigidbodies)
			{
				rb.isKinematic = true;
			}

			if (!isPickedUpByLoadSafeFile)
			{
				StartCoroutine(MoveTowardsPlayer());
			}
			else
			{
				SetPickableObjectTransformAtPlayerArms();
			}

			IsObjectPickedUp = true;
		}
	}

	public virtual void DropOffObject()
	{
		StopAllCoroutines();
		Debug.Log($"Dropped off {InteractionObjectNameSystem}");
		gameObject.tag = "Interactable";
		gameObject.layer = LayerMask.NameToLayer("Default");

		// Рекурсивно включаем все коллайдеры обратно
		var allColliders = GetComponentsInChildren<Collider>(true);
		foreach (var col in allColliders)
		{
			col.enabled = true;
		}

		// Рекурсивно возвращаем всем телам динамическую физику
		var allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
		foreach (var rb in allRigidbodies)
		{
			rb.isKinematic = false;
		}

		IsObjectPickedUp = false;

		transform.parent = null;
		transform.localPosition += transform.forward * 0.3f;

		// Внимание: Physics.IgnoreCollision работает только для конкретной ПАРЫ компонентов.
		// Если у вас много детей-коллайдеров, игнорирование придется настраивать для каждого из них отдельно,
		// либо использовать другой подход (например, слои Collision Matrix).
		if (_playerCollider != null)
		{
			foreach (var col in allColliders)
			{
				Physics.IgnoreCollision(col, _playerCollider, true);
			}
			_isCollisionIgnored = true;
		}

		StartCoroutine(EnableCollisionAfterDelay(0.05f));

		SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneAt(1));
	}

	// Этот метод также изменен согласно вашему запросу "везде"
	public void InteractCutscene()
	{
		gameObject.tag = "Untagged";

		var allColliders = GetComponentsInChildren<Collider>(true);
		foreach (var col in allColliders)
		{
			col.enabled = false;
		}

		var allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
		foreach (var rb in allRigidbodies)
		{
			rb.isKinematic = true;
		}

		transform.parent = CachedPlayer.transform;
		transform.rotation = Quaternion.Euler(0, CachedPlayer.transform.localEulerAngles.y + 180, 0);
		IsObjectPickedUp = true;
	}

	private IEnumerator EnableCollisionAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);

		if (_playerCollider == null)
		{
			_isCollisionIgnored = false;
			yield break;
		}

		// Получаем все коллайдеры (включая детей), которые были отключены при подборе
		var allColliders = GetComponentsInChildren<Collider>(true);

		foreach (var col in allColliders)
		{
			// Включаем столкновение для каждого из них с коллайдером игрока
			Physics.IgnoreCollision(col, _playerCollider, false);
		}

		_isCollisionIgnored = false;
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
}
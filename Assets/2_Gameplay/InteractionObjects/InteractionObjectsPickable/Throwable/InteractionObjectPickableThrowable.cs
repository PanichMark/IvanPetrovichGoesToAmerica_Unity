using System.Collections;
using UnityEngine;

public class InteractionObjectPickableThrowable : InteractionObjectPickableAbstract, IThrowable, IDamageable, IBreakable
{
	private bool _wasObjectDestroyed;
	private bool _canObjectBeDestroyedOnImpact;
	public bool IsObjectDestroyed => _wasObjectDestroyed;
	public float ObjectThrowPower => 10f;
	private GameObject _firstPersonRightHandWeaponSlotGameObject;

	[Header("Object Health")]
	[SerializeField, Min(0)] private float _health;
	[SerializeField] private bool _canBeDamaged;
	[SerializeField] private bool _canBeBroken;
	[SerializeField] private float _breakingThreshold;

	[Header("Object Damage")]
	[SerializeField] private float _damage;
	[SerializeField] private bool _canDamageBreakable;



	private GameObject _thirdPersonRightHandWeaponSlotGameObject;
	public float CurrentHealth => _health;


	public float CurrentDurability => _health;

	public float DuribilityThreshold => _breakingThreshold;

	public bool CanObjectBeDamaged => _canBeDamaged;

	public bool CanObjectBeBroken => _canBeBroken;

	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private void Start()
	{
		_pickableLayer = LayerMask.NameToLayer("Pickable");
		_playerLayer = LayerMask.NameToLayer("Player");
		_playerColliderGameObject = ServiceLocator.Resolve<GameObject>("GameObjectPlayerCollider");
		_playerCollider = _playerColliderGameObject.GetComponent<Collider>();
		_firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonRightHandWeaponSlotGameObject");
		_thirdPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonRightHandWeaponSlotGameObject");
		Collider = GetComponent<Collider>();
		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("GameObjectPlayer");
		_localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		_playerCameraStateMachineController = ServiceLocator.Resolve<PlayerCameraStateMachineController>("PlayerCameraStateMachineController");

		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Pickup");
		_localizationManager.OnLanguageChanged += ChangeLanguage;

		_playerCameraStateMachineController.OnFirstPersonCameraState += ChangeThrowableCameraStateFirst;
		_playerCameraStateMachineController.OnThirdPersonCameraState += ChangeThrowableCameraStateThird;
	}

	private void ChangeThrowableCameraStateFirst()
	{
		if (IsObjectPickedUp)
		{
			gameObject.layer = LayerMask.NameToLayer("FirstPerson");

			StopAllCoroutines();

			transform.parent = _firstPersonRightHandWeaponSlotGameObject.transform;
			transform.position = _firstPersonRightHandWeaponSlotGameObject.transform.position;
			transform.rotation = Quaternion.Euler(0, _firstPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
		}
	}

	private void ChangeThrowableCameraStateThird()
	{
		if (IsObjectPickedUp)
		{
			gameObject.layer = LayerMask.NameToLayer("Default");

			StopAllCoroutines();

			transform.parent = _thirdPersonRightHandWeaponSlotGameObject.transform;
			transform.position = _thirdPersonRightHandWeaponSlotGameObject.transform.position;
			transform.rotation = Quaternion.Euler(0, _thirdPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
		}
	}

	public override void PickUpObject()
	{
		if (!IsObjectPickedUp)
		{
			Debug.Log($"Picked up {InteractionObjectNameSystem}");

			gameObject.tag = "Untagged";
			Collider.enabled = false;
			RigidBody.isKinematic = true;

			if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
			{
				StartCoroutine(MoveTowardsRightHandFirstPerson());
			}
			else
			{
				StartCoroutine(MoveTowardsRightHandThirdPerson());
			}

			IsObjectPickedUp = true;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_canObjectBeDestroyedOnImpact)
		{
			var damageable = collision.gameObject.GetComponent<IDamageable>();
			if (damageable != null && damageable.CanObjectBeDamaged)
			{
				damageable.TakeDamage(_damage);
			}

			if (_canDamageBreakable)
			{
				var breakable = collision.gameObject.GetComponent<IBreakable>();
				if (breakable != null && breakable.CanObjectBeBroken)
				{
					breakable.TakeDamage(_damage);
				}
			}

			RigidBody.isKinematic = true;

			_wasObjectDestroyed = true;
			Destroy(gameObject);
			Debug.Log($"{InteractionObjectNameSystem} was destroyed on impact!");
		}
	}

	public void ThrowObject()
	{
		Debug.Log($"Throwed {InteractionObjectNameSystem}");

		Physics.IgnoreCollision(Collider, _playerCollider, true);
		_isCollisionIgnored = true;
		Collider.enabled = true;
		RigidBody.isKinematic = false;
		IsObjectPickedUp = false;

		_canObjectBeDestroyedOnImpact = true;

		transform.parent = null;

		Vector3 throwDirection = CachedPlayer.transform.forward - Camera.main.transform.up * Mathf.Tan(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);
		throwDirection.Normalize();

		RigidBody.AddForce(throwDirection * ObjectThrowPower, ForceMode.Impulse);

		gameObject.layer = LayerMask.NameToLayer("Default");
	}

	public void TakeDamage(float amount)
	{
		if (_canBeDamaged)
		{
			Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {CurrentHealth - amount}");

			_health -= amount;

			if (_health <= 0)
			{
				ObjectIsFullyDamaged();
			}
		}
		if (_canBeBroken)
		{
			if (amount >= DuribilityThreshold)
			{
				_health -= amount;

				if (_health <= 0)
				{
					ObjectIsFullyDamaged();
				}
			}
		}
	}

	public void ObjectIsFullyDamaged()
	{
		Debug.Log($"{InteractionObjectNameSystem} was destroyed!");

		_wasObjectDestroyed = true;

		Destroy(gameObject);
	}
	
	private IEnumerator MoveTowardsRightHandFirstPerson()
	{
		gameObject.layer = LayerMask.NameToLayer("FirstPerson");

		while (true)
		{
			Vector3 targetPosition = _firstPersonRightHandWeaponSlotGameObject.transform.position;

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				break;
			}

			yield return null;
		}

		transform.parent = _firstPersonRightHandWeaponSlotGameObject.transform;
		transform.position = _firstPersonRightHandWeaponSlotGameObject.transform.position;
		transform.rotation = Quaternion.Euler(0, _firstPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
	}
	

	private IEnumerator MoveTowardsRightHandThirdPerson()
	{
		while (true)
		{
			Vector3 targetPosition = _thirdPersonRightHandWeaponSlotGameObject.transform.position;

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				break;
			}

			yield return null;
		}

		transform.parent = _thirdPersonRightHandWeaponSlotGameObject.transform;
		transform.position = _thirdPersonRightHandWeaponSlotGameObject.transform.position;
		transform.rotation = Quaternion.Euler(0, _thirdPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
	}

	public void ObjectIsFullyBroken()
	{
		Debug.Log($"{InteractionObjectNameSystem} was destroyed!");

		_wasObjectDestroyed = true;

		Destroy(gameObject);
	}

	/*
	public override void SaveData(ref GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		// Инициализируем словарь или список для текущей сцены, если их нет
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
			IsPickableObjectPickedUp = IsObjectPickedUp,
			IsPickableObjectDestroyed = _wasObjectDestroyed
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}
	}

	public override void LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) return;

		if (data.PickableObjectsData == null || !data.PickableObjectsData.TryGetValue(currentScene, out var sourceList)) return;

		var savedState = sourceList.Find(item => item.PickableObjectIndex == PickableObjectIndex);

		if (savedState.Equals(default(PickableObjectData))) return;

		IsObjectPickedUp = savedState.IsPickableObjectPickedUp;
		_wasObjectDestroyed = savedState.IsPickableObjectDestroyed;

		if (_wasObjectDestroyed || IsObjectPickedUp)
		{
			gameObject.SetActive(false);
		}
	}
	*/
}
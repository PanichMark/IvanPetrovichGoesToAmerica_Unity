using System.Collections;
using UnityEngine;

public abstract class InteractionObjectPickableThrowableAbstract : InteractionObjectPickableAbstract, IThrowable
{
	private bool _canObjectBeDestroyedOnImpact;
	public float ObjectThrowPower => 10f;
	private GameObject _firstPersonRightHandWeaponSlotGameObject;

	[SerializeField] private bool IsDestroyedUponImpact;

	[Header("Object Damage")]
	[SerializeField] private float _damage;
	[SerializeField] private bool _canDamageBreakable;

	private GameObject _thirdPersonRightHandWeaponSlotGameObject;



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

	public override void PickUpObject(bool isPickedUpByLoadSafeFile)
	{
		if (!IsObjectPickedUp)
		{
			Debug.Log($"Picked up {InteractionObjectNameSystem}");

			gameObject.tag = "Untagged";
			Collider.enabled = false;
			RigidBody.isKinematic = true;

			if (!isPickedUpByLoadSafeFile)
			{
				if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
				{
					StartCoroutine(MoveTowardsRightHandFirstPerson());
				}
				else
				{
					StartCoroutine(MoveTowardsRightHandThirdPerson());
				}
			}
			else
			{
				SetPickableObjectTransformAtPlayerArms();
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
				if (breakable != null && breakable.CanObjectBeBroken && !breakable.IsObjectDestroyed)
				{
					breakable.TakeBreakDamage(_damage);
				}
			}

			RigidBody.isKinematic = true;

			_isObjectDestroyed = true;
			gameObject.SetActive(false);
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


	public void ObjectIsFullyDamaged()
	{
		Debug.Log($"{InteractionObjectNameSystem} was destroyed!");

		_isObjectDestroyed = true;

		gameObject.SetActive(false);
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

		SetPickableObjectTransformAtPlayerArms();
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

		SetPickableObjectTransformAtPlayerArms();
	}

	protected override void SetPickableObjectTransformAtPlayerArms()
	{
		if (_playerCameraStateMachineController.CurrentPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			transform.parent = _firstPersonRightHandWeaponSlotGameObject.transform;
			transform.position = _firstPersonRightHandWeaponSlotGameObject.transform.position;
			transform.rotation = Quaternion.Euler(0, _firstPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
		}
		else
		{
			transform.parent = _thirdPersonRightHandWeaponSlotGameObject.transform;
			transform.position = _thirdPersonRightHandWeaponSlotGameObject.transform.position;
			transform.rotation = Quaternion.Euler(0, _thirdPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
		}
	}


}
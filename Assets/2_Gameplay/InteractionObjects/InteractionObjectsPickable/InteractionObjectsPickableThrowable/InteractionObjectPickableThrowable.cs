using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InteractionObjectPickableThrowable : InteractionObjectPickableAbstract, IThrowable, IDamageable
{
	private bool _wasObjectDestroyed;
	private bool _canObjectBeDestroyedOnImpact;
	public bool WasObjectDestroyed => _wasObjectDestroyed;
	public float ObjectThrowPower => 10f;

	private bool isItFirstPerson;
	private GameObject firstPersonRightHandWeaponSlotGameObject;

	[SerializeField, Min(0)] private float _health;

	private GameObject thirdPersonRightHandWeaponSlotGameObject;
	public float Health
	{
		get => _health;
		set
		{
			_health = value;
			if (_health <= 0)
			{
				ObjectIsFullyDamaged();
			}
		}
	}

	private PlayerCameraController playerCameraController;

	private void OnDestroy()
	{
		playerCameraController.OnFirstPersonCameraState -= () =>
		{
			ThrowableObjectToFirstPerson();
			isItFirstPerson = true;
		};
		playerCameraController.OnThirdPersonCameraState -= () =>
		{
			ThrowableObjectToThirdPerson();
			isItFirstPerson = false;
		};
	}

	private void Start()
	{
		pickableLayer = LayerMask.NameToLayer("Pickable");
		playerLayer = LayerMask.NameToLayer("Player");
		playerColliderGameObject = ServiceLocator.Resolve<GameObject>("playerColliderGameObject");
		playerCollider = playerColliderGameObject.GetComponent<Collider>();
		firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("firstPersonRightHandWeaponSlotGameObject");
		thirdPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("thirdPersonRightHandWeaponSlotGameObject");
		Collider = GetComponent<Collider>();
		RigidBody = GetComponent<Rigidbody>();
		CachedPlayer = ServiceLocator.Resolve<GameObject>("Player");
		localizationManager = ServiceLocator.Resolve<LocalizationManager>("LocalizationManager");
		playerCameraController = ServiceLocator.Resolve<PlayerCameraController>("PlayerCameraController");

		playerCameraController.OnFirstPersonCameraState += () =>
		{
			ThrowableObjectToFirstPerson();
			isItFirstPerson = true;
		};
		playerCameraController.OnThirdPersonCameraState += () =>
		{
			ThrowableObjectToThirdPerson();
			isItFirstPerson = false;
		};

		InteractionObjectNameUI = localizationManager.GetLocalizedString(interactionObjectNameSystem);
		InteractionHintAction = localizationManager.GetLocalizedString("HUDInteraction_HintAction_Pickable");
		localizationManager.OnLanguageChangeEvent += ChangeLanguage;
	}

	private void ThrowableObjectToFirstPerson()
	{
		if (IsObjectPickedUp)
		{
			transform.parent = firstPersonRightHandWeaponSlotGameObject.transform;
			transform.position = firstPersonRightHandWeaponSlotGameObject.transform.position;
			transform.rotation = Quaternion.Euler(0, firstPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
		}
	}

	private void ThrowableObjectToThirdPerson()
	{
		if (IsObjectPickedUp)
		{
			transform.parent = thirdPersonRightHandWeaponSlotGameObject.transform;
			transform.position = thirdPersonRightHandWeaponSlotGameObject.transform.position;
			transform.rotation = Quaternion.Euler(0, thirdPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
		}
	}

	public override void PickUpObject()
	{
		if (!IsObjectPickedUp)
		{
			if (firstPersonRightHandWeaponSlotGameObject != null)
			{
				Debug.Log($"Picked up {InteractionObjectNameSystem}");

				gameObject.tag = "Untagged";
				Collider.enabled = false;
				RigidBody.isKinematic = true;

				if (isItFirstPerson)
				{
					StartCoroutine(MoveTowardsRightHandFirstPerson());
				}
				else
				{
					StartCoroutine(MoveTowardsRightHandThirdPerson());
				}

				IsObjectPickedUp = true;
			}
			else
			{
				Debug.Log("Player not found!");
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_canObjectBeDestroyedOnImpact)
		{
			RigidBody.isKinematic = true;

			_wasObjectDestroyed = true;
			Destroy(gameObject);
			Debug.Log($"{InteractionObjectNameSystem} was destroyed on impact!");
		}
	}

	public void ThrowObject()
	{
		Debug.Log($"Throwed {InteractionObjectNameSystem}");

		Physics.IgnoreCollision(Collider, playerCollider, true);
		isCollisionIgnored = true;
		Collider.enabled = true;
		RigidBody.isKinematic = false;
		IsObjectPickedUp = false;

		_canObjectBeDestroyedOnImpact = true;

		transform.parent = null;

		Vector3 throwDirection = CachedPlayer.transform.forward - Camera.main.transform.up * Mathf.Tan(Camera.main.transform.eulerAngles.x * Mathf.Deg2Rad);
		throwDirection.Normalize();

		RigidBody.AddForce(throwDirection * ObjectThrowPower, ForceMode.Impulse);
	}

	public void TakeDamage(float amount)
	{
		Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {Health - amount}");

		Health -= amount;
	}

	public void ObjectIsFullyDamaged()
	{
		Debug.Log($"{InteractionObjectNameSystem} was destroyed!");

		_wasObjectDestroyed = true;

		Destroy(gameObject);
	}

	private IEnumerator MoveTowardsRightHandFirstPerson()
	{
		while (true)
		{
			Vector3 targetPosition = firstPersonRightHandWeaponSlotGameObject.transform.position;

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				break;
			}

			yield return null;
		}

		transform.parent = firstPersonRightHandWeaponSlotGameObject.transform;
		transform.position = firstPersonRightHandWeaponSlotGameObject.transform.position;
		transform.rotation = Quaternion.Euler(0, firstPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
	}

	private IEnumerator MoveTowardsRightHandThirdPerson()
	{
		while (true)
		{
			Vector3 targetPosition = thirdPersonRightHandWeaponSlotGameObject.transform.position;

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, 5f * Time.deltaTime);

			if ((transform.position - targetPosition).sqrMagnitude < 0.001f)
			{
				break;
			}

			yield return null;
		}

		transform.parent = thirdPersonRightHandWeaponSlotGameObject.transform;
		transform.position = thirdPersonRightHandWeaponSlotGameObject.transform.position;
		transform.rotation = Quaternion.Euler(0, thirdPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
	}
}
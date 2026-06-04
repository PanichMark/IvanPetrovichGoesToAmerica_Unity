using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionObjectPickableThrowable : InteractionObjectPickableAbstract, IThrowable, IDamageable
{
	private bool _wasObjectDestroyed;
	private bool _canObjectBeDestroyedOnImpact;
	public bool WasObjectDestroyed => _wasObjectDestroyed;
	public float ObjectThrowPower => 10f;

	private bool _isItFirstPerson;
	private GameObject _firstPersonRightHandWeaponSlotGameObject;

	[SerializeField, Min(0)] private float _health;

	private GameObject _thirdPersonRightHandWeaponSlotGameObject;
	public float CurrentHealth
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

	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private void OnDestroy()
	{
		_playerCameraStateMachineController.OnFirstPersonCameraState -= () =>
		{
			ThrowableObjectToFirstPerson();
			_isItFirstPerson = true;
		};
		_playerCameraStateMachineController.OnThirdPersonCameraState -= () =>
		{
			ThrowableObjectToThirdPerson();
			_isItFirstPerson = false;
		};
	}

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

		_playerCameraStateMachineController.OnFirstPersonCameraState += () =>
		{
			ThrowableObjectToFirstPerson();
			_isItFirstPerson = true;
		};
		_playerCameraStateMachineController.OnThirdPersonCameraState += () =>
		{
			ThrowableObjectToThirdPerson();
			_isItFirstPerson = false;
		};

		InteractionObjectNameUI = _localizationManager.GetLocalizedString(_interactionObjectNameSystem);
		InteractionHintMessageAction = _localizationManager.GetLocalizedString("HUD_Interaction_HintMessage_Action_Pickup");
		_localizationManager.OnLanguageChanged += ChangeLanguage;
	}

	private void ThrowableObjectToFirstPerson()
	{
		if (IsObjectPickedUp)
		{
			transform.parent = _firstPersonRightHandWeaponSlotGameObject.transform;
			transform.position = _firstPersonRightHandWeaponSlotGameObject.transform.position;
			transform.rotation = Quaternion.Euler(0, _firstPersonRightHandWeaponSlotGameObject.transform.localEulerAngles.y, 0);
		}
	}

	private void ThrowableObjectToThirdPerson()
	{
		if (IsObjectPickedUp)
		{
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

			if (_isItFirstPerson)
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
	}

	public void TakeDamage(float amount)
	{
		Debug.Log($"{InteractionObjectNameSystem} was damaged by {amount}, current health {CurrentHealth - amount}");

		CurrentHealth -= amount;
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

	public override void SaveData(ref GameData data)
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
					WasPickableObjectPickedUp = IsObjectPickedUp,
					WasPickableObjectDestroyed = _wasObjectDestroyed
				};

				targetList[indexInList] = updatedItem;
			}
			else
			{
				targetList.Add(new PickableObjectData
				{
					PickableObjectIndex = PickableObjectIndex,
					PickableObjectNameSystem = InteractionObjectNameSystem,
					WasPickableObjectPickedUp = IsObjectPickedUp,
					WasPickableObjectDestroyed = _wasObjectDestroyed
				});
			}
		}
	}

	public override void LoadData(GameData data)
	{

	}
}
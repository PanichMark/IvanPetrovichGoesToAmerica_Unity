using System.Collections;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
	public abstract WeaponNames WeaponName { get; }
	public string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public abstract WeaponTypes WeaponType { get; }

	[SerializeField] protected Sprite _weaponIcon;

	public Sprite WeaponIcon => _weaponIcon;
	[SerializeField] protected AudioClip _weaponSoundAttack;
	public abstract float WeaponDamage { get; }
	public abstract bool IsWeaponAuto { get; }
	public abstract float WeaponAttackSpeedRate { get; }
	protected bool _isWeaponPlayerAutoAttacking;
	protected Coroutine _currentWeaponPlayerAutoAttackCourutine;
	protected bool _isWeaponInitialized;
	protected bool _isThisPlayerWeapon;
	protected PlayerWeaponController _playerWeaponController;
	protected PlayerWeaponAnimationController _playerWeaponAnimationController;
	protected AudioSource _weaponAudioSource;


	public WeaponHandsEnum WeaponHandType { get; private set; }

	public GameObject FirstPersonWeaponModelInstance { get; protected set; }
	public GameObject ThirdPersonWeaponModelInstance { get; protected set; }

	protected GameObject _firstPersonLeftHandWeaponSlotGameObject;
	protected Transform _firstPersonLeftHandWeaponSlotTransform;

	protected GameObject _firstPersonRightHandWeaponSlotGameObject;
	protected Transform _firstPersonRightHandWeaponSlotTransform;

	protected GameObject _thirdPersonLeftHandWeaponSlotGameObject;
	protected Transform _thirdPersonLeftHandWeaponSlotTransform;

	protected GameObject _thirdPersonRightHandWeaponSlotGameObject;
	protected Transform _thirdPersonRightHandWeaponSlotTransform;

	public abstract void WeaponAttack();
	public abstract void StartAutoAttackingWeaponPlayer();
	public abstract void StopAutoAttacking();
	public abstract IEnumerator AutoAttackWeaponPlayerCourutine();

	public void InstantiateWeaponPlayer(PlayerWeaponController playerWeaponController, WeaponHandsEnum handType)
	{
		_playerWeaponController = playerWeaponController;

		_isThisPlayerWeapon = true;
		WeaponHandType = handType;

		_firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonRightHandWeaponSlotGameObject");
		_firstPersonRightHandWeaponSlotTransform = _firstPersonRightHandWeaponSlotGameObject.transform;
		_thirdPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonRightHandWeaponSlotGameObject");
		_thirdPersonRightHandWeaponSlotTransform = _thirdPersonRightHandWeaponSlotGameObject.transform;
		
		_firstPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonLeftHandWeaponSlotGameObject");
		_firstPersonLeftHandWeaponSlotTransform = _firstPersonLeftHandWeaponSlotGameObject.transform;
		_thirdPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonLeftHandWeaponSlotGameObject");
		_thirdPersonLeftHandWeaponSlotTransform = _thirdPersonLeftHandWeaponSlotGameObject.transform;
		
		ThirdPersonWeaponModelInstance = gameObject;
		InstantiateFirstPersonWeaponInstance();

		FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");

		SetLayerRecursively(FirstPersonWeaponModelInstance.transform, LayerMask.NameToLayer("FirstPerson"));

		if (WeaponHandType == WeaponHandsEnum.Left)
		{
			FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonLeftHandWeaponSlotTransform, true);
			ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonLeftHandWeaponSlotTransform, true);
		}
		else if (WeaponHandType == WeaponHandsEnum.Right)
		{
			FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonRightHandWeaponSlotTransform, true);
			ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonRightHandWeaponSlotTransform, true);
		}

		FirstPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

		if (WeaponHandType == WeaponHandsEnum.Right)
		{
			_weaponAudioSource = ServiceLocator.Resolve<AudioSource>("PlayerAudioWeaponRight");
		}
		else
		{
			_weaponAudioSource = ServiceLocator.Resolve<AudioSource>("PlayerAudioWeaponLeft");
		}

		_playerWeaponAnimationController = ServiceLocator.Resolve<PlayerWeaponAnimationController>("WeaponAnimationController");

		InitializeWeapon();

		_isWeaponInitialized = true;
	}

	private void SetLayerRecursively(Transform parent, int layer)
	{
		foreach (Transform child in parent)
		{
			child.gameObject.layer = layer;
			SetLayerRecursively(child, layer);
		}
	}

	public void InstantiateFirstPersonWeaponInstance()
	{
		FirstPersonWeaponModelInstance = Instantiate(gameObject);
		WeaponAbstract FirstPersonWeaponModelInstanceComponent = FirstPersonWeaponModelInstance.GetComponent<WeaponAbstract>();
		Destroy(FirstPersonWeaponModelInstanceComponent);
		//FirstPersonWeaponModelInstanceComponent.Make1stPersonWeaponModelOwnerPlayer();
	}

	public void Make1stPersonWeaponModelOwnerPlayer()
	{
		_isThisPlayerWeapon = true;
	}

	public abstract void InitializeWeapon();

	public void InstantiateWeaponNPC(Transform NPCweaponSlotTransform)
	{
		_isThisPlayerWeapon = false;

		ThirdPersonWeaponModelInstance = gameObject;

		_thirdPersonRightHandWeaponSlotTransform = NPCweaponSlotTransform;
		ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonRightHandWeaponSlotTransform, true);

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;
	}

	public void DestroyWeaponModel()
	{
		if (ThirdPersonWeaponModelInstance != null)
		{
			Destroy(ThirdPersonWeaponModelInstance);
			ThirdPersonWeaponModelInstance = null;
		}
		if (FirstPersonWeaponModelInstance != null)
		{
			Destroy(FirstPersonWeaponModelInstance);
			FirstPersonWeaponModelInstance = null;
		}
	}

	public void MirrorWeaponPlayerModel()
	{
		if (FirstPersonWeaponModelInstance != null)
		{
			Vector3 fpScale = FirstPersonWeaponModelInstance.transform.localScale;
			fpScale.x *= -1;
			FirstPersonWeaponModelInstance.transform.localScale = fpScale;
		}

		if (ThirdPersonWeaponModelInstance != null)
		{
			Vector3 tpScale = ThirdPersonWeaponModelInstance.transform.localScale;
			tpScale.x *= -1;
			ThirdPersonWeaponModelInstance.transform.localScale = tpScale;
		}
	}
}
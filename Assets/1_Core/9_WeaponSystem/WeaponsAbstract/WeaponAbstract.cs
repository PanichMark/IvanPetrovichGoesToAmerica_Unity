using System.Collections;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
	public abstract PlayerWeaponNames WeaponName { get; }
	public string WeaponNameSystem => $"Weapon_{WeaponType}_{WeaponName}";
	public abstract WeaponTypes WeaponType { get; }
	[SerializeField] protected int _weaponDamage;
	[SerializeField] protected Sprite _weaponIconBig;
	[SerializeField] protected Sprite _weaponIconSmall;
	public abstract float TimeBetweenAbilityToAttack { get; }
	public Sprite WeaponIconBig => _weaponIconBig;
	public Sprite WeaponIconSmall => _weaponIconSmall;
	[SerializeField] protected AudioClip _weaponSoundAttack;

	protected int _layersToDamage;

	protected int _layersOrganisms;

	protected int _layersHeads;
	
	protected int _layerNPC;

	public abstract bool IsWeaponAuto { get; }
	public abstract float WeaponAttackSpeedRate { get; }
	public bool IsWeaponPlayerAutoAttacking { get; protected set; }
	protected Coroutine _currentWeaponPlayerAutoAttackCourutine;
	protected bool _isWeaponInitialized;
	protected PlayerWeaponController _playerWeaponController;
	protected PlayerWeaponAnimationController _playerWeaponAnimationController;
	protected AudioSource _weaponAudioSource;
	protected bool _isAttacking;

	public WeaponHandType WeaponHandType { get; private set; }

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

	public abstract void WeaponPlayerAttack();
	public abstract void StartAutoAttackingWeaponPlayer();
	public abstract void StopAutoAttackingWeaponPlayer();

	public virtual void WeaponNPCattack()
	{

	}

	public virtual void OnHideWeaponPlayer()
	{

	}

	public abstract IEnumerator AutoAttackWeaponPlayerCourutine();

	public abstract IEnumerator InspectWeaponAnimation();

	public void InstantiateWeaponPlayer(PlayerWeaponController playerWeaponController, WeaponHandType handType)
	{
		_layersToDamage = LayerMask.GetMask("Default", "Outline", "HitboxBody_Organism", "HitboxBody_Robot", "HitboxHead_Organism", "HitboxHead_Robot");
		_layersOrganisms = LayerMask.GetMask("HitboxBody_Organism", "HitboxHead_Organism");
		_layersHeads = LayerMask.GetMask("HitboxHead_Organism", "HitboxHead_Robot");
		_layerNPC = LayerMask.GetMask("NPC");

		_playerWeaponController = playerWeaponController;

		WeaponHandType = handType;

		_firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.WeaponSlotFirstPersonRightHand);
		_firstPersonRightHandWeaponSlotTransform = _firstPersonRightHandWeaponSlotGameObject.transform;
		_thirdPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.WeaponSlotThirdPersonRightHand);
		_thirdPersonRightHandWeaponSlotTransform = _thirdPersonRightHandWeaponSlotGameObject.transform;

		_firstPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.WeaponSlotFirstPersonLeftHand);
		_firstPersonLeftHandWeaponSlotTransform = _firstPersonLeftHandWeaponSlotGameObject.transform;
		_thirdPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.WeaponSlotThirdPersonLeftHand);
		_thirdPersonLeftHandWeaponSlotTransform = _thirdPersonLeftHandWeaponSlotGameObject.transform;
		
		ThirdPersonWeaponModelInstance = gameObject;
		InstantiateFirstPersonWeaponInstance();

		FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");

		SetLayerRecursively(FirstPersonWeaponModelInstance.transform, LayerMask.NameToLayer("FirstPerson"));

		if (WeaponHandType == WeaponHandType.Left)
		{
			FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonLeftHandWeaponSlotTransform, true);
			ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonLeftHandWeaponSlotTransform, true);
		}
		else if (WeaponHandType == WeaponHandType.Right)
		{
			FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonRightHandWeaponSlotTransform, true);
			ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonRightHandWeaponSlotTransform, true);
		}

		FirstPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

		if (WeaponHandType == WeaponHandType.Right)
		{
			_weaponAudioSource = ServiceLocator.Resolve(ServiceLocatorAudioSourcesEnum.PlayerAudioWeaponRight);
		}
		else
		{
			_weaponAudioSource = ServiceLocator.Resolve(ServiceLocatorAudioSourcesEnum.PlayerAudioWeaponLeft);
		}

		_playerWeaponAnimationController = ServiceLocator.Resolve<PlayerWeaponAnimationController>();

		InitializeWeaponPlayer();

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

	public abstract void InitializeWeaponPlayer();

	public virtual void InitializeWeaponNPC(Transform NPCweaponAttackPoint)
	{

	}

	public void InstantiateWeaponNPC(Transform NPCweaponSlotTransform, Transform NPCweaponAttackPoint)
	{
		_layersToDamage = LayerMask.GetMask("Player", "Default", "Outline", "HitboxBody_Organism", "HitboxBody_Robot", "HitboxHead_Organism", "HitboxHead_Robot");
		_layersOrganisms = LayerMask.GetMask("HitboxBody_Organism", "HitboxHead_Organism");
		_layersHeads = LayerMask.GetMask("HitboxHead_Organism", "HitboxHead_Robot");
		_layerNPC = LayerMask.GetMask("NPC");

		//_thirdPersonRightHandWeaponSlotTransform = NPCweaponSlotTransform;
		gameObject.transform.SetParent(NPCweaponSlotTransform, false);

		//ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;

		if (this is WeaponRangedAbstract)
		{
			gameObject.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
		}
		else
		{
			gameObject.transform.localRotation = Quaternion.identity;
		}

		InitializeWeaponNPC(NPCweaponAttackPoint);
	}

	public void DestroyWeaponPlayerModels()
	{
		Destroy(ThirdPersonWeaponModelInstance);
		ThirdPersonWeaponModelInstance = null;
		
		Destroy(FirstPersonWeaponModelInstance);
		FirstPersonWeaponModelInstance = null;
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

	public void SetUpWeaponInspect()
	{
		_firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.WeaponSlotFirstPersonRightHand);
		_firstPersonRightHandWeaponSlotTransform = _firstPersonRightHandWeaponSlotGameObject.transform;

		_firstPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve(ServiceLocatorGameObjectsEnum.WeaponSlotFirstPersonLeftHand);
		_firstPersonLeftHandWeaponSlotTransform = _firstPersonLeftHandWeaponSlotGameObject.transform;

		//Destroy(ThirdPersonWeaponModelInstance);

		_playerWeaponAnimationController = ServiceLocator.Resolve<PlayerWeaponAnimationController>();

		//FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");

		SetLayerRecursively(gameObject.transform, LayerMask.NameToLayer("FirstPerson"));

		// Пытаемся преобразовать текущий объект в WeaponEugenicAbstract
		WeaponEugenicAbstract eugenicWeapon = this as WeaponEugenicAbstract;

		if (eugenicWeapon == null)
		{
			//FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonRightHandWeaponSlotTransform, true);
		}
		else
		{
			// Вызываем метод у полученного объекта
			eugenicWeapon.ShowOnlyEugenicWeaponBottle();
		}

		//Destroy(gameObject);
	}

	public virtual void DetroyInspectedWeapon()
	{
	}
}
using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
	public abstract string WeaponName { get; }
	public abstract string WeaponNameSystem { get; }
	public abstract string WeaponType { get; }
	public abstract Sprite WeaponIcon { get; }
	public abstract float WeaponDamage { get; }
	public abstract bool IsWeaponAuto { get; }
	protected float _weaponAutoAttackSpeedRate;
	protected bool _isWeaponAutoAttacking;
	protected Coroutine _weaponAutoAttackCourutine;
	protected bool _isWeaponInitialized;
	protected bool _isThisPlayerWeapon;

	public GameObject FirstPersonWeaponModelInstance { get; protected set; }
	public GameObject ThirdPersonWeaponModelInstance { get; protected set; }

	protected GameObject _firstPersonLeftHandWeaponSlotGameObject;
	protected Transform _firstPersonLeftHandWeaponSlotTransform;

	protected GameObject _firstPersonRightHandWeaponSlotGameObject;
	protected Transform _firstPersonRightHandWeaponSlotTransform;

	private GameObject _thirdPersonLeftHandWeaponSlotGameObject;
	private Transform _thirdPersonLeftHandWeaponSlotTransform;

	private GameObject _thirdPersonRightHandWeaponSlotGameObject;
	private Transform _thirdPersonRightHandWeaponSlotTransform;

	public abstract void WeaponAttack();
	public abstract void StartAutoAttacking();
	public abstract void StopAutoAttacking();
	public abstract IEnumerator AutoAttackCourutine();

	public void InstantiateWeapon(WeaponHandsEnum handType)
	{
		_isThisPlayerWeapon = true;

		if (handType == WeaponHandsEnum.HandRight)
		{
			_firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonRightHandWeaponSlotGameObject");
			_firstPersonRightHandWeaponSlotTransform = _firstPersonRightHandWeaponSlotGameObject.transform;

			_thirdPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonRightHandWeaponSlotGameObject");
			_thirdPersonRightHandWeaponSlotTransform = _thirdPersonRightHandWeaponSlotGameObject.transform;
		}
		else if (handType == WeaponHandsEnum.HandLeft)
		{
			_firstPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonLeftHandWeaponSlotGameObject");
			_firstPersonLeftHandWeaponSlotTransform = _firstPersonLeftHandWeaponSlotGameObject.transform;

			_thirdPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonLeftHandWeaponSlotGameObject");
			_thirdPersonLeftHandWeaponSlotTransform = _thirdPersonLeftHandWeaponSlotGameObject.transform;
		}

		ThirdPersonWeaponModelInstance = gameObject;
		InstantiateFirstPersonWeaponInstance();

		FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");

		SetLayerRecursively(FirstPersonWeaponModelInstance.transform, LayerMask.NameToLayer("FirstPerson"));

		if (handType == WeaponHandsEnum.HandLeft)
		{
			FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonLeftHandWeaponSlotTransform, true);
			ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonLeftHandWeaponSlotTransform, true);
		}
		else if (handType == WeaponHandsEnum.HandRight)
		{
			FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonRightHandWeaponSlotTransform, true);
			ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonRightHandWeaponSlotTransform, true);
		}

		FirstPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

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
		FirstPersonWeaponModelInstanceComponent.MakeOwnerPlayer();
	}

	public void MakeOwnerPlayer()
	{
		_isThisPlayerWeapon = true;
	}

	public abstract void InitializeWeapon();

	public void InstantiateWeapon(Transform NPCweaponSlotTransform)
	{
		MakeOwnerNPC();

		ThirdPersonWeaponModelInstance = gameObject;

		_thirdPersonRightHandWeaponSlotTransform = NPCweaponSlotTransform;
		ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonRightHandWeaponSlotTransform, true);

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;
	}

	public void MakeOwnerNPC()
	{
		_isThisPlayerWeapon = false;
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

	public void FlipWeaponModel()
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
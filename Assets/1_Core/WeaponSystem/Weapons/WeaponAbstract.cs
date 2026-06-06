using System;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
	public abstract string WeaponType { get; }
	public abstract bool IsWeaponAuto { get; }

	public abstract string WeaponName { get; }
	public abstract string WeaponNameSystem { get; }

	protected bool _isWeaponAutoAttacking;
	protected Coroutine _weaponAutoAttackCourutine;
	protected float _weaponAutoAttackSpeedRate;


	private GameObject _firstPersonLeftHandWeaponSlotGameObject;
	private GameObject _firstPersonRightHandWeaponSlotGameObject;
	private GameObject _thirdPersonLeftHandWeaponSlotGameObject;
	private GameObject _thirdPersonRightHandWeaponSlotGameObject;
	public abstract Sprite WeaponIcon { get; }         
	protected bool _isThisPlayerWeapon;
	public abstract float WeaponDamage { get; }

	public GameObject FirstPersonWeaponModelInstance { get; protected set; }
	public GameObject ThirdPersonWeaponModelInstance { get; protected set; } 

	private Transform _firstPersonLeftHandWeaponSlotTransform;
	private Transform _firstPersonRightHandWeaponSlotTransform;
	private Transform _thirdPersonLeftHandWeaponSlotTransform;
	private Transform _thirdPersonRightHandWeaponSlotTransform;

	public void MakeOwnerPlayer()
	{
		_isThisPlayerWeapon = true;
	}

	public void MakeOwnerNPC()
	{
		_isThisPlayerWeapon = false;
	}

	public virtual void StopAutoAttacking()
	{

	}

	public virtual void WeaponAttack()
	{

	}

	public void InstantiateWeapon(Transform NPCweaponSlotTransform)
	{
		MakeOwnerNPC();

		ThirdPersonWeaponModelInstance = gameObject;

		_thirdPersonRightHandWeaponSlotTransform = NPCweaponSlotTransform;
		ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonRightHandWeaponSlotTransform, true);

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;
	}

	public void InstantiateFirstPersonWeaponInstance()
	{
		FirstPersonWeaponModelInstance = Instantiate(gameObject);
		WeaponAbstract FirstPersonWeaponModelInstanceComponent = FirstPersonWeaponModelInstance.GetComponent<WeaponAbstract>();
		FirstPersonWeaponModelInstanceComponent.MakeOwnerPlayer();
	}

	public void InstantiateWeapon(WeaponHandsEnum handType)
	{
		_isThisPlayerWeapon = true;

		string handString = "";

		if (handType == WeaponHandsEnum.HandRight)
		{
			handString = "RightHand";
		}
		else if (handType == WeaponHandsEnum.HandLeft)
		{
			handString = "LeftHand";
		}
		else
		{
			throw new ArgumentException("Неверный тип руки.");
		}

		ThirdPersonWeaponModelInstance = gameObject;
		InstantiateFirstPersonWeaponInstance();

		FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");
		foreach (Transform child in FirstPersonWeaponModelInstance.transform)
			child.gameObject.layer = LayerMask.NameToLayer("FirstPerson");

		if (handString == "LeftHand")
		{
			_firstPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonLeftHandWeaponSlotGameObject");
			_firstPersonLeftHandWeaponSlotTransform = _firstPersonLeftHandWeaponSlotGameObject.transform;
			FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonLeftHandWeaponSlotTransform, true);

			_thirdPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonLeftHandWeaponSlotGameObject");
			_thirdPersonLeftHandWeaponSlotTransform = _thirdPersonLeftHandWeaponSlotGameObject.transform;
			ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonLeftHandWeaponSlotTransform, true);
		}
		else if (handString == "RightHand")
		{
			_firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonRightHandWeaponSlotGameObject");
			_firstPersonRightHandWeaponSlotTransform = _firstPersonRightHandWeaponSlotGameObject.transform;
			FirstPersonWeaponModelInstance.transform.SetParent(_firstPersonRightHandWeaponSlotTransform, true);

			_thirdPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonRightHandWeaponSlotGameObject");
			_thirdPersonRightHandWeaponSlotTransform = _thirdPersonRightHandWeaponSlotGameObject.transform;
			ThirdPersonWeaponModelInstance.transform.SetParent(_thirdPersonRightHandWeaponSlotTransform, true);
		}

		FirstPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

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
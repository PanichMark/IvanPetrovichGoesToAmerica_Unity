using System;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
	public virtual string WeaponNameUI { get; protected set; }
	public virtual string WeaponNameSystem { get; protected set; }

	private GameObject firstPersonLeftHandWeaponSlotGameObject;
	private GameObject firstPersonRightHandWeaponSlotGameObject;
	private GameObject thirdPersonLeftHandWeaponSlotGameObject;
	private GameObject thirdPersonRightHandWeaponSlotGameObject;

	public virtual Sprite WeaponIcon { get; protected set; }           
	protected bool IsThisPlayerWeapon;
	public virtual float WeaponDamage { get; protected set; }

	public GameObject FirstPersonWeaponModelInstance { get; protected set; }
	public GameObject ThirdPersonWeaponModelInstance { get; protected set; } 

	private Transform firstPersonLeftHandWeaponSlotTransform;
	private Transform firstPersonRightHandWeaponSlotTransform;
	private Transform thirdPersonLeftHandWeaponSlotTransform;
	private Transform thirdPersonRightHandWeaponSlotTransform;

	public void MakeOwnerPlayer()
	{
		IsThisPlayerWeapon = true;
	}

	public void MakeOwnerNPC()
	{
		IsThisPlayerWeapon = false;
	}
	public virtual void WeaponAttack()
	{

	}

	public void InstantiateWeapon(Transform NPCweaponSlotTransform)
	{
		MakeOwnerNPC();

		ThirdPersonWeaponModelInstance = gameObject;

		
		thirdPersonRightHandWeaponSlotTransform = NPCweaponSlotTransform;
		ThirdPersonWeaponModelInstance.transform.SetParent(thirdPersonRightHandWeaponSlotTransform, true);

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
		IsThisPlayerWeapon = true;

		string handString = "";

		switch (handType)
		{
			case WeaponHandsEnum.RightHand:
				handString = "RightHand";
				break;
			case WeaponHandsEnum.LeftHand:
				handString = "LeftHand";
				break;
			default:
				throw new ArgumentException("Неверный тип руки.");
		}

		ThirdPersonWeaponModelInstance = gameObject;
		InstantiateFirstPersonWeaponInstance();

		FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");
		foreach (Transform child in FirstPersonWeaponModelInstance.transform)
			child.gameObject.layer = LayerMask.NameToLayer("FirstPerson");

		if (handString == "LeftHand")
		{
			firstPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonLeftHandWeaponSlotGameObject");
			firstPersonLeftHandWeaponSlotTransform = firstPersonLeftHandWeaponSlotGameObject.transform;
			FirstPersonWeaponModelInstance.transform.SetParent(firstPersonLeftHandWeaponSlotTransform, true);

			thirdPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonLeftHandWeaponSlotGameObject");
			thirdPersonLeftHandWeaponSlotTransform = thirdPersonLeftHandWeaponSlotGameObject.transform;
			ThirdPersonWeaponModelInstance.transform.SetParent(thirdPersonLeftHandWeaponSlotTransform, true);
		}
		else if (handString == "RightHand")
		{
			firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("FirstPersonRightHandWeaponSlotGameObject");
			firstPersonRightHandWeaponSlotTransform = firstPersonRightHandWeaponSlotGameObject.transform;
			FirstPersonWeaponModelInstance.transform.SetParent(firstPersonRightHandWeaponSlotTransform, true);

			thirdPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("ThirdPersonRightHandWeaponSlotGameObject");
			thirdPersonRightHandWeaponSlotTransform = thirdPersonRightHandWeaponSlotGameObject.transform;
			ThirdPersonWeaponModelInstance.transform.SetParent(thirdPersonRightHandWeaponSlotTransform, true);
		}

		FirstPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;
	}

	public void DestroyWeapon()
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

	public void FlipWeapon()
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
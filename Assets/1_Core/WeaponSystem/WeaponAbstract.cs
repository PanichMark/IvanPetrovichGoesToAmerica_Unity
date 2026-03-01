using System;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
	public virtual string WeaponNameUI { get; protected set; }

	public virtual string WeaponNameSystem { get; protected set; }

	
	public virtual Sprite WeaponIcon { get; protected set; }            // Иконка оружия

	public virtual float WeaponDamage { get; protected set; }

             // Модель оружия
	public GameObject FirstPersonWeaponModelInstance { get; protected set; } // Первая камера
	public GameObject ThirdPersonWeaponModelInstance { get; protected set; } // Третья камера

	private MeshRenderer firstPersonWeaponMeshRenderer;
	private MeshRenderer thirdPersonWeaponMeshRenderer;

	// Объекты слотов для прикрепления моделей
	private Transform firstPersonLeftHandWeaponSlotTransform;
	private Transform firstPersonRightHandWeaponSlotTransform;
	private Transform thirdPersonLeftHandWeaponSlotTransform;
	private Transform thirdPersonRightHandWeaponSlotTransform;

	public virtual void WeaponAttack()
	{
		// Реализация атаки должна быть в подклассах
	}

	// Создание модели оружия
	public void InstantiateWeaponModel(WeaponHandsEnum handType)
	{
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


		FirstPersonWeaponModelInstance = Instantiate(gameObject);
		ThirdPersonWeaponModelInstance = Instantiate(gameObject);

		FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");
		foreach (Transform child in FirstPersonWeaponModelInstance.transform)
			child.gameObject.layer = LayerMask.NameToLayer("FirstPerson");

		if (handString == "LeftHand")
		{
			firstPersonLeftHandWeaponSlotTransform = GameObject.Find("Slot1.L").transform;
			FirstPersonWeaponModelInstance.transform.SetParent(firstPersonLeftHandWeaponSlotTransform, true);

			thirdPersonLeftHandWeaponSlotTransform = GameObject.Find("Slot.L").transform;
			ThirdPersonWeaponModelInstance.transform.SetParent(thirdPersonLeftHandWeaponSlotTransform, true);
		}
		else if (handString == "RightHand")
		{
			firstPersonRightHandWeaponSlotTransform = GameObject.Find("Slot1.R").transform;
			FirstPersonWeaponModelInstance.transform.SetParent(firstPersonRightHandWeaponSlotTransform, true);

			thirdPersonRightHandWeaponSlotTransform = GameObject.Find("Slot.R").transform;
			ThirdPersonWeaponModelInstance.transform.SetParent(thirdPersonRightHandWeaponSlotTransform, true);
		}

		FirstPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;
		
	}

	// Удаление модели оружия
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
}

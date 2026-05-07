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

	public virtual Sprite WeaponIcon { get; protected set; }            // Иконка оружия
	protected bool IsThisPlayerWeapon;
	public virtual float WeaponDamage { get; protected set; }

             // Модель оружия
	public GameObject FirstPersonWeaponModelInstance { get; protected set; } // Первая камера
	public GameObject ThirdPersonWeaponModelInstance { get; protected set; } // Третья камера


	// Объекты слотов для прикрепления моделей
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
		// Реализация атаки должна быть в подклассах
	}


	public void InstantiateWeapon(Transform NPCweaponSlotTransform)
	{
		//Debug.Log("BRUH!");
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

	// Создание модели оружия
	public void InstantiateWeapon(WeaponHandsEnum handType)
	{
		//Debug.Log("PLAYER");
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


		//object FirstPersonWeaponModelAbstractClass = FirstPersonWeaponModelInstance.GetComponent<WeaponAbstract>();
		//FirstPersonWeaponModelAbstractClass.Make
		//Debug.Log(gameObject);
		ThirdPersonWeaponModelInstance = gameObject;
		InstantiateFirstPersonWeaponInstance();
		//FirstPersonWeaponModelInstance = gameObject;

		FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");
		foreach (Transform child in FirstPersonWeaponModelInstance.transform)
			child.gameObject.layer = LayerMask.NameToLayer("FirstPerson");

		if (handString == "LeftHand")
		{
			firstPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("firstPersonLeftHandWeaponSlotGameObject");
			firstPersonLeftHandWeaponSlotTransform = firstPersonLeftHandWeaponSlotGameObject.transform;
			FirstPersonWeaponModelInstance.transform.SetParent(firstPersonLeftHandWeaponSlotTransform, true);

			thirdPersonLeftHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("thirdPersonLeftHandWeaponSlotGameObject");
			thirdPersonLeftHandWeaponSlotTransform = thirdPersonLeftHandWeaponSlotGameObject.transform;
			ThirdPersonWeaponModelInstance.transform.SetParent(thirdPersonLeftHandWeaponSlotTransform, true);
		}
		else if (handString == "RightHand")
		{
			firstPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("firstPersonRightHandWeaponSlotGameObject");
			firstPersonRightHandWeaponSlotTransform = firstPersonRightHandWeaponSlotGameObject.transform;
			FirstPersonWeaponModelInstance.transform.SetParent(firstPersonRightHandWeaponSlotTransform, true);

			thirdPersonRightHandWeaponSlotGameObject = ServiceLocator.Resolve<GameObject>("thirdPersonRightHandWeaponSlotGameObject");
			thirdPersonRightHandWeaponSlotTransform = thirdPersonRightHandWeaponSlotGameObject.transform;
			ThirdPersonWeaponModelInstance.transform.SetParent(thirdPersonRightHandWeaponSlotTransform, true);
		}

		FirstPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;

		ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
		ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;
		
	}

	// Удаление модели оружия
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
		// Проверяем и переворачиваем модель для камеры от первого лица
		if (FirstPersonWeaponModelInstance != null)
		{
			Vector3 fpScale = FirstPersonWeaponModelInstance.transform.localScale;
			fpScale.x *= -1;
			FirstPersonWeaponModelInstance.transform.localScale = fpScale;
		}

		// Проверяем и переворачиваем модель для камеры от третьего лица
		if (ThirdPersonWeaponModelInstance != null)
		{
			Vector3 tpScale = ThirdPersonWeaponModelInstance.transform.localScale;
			tpScale.x *= -1;
			ThirdPersonWeaponModelInstance.transform.localScale = tpScale;
		}
	}

}

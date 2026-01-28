using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
	[SerializeField]
	public string WeaponNameUI;            // Видимое название оружия в интерфейсе

	[SerializeField]
	public string WeaponNameSystem;

	[SerializeField]
	public Sprite WeaponIcon;              // Иконка оружия

	public virtual float WeaponDamage { get; protected set; }

	public GameObject weaponModel;                  // Модель оружия
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
	public void InstantiateWeaponModel(string handType)
	{
		if (weaponModel != null)
		{
			FirstPersonWeaponModelInstance = Instantiate(weaponModel);
			ThirdPersonWeaponModelInstance = Instantiate(weaponModel);

			FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");
			foreach (Transform child in FirstPersonWeaponModelInstance.transform)
				child.gameObject.layer = LayerMask.NameToLayer("FirstPerson");

			if (handType == "left")
			{
				firstPersonLeftHandWeaponSlotTransform = GameObject.Find("Slot1.L").transform;
				FirstPersonWeaponModelInstance.transform.SetParent(firstPersonLeftHandWeaponSlotTransform, true);

				thirdPersonLeftHandWeaponSlotTransform = GameObject.Find("Slot.L").transform;
				ThirdPersonWeaponModelInstance.transform.SetParent(thirdPersonLeftHandWeaponSlotTransform, true);
			}
			else if (handType == "right")
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

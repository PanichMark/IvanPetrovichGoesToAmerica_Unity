using UnityEngine;

public abstract class WeaponClass : MonoBehaviour
{
	public string WeaponNameSystem { get; protected set; }
	public string WeaponNameUI { get; protected set; }
	public virtual float WeaponDamage {  get; protected set; }

	protected GameObject weaponModel; // Ссылка на 3D модель оружия
	public GameObject FirstPersonWeaponModelInstance { get; protected set; } // Ссылка на инстанцированную модель
	public GameObject ThirdPersonWeaponModelInstance { get; protected set; } // Ссылка на инстанцированную модель

	private MeshRenderer FirstPersonWeaponMeshRenderer;
	private MeshRenderer ThirdPersonWeaponMeshRenderer;

	// Теперь слот для рук задаётся через инспектор
	private GameObject ThirdPersonLeftHandWeaponSlot; // Левый слот (кость руки)
	private GameObject ThirdRightHandWeaponSlot; // Правый слот (кость руки)
	private Transform ThirdLeftHandWeaponSlotTransform; // Левый слот (кость руки)
	private Transform ThirdRightHandWeaponSlotTransform; // Правый слот (кость руки)

	// Теперь слот для рук задаётся через инспектор
	private GameObject FirstPersonLeftHandWeaponSlot; // Левый слот (кость руки)
	private GameObject FirstRightHandWeaponSlot; // Правый слот (кость руки)


	private Transform FirstLeftHandWeaponSlotTransform; // Левый слот (кость руки)
	private Transform FirstRightHandWeaponSlotTransform; // Правый слот (кость руки)

	public virtual void WeaponAttack()
	{
		// 4 weapon classes override this method
	}

	
	public void InstantiateWeaponModel(string handType)
	{
		if (weaponModel != null)
		{
			
			FirstPersonWeaponModelInstance = Instantiate(weaponModel);
			ThirdPersonWeaponModelInstance = Instantiate(weaponModel);
			FirstPersonWeaponMeshRenderer = GetComponent<MeshRenderer>();
			ThirdPersonWeaponMeshRenderer = GetComponent<MeshRenderer>();
			FirstPersonWeaponModelInstance.transform.parent = transform;
			FirstPersonWeaponModelInstance.layer = LayerMask.NameToLayer("FirstPerson");
			// Перебираем всех непосредственных детей объекта
			foreach (Transform child in FirstPersonWeaponModelInstance.transform)
			{
				// Применяем слой каждому ребенку
				child.gameObject.layer = LayerMask.NameToLayer("FirstPerson");
			}

			if (handType == "left")
			{
				ThirdLeftHandWeaponSlotTransform = GameObject.Find("Slot.L").transform;
				ThirdPersonWeaponModelInstance.transform.SetParent(ThirdLeftHandWeaponSlotTransform, true);

				FirstLeftHandWeaponSlotTransform = GameObject.Find("Slot1.L").transform;
				FirstPersonWeaponModelInstance.transform.SetParent(FirstLeftHandWeaponSlotTransform, true);

			}
			else if(handType == "right")
			{

				ThirdLeftHandWeaponSlotTransform = GameObject.Find("Slot.R").transform;
				ThirdPersonWeaponModelInstance.transform.SetParent(ThirdLeftHandWeaponSlotTransform, true);

				FirstLeftHandWeaponSlotTransform = GameObject.Find("Slot1.R").transform;
				FirstPersonWeaponModelInstance.transform.SetParent(FirstLeftHandWeaponSlotTransform, true);

			}
			// Обнуляем локальную позицию и ориентацию
			FirstPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
			FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;
			//FirstPersonWeaponModelInstance.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

			ThirdPersonWeaponModelInstance.transform.localPosition = Vector3.zero;
			ThirdPersonWeaponModelInstance.transform.localRotation = Quaternion.identity;
		}
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
}

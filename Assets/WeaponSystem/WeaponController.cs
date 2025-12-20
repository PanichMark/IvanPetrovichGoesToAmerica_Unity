using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
	private IInputDevice inputDevice;

	// Конструктор принимает зависимость
	public WeaponController(IInputDevice inputDevice)
	{
		this.inputDevice = inputDevice;
	}
	//public PlayerCamera playerCamera;

	public bool IsPoliceBatonWeaponUnlocked {  get; private set; }
	public bool IsHarmoniceRevolverWeaponUnlocked { get; private set; }
	public bool IsPlungerCrossbowWeaponUnlocked { get; private set; }
	public bool IsEugenicGenieWeaponUnlocked { get; private set; }

	//InputManager playerInputsList;
	WeaponWheelController weaponWheelController;
	PlayerBehaviour playerBehaviour;
	InteractionController interactionController;

	public WeaponClass LeftHandWeapon {  get; private set; }
	public WeaponClass RightHandWeapon {  get; private set; }

	private void Awake()
	{
		inputDevice = new InputKeyboard();
	}

	private void Start()
	{
		IsPoliceBatonWeaponUnlocked = true;
		IsHarmoniceRevolverWeaponUnlocked = true;
		IsPlungerCrossbowWeaponUnlocked = true;
		IsEugenicGenieWeaponUnlocked = true;
		
		
		//playerInputsList = GetComponent<InputManager>();
		weaponWheelController = GetComponent<WeaponWheelController>();
		playerBehaviour = GetComponent<PlayerBehaviour>();
		interactionController = GetComponent<InteractionController>();
	}

	private void Update()
	{
		if (inputDevice.GetKeyRightHandWeaponAttack() && !MenuManager.IsAnyMenuOpened)
		{
			RightWeaponAttack();
		}

		if (inputDevice.GetKeyLeftHandWeaponAttack() && !MenuManager.IsAnyMenuOpened)
		{
			LeftWeaponAttack();
		}
	}

	public void SelectWeapon(System.Type weaponType)
	{
		bool isLeftHand = weaponWheelController.IsWeaponLeftHand;

		// Проверяем, есть ли оружие в левой руке
		if (isLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetType() == weaponType)
		{
			// Если текущее оружие совпадает с выбранным, ничего не делаем
			//Debug.Log("Same left");
			return;
		}
		// Проверяем, есть ли оружие в правой руке
		else if (!isLeftHand && RightHandWeapon != null && RightHandWeapon.GetType() == weaponType)
		{
			// Если текущее оружие совпадает с выбранным, ничего не делаем
			//Debug.Log("Same right");
			return;
		}
		else
		{
			// Если оружие не найдено ни в одной руке, создаем новый экземпляр оружия
			if (isLeftHand)
			{
				if (LeftHandWeapon != null)
				{
					RemoveWeapon("left");
				}
				else if (RightHandWeapon != null && RightHandWeapon.GetType() == weaponType)
				{
					RemoveWeapon("right");
				}

				// Создаем новый экземпляр оружия
				LeftHandWeapon = (WeaponClass)gameObject.AddComponent(weaponType);
				weaponWheelController.ChangeWheaponWheelButtonColor("left");
				LeftHandWeapon.InstantiateWeaponModel("left"); // Передаем флаг isLeftHand
				playerBehaviour.ArmPlayer();

				if (interactionController.CurrentPickableObject != null)
				{
					playerBehaviour.DisarmPlayer();
					//Debug.Log("DISARM");
				}
			}
			else
			{
				if (RightHandWeapon != null)
				{
					RemoveWeapon("right");
				}
				else if (LeftHandWeapon != null && LeftHandWeapon.GetType() == weaponType)
				{
					RemoveWeapon("left");
				}

				// Создаем новый экземпляр оружия
				RightHandWeapon = (WeaponClass)gameObject.AddComponent(weaponType);
				weaponWheelController.ChangeWheaponWheelButtonColor("right");
				RightHandWeapon.InstantiateWeaponModel("right"); // Передаем флаг isLeftHand
				playerBehaviour.ArmPlayer();

				if (interactionController.CurrentPickableObject != null)
				{
					playerBehaviour.DisarmPlayer();
					//Debug.Log("DISARM");
				}
			}

			if (LeftHandWeapon != null && RightHandWeapon != null && RightHandWeapon.WeaponNameSystem == LeftHandWeapon.WeaponNameSystem)
			{
				if (isLeftHand == true)
				{
					RemoveWeapon("right");
				}
				else if (isLeftHand == false)
				{
					RemoveWeapon("left");
				}
			}

			Debug.Log("LeftHand: " + (LeftHandWeapon?.WeaponNameSystem ?? "null") + " | RightHand: " + (RightHandWeapon?.WeaponNameSystem ?? "null"));
		}
	}

	public void RightWeaponAttack()
	{
		if (RightHandWeapon != null)
		{
			if (RightHandWeapon.ThirdPersonWeaponModelInstance.activeInHierarchy)
			{ 
			RightHandWeapon.WeaponAttack();
			}
			playerBehaviour.ArmPlayer();
		}
	}

	public void LeftWeaponAttack()
	{
		if (LeftHandWeapon != null)
		{
			if (LeftHandWeapon.ThirdPersonWeaponModelInstance.activeInHierarchy)
			{ 
			LeftHandWeapon.WeaponAttack();
			}
			playerBehaviour.ArmPlayer();
		}
	}

	public void RemoveWeapon(string handType)
	{
		if (handType == "right")
		{
			RightHandWeapon.DestroyWeaponModel(); // Добавляем вызов Unequip()
			Destroy(RightHandWeapon); // Уничтожаем предыдущее оружие
			RightHandWeapon = null;
		}
		else if (handType == "left")
		{
			LeftHandWeapon.DestroyWeaponModel(); // Добавляем вызов Unequip()
			Destroy(LeftHandWeapon); // Уничтожаем предыдущее оружие
			LeftHandWeapon = null;
		}
	}

	public void ShowWeapon(string handType)
	{ 
		if (handType == "right")
		{
			RightHandWeapon.FirstPersonWeaponModelInstance.SetActive(true);
			RightHandWeapon.ThirdPersonWeaponModelInstance.SetActive(true);
		}
		else if (handType == "left")
		{
			LeftHandWeapon.FirstPersonWeaponModelInstance.SetActive(true);
			LeftHandWeapon.ThirdPersonWeaponModelInstance.SetActive(true);
		}
	}

	public void HideWeapon(string handType)
	{
		if (handType == "right")
		{
			RightHandWeapon.FirstPersonWeaponModelInstance.SetActive(false);
			RightHandWeapon.ThirdPersonWeaponModelInstance.SetActive(false);
		}
		else if (handType == "left")
		{
			LeftHandWeapon.FirstPersonWeaponModelInstance.SetActive(false);
			LeftHandWeapon.ThirdPersonWeaponModelInstance.SetActive(false);
		}
	}

	public void UnlockPoliceBatonWeapon()
	{
		IsPoliceBatonWeaponUnlocked = true;
	}

	public void UnlockHarmonicaRevolverWeapon()
	{
		IsHarmoniceRevolverWeaponUnlocked = true;
	}

	public void UnlockPlungerCrossbowWeapon()
	{
		IsPlungerCrossbowWeaponUnlocked = true;
	}

	public void UnlockEugenicGenieWeapon()
	{
		IsEugenicGenieWeaponUnlocked = false;
	}
}

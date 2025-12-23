using UnityEngine;
using UnityEngine.UI;

public delegate void OnWeaponChanged(string activeHand);

public class WeaponController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	private PlayerBehaviour playerBehaviour;

	// Конструктор принимает зависимость
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour)
	{
		this.inputDevice = inputDevice;
		this.menuManager = menuManager;	
		this.playerBehaviour = playerBehaviour;
		Debug.Log("WeaponController Initialized");
	}


	public event OnWeaponChanged OnWeaponChanged;

	public bool IsPoliceBatonWeaponUnlocked {  get; private set; }
	public bool IsHarmoniceRevolverWeaponUnlocked { get; private set; }
	public bool IsPlungerCrossbowWeaponUnlocked { get; private set; }
	public bool IsEugenicGenieWeaponUnlocked { get; private set; }

	public GameObject LeftHandWeapon {  get; private set; }
	public GameObject RightHandWeapon {  get; private set; }

	public WeaponClass leftHandWeaponComponent { get; private set; }

	public WeaponClass rightHandWeaponComponent { get; private set; }

	//public bool IsLeftHand { get; private set; }



	private void Start()
	{
		IsPoliceBatonWeaponUnlocked = true;
		IsHarmoniceRevolverWeaponUnlocked = true;
		IsPlungerCrossbowWeaponUnlocked = true;
		IsEugenicGenieWeaponUnlocked = true;
	}

	private void Update()
	{
		if (inputDevice.GetKeyRightHandWeaponAttack() && !menuManager.IsAnyMenuOpened)
		{
			RightWeaponAttack();
		}

		if (inputDevice.GetKeyLeftHandWeaponAttack() && !menuManager.IsAnyMenuOpened)
		{
			LeftWeaponAttack();
		}

		if (playerBehaviour.IsPlayerArmed)
		{
			if (RightHandWeapon != null)
			{
				ShowWeapon("right");
			}
			if (LeftHandWeapon != null)
			{
				ShowWeapon("left");
			}
		}
		else 
		{
			if (RightHandWeapon != null)
			{
				HideWeapon("right");
			}
			if (LeftHandWeapon != null)
			{
				HideWeapon("left");
			}
		}

	}


	public void SelectWeapon(GameObject weaponPrefab)
	{
		bool isLeftHand = inputDevice.GetKeyLeftHandWeaponWheel();

		// Извлекаем компонент оружия из префаба
		WeaponClass weaponComponent = weaponPrefab.GetComponent<WeaponClass>();
		if (weaponComponent == null)
		{
			Debug.LogError("Prefab must contain a WeaponClass component.");
			return;
		}

		// Проверяем, есть ли оружие в левой руке
		if (isLeftHand && LeftHandWeapon != null && LeftHandWeapon == weaponPrefab)
		{
			// Если текущее оружие совпадает с выбранным, ничего не делаем
			return;
		}
		// Проверяем, есть ли оружие в правой руке
		else if (!isLeftHand && RightHandWeapon != null && RightHandWeapon == weaponPrefab)
		{
			// Если текущее оружие совпадает с выбранным, ничего не делаем
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
				else if (RightHandWeapon != null && RightHandWeapon == weaponPrefab)
				{
					RemoveWeapon("right");
				}

				// Устанавливаем новое оружие в левую руку
				LeftHandWeapon = weaponPrefab;
				OnWeaponChanged?.Invoke("left");
				weaponComponent.InstantiateWeaponModel("left");
				leftHandWeaponComponent = LeftHandWeapon.GetComponent<WeaponClass>();
				playerBehaviour.ArmPlayer();
			}
			else
			{
				if (RightHandWeapon != null)
				{
					RemoveWeapon("right");
				}
				else if (LeftHandWeapon != null && LeftHandWeapon == weaponPrefab)
				{
					RemoveWeapon("left");
				}

				// Устанавливаем новое оружие в правую руку
				RightHandWeapon = weaponPrefab;
				OnWeaponChanged?.Invoke("right");

				weaponComponent.InstantiateWeaponModel("right");
				rightHandWeaponComponent = RightHandWeapon.GetComponent<WeaponClass>();
				playerBehaviour.ArmPlayer();
			}

			if (LeftHandWeapon != null && RightHandWeapon != null &&
			   RightHandWeapon.GetComponent<WeaponClass>().WeaponNameSystem ==
			   LeftHandWeapon.GetComponent<WeaponClass>().WeaponNameSystem)
			{
				if (isLeftHand)
				{
					RemoveWeapon("right");
				}
				else
				{
					RemoveWeapon("left");
				}
			}

			Debug.Log("LeftHand: " + (LeftHandWeapon ? LeftHandWeapon.name : "null") +
					 " | RightHand: " + (RightHandWeapon ? RightHandWeapon.name : "null"));
		}
	}





	/*
	public void SelectWeapon(System.Type weaponType)
	{
		
		bool isLeftHand = inputDevice.GetKeyLeftHandWeaponWheel();

		// Проверяем, есть ли оружие в левой руке
		if (isLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetType() == weaponType)
		{
			// Если текущее оружие совпадает с выбранным, ничего не делаем
			return;
		}
		// Проверяем, есть ли оружие в правой руке
		else if (!isLeftHand && RightHandWeapon != null && RightHandWeapon.GetType() == weaponType)
		{
			// Если текущее оружие совпадает с выбранным, ничего не делаем
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
				/////
				OnWeaponChanged?.Invoke("left");
				LeftHandWeapon.InstantiateWeaponModel("left"); // Передаем флаг isLeftHand
				playerBehaviour.ArmPlayer();


				
				//if (interactionController.CurrentPickableObject != null)
				//{
				//	playerBehaviour.DisarmPlayer();
				//	//Debug.Log("DISARM");
				//}
				
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
				////
				OnWeaponChanged?.Invoke("right");
				RightHandWeapon.InstantiateWeaponModel("right"); // Передаем флаг isLeftHand
				playerBehaviour.ArmPlayer();


				
				//if (interactionController.CurrentPickableObject != null)
				//{
					//playerBehaviour.DisarmPlayer();
					/////Debug.Log("DISARM");
				//}
				
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
	*/

	public void RightWeaponAttack()
	{
		if (RightHandWeapon != null)
		{
			
			rightHandWeaponComponent.WeaponAttack();
			playerBehaviour.ArmPlayer();
		}
	}



	public void LeftWeaponAttack()
	{
		if (LeftHandWeapon != null)
		{

			
			leftHandWeaponComponent.WeaponAttack();
			playerBehaviour.ArmPlayer();
		}
	}


	public void RemoveWeapon(string handType)
{
    if (handType == "right")
    {
        if (RightHandWeapon != null)
        {
         
            rightHandWeaponComponent.DestroyWeaponModel();
            
            // Уничтожаем объект оружия
            Destroy(RightHandWeapon);
            RightHandWeapon = null;
        }
    }
    else if (handType == "left")
    {
        if (LeftHandWeapon != null)
        {
         
            leftHandWeaponComponent.DestroyWeaponModel();
            
            // Уничтожаем объект оружия
            Destroy(LeftHandWeapon);
            LeftHandWeapon = null;
        }
    }
}

	public void ShowWeapon(string handType)
	{
		if (handType == "right" && RightHandWeapon != null)
		{
			RightHandWeapon.SetActive(true);
		}
		else if (handType == "left" && LeftHandWeapon != null)
		{
			LeftHandWeapon.SetActive(true);
		}
	}

	public void HideWeapon(string handType)
	{
		if (handType == "right")
		{
			RightHandWeapon.SetActive(false);
		}
		else if (handType == "left")
		{
			LeftHandWeapon.SetActive(false);
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

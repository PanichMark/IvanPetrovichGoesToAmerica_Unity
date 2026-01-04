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

	public bool hasAnyWeapon { get; private set; } = false;

	public bool IsPoliceBatonWeaponUnlocked { get; private set; }
	public bool IsHarmonicaRevolverWeaponUnlocked { get; private set; }
	public bool IsPlungerCrossbowWeaponUnlocked { get; private set; }
	public bool IsEugenicGenieWeaponUnlocked { get; private set; }

	public GameObject LeftHandWeapon { get; private set; }
	public GameObject RightHandWeapon { get; private set; }

	public WeaponClass leftHandWeaponComponent { get; private set; }
	public WeaponClass rightHandWeaponComponent { get; private set; }

	private void Start()
	{
		ResetAllWeapons(); // Сбрасываем все оружие в начале
	}

	// Разблокировка оружия
	public void UnlockPoliceBatonWeapon()
	{
		IsPoliceBatonWeaponUnlocked = true;
		SetHasAnyWeapon();
	}

	public void UnlockHarmonicaRevolverWeapon()
	{
		IsHarmonicaRevolverWeaponUnlocked = true;
		SetHasAnyWeapon();
	}

	public void UnlockPlungerCrossbowWeapon()
	{
		IsPlungerCrossbowWeaponUnlocked = true;
		SetHasAnyWeapon();
	}

	public void UnlockEugenicGenieWeapon()
	{
		IsEugenicGenieWeaponUnlocked = true;
		SetHasAnyWeapon();
	}

	// Устанавливаем флаг наличия оружия
	private void SetHasAnyWeapon()
	{
		hasAnyWeapon = true;
	}

	// Сброс всех доступных видов оружия
	public void ResetAllWeapons()
	{
		IsPoliceBatonWeaponUnlocked = false;
		IsHarmonicaRevolverWeaponUnlocked = false;
		IsPlungerCrossbowWeaponUnlocked = false;
		IsEugenicGenieWeaponUnlocked = false;
		hasAnyWeapon = false;
	}

	// Выбор оружия
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
			// Если оружие не найдено ни в одной руке, устанавливаем новое оружие
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

	// Атака оружием
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

	// Удаление оружия
	public void RemoveWeapon(string handType)
	{
		if (handType == "right")
		{
			if (RightHandWeapon != null)
			{
				rightHandWeaponComponent.DestroyWeaponModel();
				Destroy(RightHandWeapon);
				RightHandWeapon = null;
			}
		}
		else if (handType == "left")
		{
			if (LeftHandWeapon != null)
			{
				leftHandWeaponComponent.DestroyWeaponModel();
				Destroy(LeftHandWeapon);
				LeftHandWeapon = null;
			}
		}
	}

	// Отображение оружия
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


}
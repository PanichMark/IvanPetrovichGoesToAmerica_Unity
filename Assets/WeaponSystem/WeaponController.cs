using System.Collections.Generic;
using UnityEngine;

public delegate void OnWeaponUnlocked(GameObject weaponPrefab);
public delegate void OnWeaponChanged(string activeHand);

public class WeaponController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	private PlayerBehaviour playerBehaviour;

	// Список разблокированных видов оружия
	public Dictionary<string, GameObject> unlockedWeapons = new Dictionary<string, GameObject>();

	public event OnWeaponUnlocked OnWeaponUnlocked; // Делегат для уведомления о разблокировке оружия

	public event OnWeaponChanged OnWeaponChanged;

	public bool hasAnyWeapon { get; private set; } = false;

	public GameObject LeftHandWeapon { get; private set; }
	public GameObject RightHandWeapon { get; private set; }

	public WeaponClass leftHandWeaponComponent { get; private set; }
	public WeaponClass rightHandWeaponComponent { get; private set; }

	// Инициализация контроллера
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour)
	{
		this.inputDevice = inputDevice;
		this.menuManager = menuManager;
		this.playerBehaviour = playerBehaviour;
		Debug.Log("WeaponController Initialized");
	}


	private void Start()
	{
		ResetAllWeapons(); // Сбрасываем все оружие в начале
	}

	// Единственный метод разблокировки оружия
	public void UnlockWeapon(GameObject weaponPrefab)
	{
		// Выделяем имя оружия и индекс из имени префаба
		string[] parts = weaponPrefab.name.Split('_');
		if (parts.Length != 2)
		{
			Debug.LogError("Некорректное имя префаба оружия!");
			return;
		}

		string weaponName = parts[0]; // Имя оружия
		int index = int.Parse(parts[1]); // Индекс оружия

		// Создаем ключ в формате "ИмяОружия_Индекс"
		string key = $"{weaponName}_{index}";

		// Добавляем оружие в словарь
		unlockedWeapons[key] = weaponPrefab;
		SetHasAnyWeapon();

		// Вызываем делегат для уведомления о разблокировке оружия
		OnWeaponUnlocked?.Invoke(weaponPrefab);

		Debug.Log($"Unlocked {weaponPrefab}");
	}

	// Устанавливаем флаг наличия оружия
	private void SetHasAnyWeapon()
	{
		hasAnyWeapon = true;
	}

	// Сброс всех доступных видов оружия
	public void ResetAllWeapons()
	{
		unlockedWeapons.Clear();
		hasAnyWeapon = false;
	}

	// Выбор оружия
	public void SelectWeapon(GameObject weaponPrefab)
	{
		bool isLeftHand = inputDevice.GetKeyLeftHandWeaponWheel();

		WeaponClass weaponComponent = weaponPrefab.GetComponent<WeaponClass>();
		if (weaponComponent == null)
		{
			Debug.LogError("Prefab must contain a WeaponClass component.");
			return;
		}

		// Проверяем конфликт оружия в руках
		if (isLeftHand && LeftHandWeapon != null && LeftHandWeapon == weaponPrefab ||
			!isLeftHand && RightHandWeapon != null && RightHandWeapon == weaponPrefab)
		{
			return; // Ничего не меняем, если оружие уже установлено
		}

		// Проверяем конфликт имен системы оружия
		if (isLeftHand && RightHandWeapon != null && RightHandWeapon.GetComponent<WeaponClass>().WeaponNameSystem == weaponComponent.WeaponNameSystem ||
			!isLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetComponent<WeaponClass>().WeaponNameSystem == weaponComponent.WeaponNameSystem)
		{
			// Если совпадают имена системы, убираем предыдущее оружие
			if (isLeftHand)
			{
				RemoveWeapon("right");
			}
			else
			{
				RemoveWeapon("left");
			}
		}

		// Установка нового оружия
		if (isLeftHand)
		{
			if (LeftHandWeapon != null)
			{
				RemoveWeapon("left");
			}
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
			RightHandWeapon = weaponPrefab;
			OnWeaponChanged?.Invoke("right");
			weaponComponent.InstantiateWeaponModel("right");
			rightHandWeaponComponent = RightHandWeapon.GetComponent<WeaponClass>();
			playerBehaviour.ArmPlayer();
		}

		Debug.Log("LeftHand: " + (LeftHandWeapon ? LeftHandWeapon.name : "null") +
				  " | RightHand: " + (RightHandWeapon ? RightHandWeapon.name : "null"));
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
	// Удаление оружия
	public void RemoveWeapon(string handType)
	{
		if (handType == "right")
		{
			if (RightHandWeapon != null)
			{
				rightHandWeaponComponent.DestroyWeaponModel();
				RightHandWeapon = null;
			}
		}
		else if (handType == "left")
		{
			if (LeftHandWeapon != null)
			{
				leftHandWeaponComponent.DestroyWeaponModel();
				LeftHandWeapon = null;
			}
		}
	}

	// Сбор разблокированных видов оружия
	public List<GameObject> CollectActiveWeapons()
	{
		return new List<GameObject>(unlockedWeapons.Values);
	}
}
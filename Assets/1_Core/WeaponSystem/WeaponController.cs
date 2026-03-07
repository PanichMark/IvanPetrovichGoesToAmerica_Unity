using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnWeaponUnlocked(GameObject weaponPrefab);
public delegate void OnWeaponChanged(string activeHand);

public class WeaponController : MonoBehaviour
{
	private IInputDevice inputDevice;
	private MenuManager menuManager;
	private PlayerBehaviour playerBehaviour;
	private InteractionController interactionController;

	// Список разблокированных видов оружия
	public Dictionary<string, GameObject> unlockedWeapons = new Dictionary<string, GameObject>();

	public event OnWeaponUnlocked OnWeaponUnlocked; // Делегат для уведомления о разблокировке оружия

	public event OnWeaponChanged OnWeaponChanged;
	public bool isLeftHand {  get; private set; }
	public bool isAbleToUseRightWeapon { get; private set; }
	public bool isAbleToUseLeftWeapon { get; private set; }

	public bool hasAnyWeapon { get; private set; } = false;

	public GameObject LeftHandWeapon { get; private set; }
	public GameObject RightHandWeapon { get; private set; }

	public WeaponAbstract leftHandWeaponComponent { get; private set; }
	public WeaponAbstract rightHandWeaponComponent { get; private set; }

	


	// Инициализация контроллера
	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour, InteractionController interactionController)
	{
		this.inputDevice = inputDevice;
		this.menuManager = menuManager;
		this.playerBehaviour = playerBehaviour;
		this.interactionController = interactionController;

		isAbleToUseRightWeapon = true;
		isAbleToUseLeftWeapon = true;

		// Подписываемся на события игрока
		this.playerBehaviour.OnPlayerArmed += OnPlayerArmed;
		this.playerBehaviour.OnPlayerDisarmed += OnPlayerDisarmed;

		this.interactionController.OnPickUpNonThrowable += () =>
		{
			isAbleToUseRightWeapon = false;
			isAbleToUseLeftWeapon = false;
		};
		this.interactionController.OnPickUpThrowable += () =>
		{
			isAbleToUseRightWeapon = false;
			isAbleToUseLeftWeapon = true;

			if (RightHandWeapon != null)
			{
				HideWeapon(WeaponHandsEnum.RightHand);
			}
		};
		this.interactionController.OnGetRidOfPickable += OnGetRidOfPickableHandler;

		ResetAllWeapons(); // Сбрасываем все оружие в начале
		

		_isInitialized = true;
		Debug.Log("WeaponController Initialized");
	}
	// Синхронный обработчик события
	private void OnGetRidOfPickableHandler()
	{
		StartCoroutine(OnGetRidOfPickableCourutine()); // Запускаем корутину
	}

	// Ваша корутина
	private IEnumerator OnGetRidOfPickableCourutine()
	{
		yield return new WaitForSecondsRealtime(0.05f); // Пауза 0.05 секунды
		isAbleToUseRightWeapon = true;
		isAbleToUseLeftWeapon = true;

		if (RightHandWeapon != null)
		{
			ShowWeapon(WeaponHandsEnum.RightHand);
		}
		yield return null;
	}
	private void OnPlayerArmed()
	{
		// Реакция на событие "игрок вооружился": покажем оружие
		if (rightHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.RightHand); // Показать оружие правой руки
		}

		if (leftHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.LeftHand); // Показать оружие левой руки
		}
	}

	private void OnPlayerDisarmed()
	{
		// Реакция на событие "игрок обезоружился": спрячем оружие
		if (rightHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.RightHand); // Скрыть оружие правой руки
		}

		if (leftHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.LeftHand); // Скрыть оружие левой руки
		}
	}


	private bool _isInitialized = false;

	private void Update()
	{
		// Если инициализация не завершена, ничего не делаем
		if (!_isInitialized)
			return;
		if (inputDevice.GetKeyRightHandWeaponAttack() && !menuManager.IsAnyMenuOpened && isAbleToUseRightWeapon)
		{
			RightWeaponAttack();
		}

		if (inputDevice.GetKeyLeftHandWeaponAttack() && !menuManager.IsAnyMenuOpened && isAbleToUseLeftWeapon)
		{
			LeftWeaponAttack();
		}

		isLeftHand = inputDevice.GetKeyLeftHandWeaponWheel();

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
		

		WeaponAbstract weaponComponent = weaponPrefab.GetComponent<WeaponAbstract>();
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
		if (isLeftHand && RightHandWeapon != null && RightHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == weaponComponent.WeaponNameSystem ||
			!isLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == weaponComponent.WeaponNameSystem)
		{
			// Если совпадают имена системы, убираем предыдущее оружие
			if (isLeftHand)
			{
				DestroyWeapon(WeaponHandsEnum.RightHand);
			}
			else
			{
				DestroyWeapon(WeaponHandsEnum.LeftHand);
			}
		}

		// Установка нового оружия
		if (isLeftHand)
		{
			if (LeftHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.LeftHand);
			}
			LeftHandWeapon = weaponPrefab;
			OnWeaponChanged?.Invoke("left");
			weaponComponent.InstantiateWeaponModel(WeaponHandsEnum.LeftHand);
			leftHandWeaponComponent = LeftHandWeapon.GetComponent<WeaponAbstract>();
			playerBehaviour.ArmPlayer();
		}
		else
		{
			if (RightHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.RightHand);
			}
			RightHandWeapon = weaponPrefab;
			OnWeaponChanged?.Invoke("right");
			weaponComponent.InstantiateWeaponModel(WeaponHandsEnum.RightHand);
			rightHandWeaponComponent = RightHandWeapon.GetComponent<WeaponAbstract>();
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
	public void DestroyWeapon(WeaponHandsEnum handType)
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


		if (handString == "RightHand")
		{
			if (RightHandWeapon != null)
			{
				rightHandWeaponComponent.DestroyWeaponModel();
				RightHandWeapon = null;
			}
		}
		else if (handString == "LeftHand")
		{
			if (LeftHandWeapon != null)
			{
				leftHandWeaponComponent.DestroyWeaponModel();
				LeftHandWeapon = null;
			}
		}
	}

	public void ShowWeapon(WeaponHandsEnum handType)
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

		// Теперь используем обработанное строковое значение дальше
		if (handString == "RightHand" && rightHandWeaponComponent != null)
		{
			if (rightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				rightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);

			if (rightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				rightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
		}
		else if (handString == "LeftHand" && leftHandWeaponComponent != null)
		{
			if (leftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				leftHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);

			if (leftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				leftHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
		}
	}

	public void HideWeapon(WeaponHandsEnum handType)
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

		if (handString == "RightHand" && rightHandWeaponComponent != null)
		{
			if (rightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				rightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

			if (rightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				rightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);
		}
		else if (handString == "LeftHand" && leftHandWeaponComponent != null)
		{
			if (leftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				leftHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

			if (leftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				leftHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);
		}
	}


	// Сбор разблокированных видов оружия
	public List<GameObject> CollectActiveWeapons()
	{
		return new List<GameObject>(unlockedWeapons.Values);
	}
}

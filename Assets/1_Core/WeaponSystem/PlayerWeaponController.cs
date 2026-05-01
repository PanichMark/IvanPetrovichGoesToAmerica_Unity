using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnWeaponUnlocked(GameObject weaponPrefab);
public delegate void OnWeaponChanged(string activeHand);


public class PlayerWeaponController : MonoBehaviour
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

		

		// Проверяем нажатие кнопки перезарядки
		if (inputDevice.GetKeyReload())
		{
			// Кэшируем компоненты, чтобы не вызывать GetComponent несколько раз
			WeaponAbstract leftWeapon = LeftHandWeapon?.GetComponent<WeaponAbstract>();
			WeaponAbstract rightWeapon = RightHandWeapon?.GetComponent<WeaponAbstract>();

			// 1. Сначала пытаемся перезарядить оружие в левой руке
			if (leftHandWeaponComponent != null && leftWeapon is WeaponRangedAbstract)
			{
				(leftWeapon as WeaponRangedAbstract).Reload();
			}
			// 2. Если в левой руке нет оружия или оно не стрелковое, пробуем правую
			if (rightHandWeaponComponent != null && rightWeapon is WeaponRangedAbstract)
			{
				(rightWeapon as WeaponRangedAbstract).Reload();
			}
		}

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
		// 1. Создаем экземпляр (копию) оружия в сцене.
		// Это ключевой момент: мы работаем с объектом в мире, а не с шаблоном в папке Assets.
		GameObject weaponInstance = Instantiate(weaponPrefab);

		// 2. Получаем компонент оружия от нового экземпляра.
		WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();
		if (weaponComponent == null)
		{
			Debug.LogError("Prefab must contain a WeaponAbstract component.");
			Destroy(weaponInstance); // Удаляем неправильно созданный объект
			return;
		}

		// --- Блок проверки конфликтов ---

		// 3. Проверка: Не пытаемся ли мы взять в ту же руку то же самое оружие?
		bool isSameObject = (isLeftHand && LeftHandWeapon == weaponInstance) || (!isLeftHand && RightHandWeapon == weaponInstance);
		if (isSameObject)
		{
			return; // Ничего не делаем
		}

		// 4. Проверка конфликта имен системы (например, нельзя два одинаковых револьвера)
		string newWeaponSystemName = weaponComponent.WeaponNameSystem;

		if (isLeftHand && RightHandWeapon != null && RightHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			// Если в правой руке оружие с таким же системным именем, убираем его.
			DestroyWeapon(WeaponHandsEnum.RightHand);
		}
		else if (!isLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			// Если в левой руке оружие с таким же системным именем, убираем его.
			DestroyWeapon(WeaponHandsEnum.LeftHand);
		}

		// --- Блок установки нового оружия ---

		if (isLeftHand)
		{
			// Если в левой руке уже что-то есть - убираем это.
			if (LeftHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.LeftHand);
			}

			// Устанавливаем НОВЫЙ экземпляр в левую руку
			LeftHandWeapon = weaponInstance;
			OnWeaponChanged?.Invoke("left");

			// Создаем модель для этой руки
			weaponComponent.InstantiateWeapon(WeaponHandsEnum.LeftHand);
			weaponComponent.FlipWeapon();

			// Сохраняем ссылку на компонент для быстрой работы
			leftHandWeaponComponent = weaponComponent;

			playerBehaviour.ArmPlayer();
		}
		else
		{
			// Если в правой руке уже что-то есть - убираем это.
			if (RightHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.RightHand);
			}

			// Устанавливаем НОВЫЙ экземпляр в правую руку
			RightHandWeapon = weaponInstance;
			OnWeaponChanged?.Invoke("right");

			// Создаем модель для этой руки
			weaponComponent.InstantiateWeapon(WeaponHandsEnum.RightHand);

			// Сохраняем ссылку на компонент для быстрой работы
			rightHandWeaponComponent = weaponComponent;

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
		// Используем конструкцию switch для наглядности
		switch (handType)
		{
			case WeaponHandsEnum.RightHand:
				// Проверяем, есть ли что-то в правой руке
				if (RightHandWeapon != null)
				{
					// Уничтожаем ВЕСЬ игровой объект (и скрипт, и модель)
					Destroy(RightHandWeapon);
					rightHandWeaponComponent.DestroyWeapon();

					// Сбрасываем ссылки на объект и его компонент
					RightHandWeapon = null;
					rightHandWeaponComponent = null;
				}
				break;

			case WeaponHandsEnum.LeftHand:
				// Проверяем, есть ли что-то в левой руке
				if (LeftHandWeapon != null)
				{
					// Уничтожаем ВЕСЬ игровой объект (и скрипт, и модель)
					Destroy(LeftHandWeapon);
					leftHandWeaponComponent.DestroyWeapon();

					// Сбрасываем ссылки на объект и его компонент
					LeftHandWeapon = null;
					leftHandWeaponComponent = null;
				}
				break;
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnAnyWeaponUnlocked(GameObject weaponPrefab);

public delegate void OnWeaponChanged(WeaponHandsEnum activeHand);

public class PlayerWeaponController : MonoBehaviour, ISaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PlayerBehaviourController _playerBehaviour;
	private InteractionController _interactionController;
	private PlayerResourcesAmmoManager _ammoManager;
	private HUDhealthAndManaController _HUDhealthAndManaController;
	private GameController _gameController;
	public Dictionary<string, GameObject> UnlockedWeapons = new Dictionary<string, GameObject>();

	private bool _wasRightButtonPressedLastFrame;
	private bool _wasLeftButtonPressedLastFrame;

	public event OnAnyWeaponUnlocked OnAnyWeaponUnlocked; 

	public event OnWeaponChanged OnWeaponChanged;

	public bool IsLeftHand {  get; private set; }
	public bool IsAbleToUseRightWeapon { get; private set; }
	public bool IsAbleToUseLeftWeapon { get; private set; }

	public bool HasAnyWeapon { get; private set; } = false;
	public GameObject LeftHandWeapon { get; private set; }
	public GameObject RightHandWeapon { get; private set; }

	public WeaponAbstract LeftHandWeaponComponent { get; private set; }
	public WeaponAbstract RightHandWeaponComponent { get; private set; }

	public void Initialize(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		HUDhealthAndManaController HUDhealthAndManaController,
		PlayerResourcesAmmoManager ammoManager,
		InteractionController interactionController)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_playerBehaviour = playerBehaviour;
		_ammoManager = ammoManager;
		_interactionController = interactionController;
		_HUDhealthAndManaController = HUDhealthAndManaController;

		IsAbleToUseRightWeapon = true;
		IsAbleToUseLeftWeapon = true;

		_playerBehaviour.OnPlayerArmed += OnPlayerArmed;
		_playerBehaviour.OnPlayerDisarmed += OnPlayerDisarmed;

		_interactionController.OnPickUpNonThrowable += () =>
		{
			IsAbleToUseRightWeapon = false;
			IsAbleToUseLeftWeapon = false;
		};
		_interactionController.OnPickUpThrowable += () =>
		{
			IsAbleToUseRightWeapon = false;
			IsAbleToUseLeftWeapon = true;

			if (RightHandWeapon != null)
			{
				HideWeapon(WeaponHandsEnum.HandRight);
			}
		};
		_menuManager.OnClosePauseMenu += () =>
		{
			StopRihtWeaponAutoAttack();
			StopLeftWeaponAutoAttack();
		};

		_interactionController.OnGetRidOfThrowable += OnGetRidOfPickableThrowableHandler;
		_interactionController.OnGetRidOfNonThrowable += OnGetRidOfPickableNonThrowableHandler;
		_gameController.OnPlayerEarlyDeath += DisarmPlayerOnDeath;

		ResetAllWeapons(); 

		Debug.Log("PlayerWeaponController");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_inputDevice.GetKeyRightHandWeaponAttackReleased())
		{
			StopRihtWeaponAutoAttack();
			_wasRightButtonPressedLastFrame = false;
		}
		if (RightHandWeapon != null && RightHandWeapon.activeInHierarchy)
		{
			if (_inputDevice.GetKeyRightHandWeaponAttack() && !_wasRightButtonPressedLastFrame && !_menuManager.IsAnyMenuOpened && IsAbleToUseRightWeapon)
			{
				RightWeaponAttack();
				_wasRightButtonPressedLastFrame = true;
			}
		}
		if (RightHandWeapon != null && !RightHandWeapon.activeInHierarchy)
		{
			if (_inputDevice.GetKeyRightHandWeaponAttack() && !_wasRightButtonPressedLastFrame && !_menuManager.IsAnyMenuOpened && IsAbleToUseRightWeapon)
			{
				_playerBehaviour.ArmPlayer();
			}
		}

		if (_inputDevice.GetKeyLeftHandWeaponAttackReleased())
		{
			StopLeftWeaponAutoAttack();
			_wasLeftButtonPressedLastFrame = false;
		}
		if (LeftHandWeapon != null && LeftHandWeapon.activeInHierarchy)
		{
			if (_inputDevice.GetKeyLeftHandWeaponAttack() && !_wasLeftButtonPressedLastFrame && !_menuManager.IsAnyMenuOpened && IsAbleToUseLeftWeapon)
			{
				LeftWeaponAttack();
				_wasLeftButtonPressedLastFrame = true;
			}
		}
		if (LeftHandWeapon != null && !LeftHandWeapon.activeInHierarchy)
		{
			if (_inputDevice.GetKeyLeftHandWeaponAttack() && !_wasLeftButtonPressedLastFrame && !_menuManager.IsAnyMenuOpened && IsAbleToUseLeftWeapon)
			{
				_playerBehaviour.ArmPlayer();
			}
		}

		IsLeftHand = _inputDevice.GetKeyLeftHandWeaponWheel();

		if (_inputDevice.GetKeyReload())
		{
			WeaponAbstract leftWeapon = LeftHandWeapon?.GetComponent<WeaponAbstract>();
			WeaponAbstract rightWeapon = RightHandWeapon?.GetComponent<WeaponAbstract>();

			if (LeftHandWeaponComponent != null && leftWeapon is WeaponRangedAbstract)
			{
				(leftWeapon as WeaponRangedAbstract).Reload();
			}
			if (RightHandWeaponComponent != null && rightWeapon is WeaponRangedAbstract)
			{
				(rightWeapon as WeaponRangedAbstract).Reload();
			}
		}
	}

	private void DisarmPlayerOnDeath()
	{
		if(_playerBehaviour.IsPlayerArmed)
		{
			_playerBehaviour.DisarmPlayer();
		}	
	}

	private void OnGetRidOfPickableNonThrowableHandler()
	{
		StartCoroutine(OnGetRidOfPickableNonThrowableCourutine());
	}

	private IEnumerator OnGetRidOfPickableNonThrowableCourutine()
	{
		yield return new WaitForSecondsRealtime(0.05f);
		IsAbleToUseRightWeapon = true;
		IsAbleToUseLeftWeapon = true;

		yield return null;
	}

	private void OnGetRidOfPickableThrowableHandler()
	{
		StartCoroutine(OnGetRidOfPickableThrowableCourutine()); 
	}

	private IEnumerator OnGetRidOfPickableThrowableCourutine()
	{
		yield return new WaitForSecondsRealtime(0.05f);
		IsAbleToUseRightWeapon = true;
		IsAbleToUseLeftWeapon = true;

		if (RightHandWeapon != null && _playerBehaviour.IsPlayerArmed)
		{
			ShowWeapon(WeaponHandsEnum.HandRight);
		}
		yield return null;
	}

	private void OnPlayerArmed()
	{
		//Debug.Log(RightHandWeaponComponent);
		if (RightHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.HandRight); 
		}

		if (LeftHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.HandLeft);
		}
	}

	private void OnPlayerDisarmed()
	{
		if (RightHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.HandRight); 
		}

		if (LeftHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.HandLeft); 
		}
	}

	public void UnlockWeapon(GameObject weaponPrefab)
	{
		int index = ExtractWeaponIndex(weaponPrefab.name);

		if (index == -1)
		{
			Debug.LogError($"Некорректное имя префаба оружия: {weaponPrefab.name}. Ожидается формат 'idx[ЧИСЛО]_[Название]'.");
			return;
		}

		string[] parts = weaponPrefab.name.Split('_');
		string weaponName = parts.Length > 1 ? parts[1] : "Unknown";

		string key = $"idx{index}_{weaponName}";

		UnlockedWeapons[key] = weaponPrefab;
		SetHasAnyWeapon();

		var rangedComponent = weaponPrefab.GetComponent<WeaponEugenicAbstract>();
		if (rangedComponent != null)
		{
			SetHasEugenicWeapon();
		}

			OnAnyWeaponUnlocked?.Invoke(weaponPrefab);

		Debug.Log($"Unlocked {key}");
	}

	public int ExtractWeaponIndex(string name)
	{
		int idxPos = name.IndexOf("idx");
		if (idxPos == -1)
			return -1; 

		int nextUnderscorePos = name.IndexOf('_', idxPos + 3);
		if (nextUnderscorePos == -1)
			return -1;

		string numberStr = name.Substring(idxPos + 3, nextUnderscorePos - (idxPos + 3));

		if (int.TryParse(numberStr, out int result))
			return result;

		return -1;
	}

	private void SetHasAnyWeapon()
	{
		HasAnyWeapon = true;
		_HUDhealthAndManaController.ShowHealthBar();
	}

	private void SetHasEugenicWeapon()
	{
		_HUDhealthAndManaController.ShowManaBar();
	}

	public void ResetAllWeapons()
	{
		UnlockedWeapons.Clear();
		HasAnyWeapon = false;
		_HUDhealthAndManaController.HideHealthBar();
		_HUDhealthAndManaController.HideManaBar();
	}

	public void SelectWeapon(GameObject weaponPrefab)
	{
		GameObject weaponInstance = Instantiate(weaponPrefab);
		WeaponAbstract weaponComponent = weaponInstance.GetComponent<WeaponAbstract>();
		if (weaponComponent == null)
		{
			Debug.LogError("Prefab must contain a WeaponAbstract component.");
			Destroy(weaponInstance);
			return;
		}

		WeaponRangedAbstract rangedToSave = null;
		if (IsLeftHand && LeftHandWeapon != null)
		{
			rangedToSave = LeftHandWeaponComponent as WeaponRangedAbstract;
		}
		else if (!IsLeftHand && RightHandWeapon != null)
		{
			rangedToSave = RightHandWeaponComponent as WeaponRangedAbstract;
		}

		if (rangedToSave != null)
		{
			WeaponsRangedEnum key = (WeaponsRangedEnum)System.Enum.Parse(typeof(WeaponsRangedEnum), rangedToSave.WeaponName);
			if (_ammoManager.WeaponsRangedDictionary.TryGetValue(key, out var data))
			{
				data.MagazineAmmoCurrent = rangedToSave.PlayerMagazineAmmoCurrent;
				_ammoManager.WeaponsRangedDictionary[key] = data;
			}
		}

		bool isSameObject = (IsLeftHand && LeftHandWeapon == weaponInstance) || (!IsLeftHand && RightHandWeapon == weaponInstance);
		if (isSameObject)
		{
			return;
		}

		string newWeaponSystemName = weaponComponent.WeaponNameSystem;

		if (IsLeftHand && RightHandWeapon != null && RightHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			DestroyWeapon(WeaponHandsEnum.HandRight);
		}
		else if (!IsLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			DestroyWeapon(WeaponHandsEnum.HandLeft);
		}

		if (weaponComponent is WeaponRangedAbstract rangedNew)
		{
			WeaponsRangedEnum newKey = (WeaponsRangedEnum)System.Enum.Parse(typeof(WeaponsRangedEnum), rangedNew.WeaponName);
			if (_ammoManager.WeaponsRangedDictionary.TryGetValue(newKey, out var newData))
			{
				//rangedNew.SetPlayerWeaponAmmoType(newData.AmmoTypeSystem);
				rangedNew.SetPlayerMagazineProperties(newData.MagazineAmmoMax, newData.MagazineAmmoCurrent);
			}
		}

		if (IsLeftHand)
		{
			if (LeftHandWeapon != null)
			{
				Destroy(LeftHandWeapon);
				LeftHandWeaponComponent.DestroyWeaponModel();
			}

			LeftHandWeapon = weaponInstance;
			OnWeaponChanged?.Invoke(WeaponHandsEnum.HandLeft);

			weaponComponent.InstantiateWeapon(WeaponHandsEnum.HandLeft);
			weaponComponent.FlipWeaponModel();

			LeftHandWeaponComponent = weaponComponent;

			_playerBehaviour.ArmPlayer();
		}
		else
		{
			if (RightHandWeapon != null)
			{
				Destroy(RightHandWeapon);
				RightHandWeaponComponent.DestroyWeaponModel();
			}

			RightHandWeapon = weaponInstance;
			OnWeaponChanged?.Invoke(WeaponHandsEnum.HandRight);

			weaponComponent.InstantiateWeapon(WeaponHandsEnum.HandRight);

			RightHandWeaponComponent = weaponComponent;

			_playerBehaviour.ArmPlayer();
		}
	}

	public void RightWeaponAttack()
	{
		if (RightHandWeapon != null && _playerBehaviour.IsPlayerArmed)
		{
			RightHandWeaponComponent.WeaponAttack();
		}
	}

	public void StopRihtWeaponAutoAttack()
	{
		if (RightHandWeapon != null)
		{
			RightHandWeaponComponent.StopAutoAttacking();
		}
	}

	public void LeftWeaponAttack()
	{
		if (LeftHandWeapon != null && _playerBehaviour.IsPlayerArmed)
		{
			LeftHandWeaponComponent.WeaponAttack();
		}
	}

	public void StopLeftWeaponAutoAttack()
	{
		if (LeftHandWeapon != null)
		{
			LeftHandWeaponComponent.StopAutoAttacking();
		}
	}

	public void DestroyWeapon(WeaponHandsEnum handType)
	{
		if (handType == WeaponHandsEnum.HandRight)
		{
			if (RightHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				WeaponsRangedEnum key = (WeaponsRangedEnum)System.Enum.Parse(typeof(WeaponsRangedEnum), rangedWeapon.WeaponName);
				if (_ammoManager.WeaponsRangedDictionary.TryGetValue(key, out var data))
				{
					data.MagazineAmmoCurrent = rangedWeapon.PlayerMagazineAmmoCurrent;
					_ammoManager.WeaponsRangedDictionary[key] = data;
				}
			}

			if (RightHandWeapon != null)
			{
				Destroy(RightHandWeapon);
				RightHandWeaponComponent.DestroyWeaponModel();

				RightHandWeapon = null;
				RightHandWeaponComponent = null;
			}
		}
		else if (handType == WeaponHandsEnum.HandLeft)
		{
			if (LeftHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				WeaponsRangedEnum key = (WeaponsRangedEnum)System.Enum.Parse(typeof(WeaponsRangedEnum), rangedWeapon.WeaponName);
				if (_ammoManager.WeaponsRangedDictionary.TryGetValue(key, out var data))
				{
					data.MagazineAmmoCurrent = rangedWeapon.PlayerMagazineAmmoCurrent;
					_ammoManager.WeaponsRangedDictionary[key] = data;
				}
			}

			if (LeftHandWeapon != null)
			{
				Destroy(LeftHandWeapon);
				LeftHandWeaponComponent.DestroyWeaponModel();

				LeftHandWeapon = null;
				LeftHandWeaponComponent = null;
			}
		}
	}

	public void ShowWeapon(WeaponHandsEnum handType)
	{
		if (handType == WeaponHandsEnum.HandRight)
		{
			if (RightHandWeaponComponent != null)
			{
				if (RightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
					RightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);

				if (RightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
					RightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
			}
		}
		else if (handType == WeaponHandsEnum.HandLeft)
		{
			if (LeftHandWeaponComponent != null)
			{
				if (LeftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
					LeftHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);

				if (LeftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
					LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
			}
		}
		else
		{
			throw new ArgumentException("Неверный тип руки.");
		}
	}

	public void HideWeapon(WeaponHandsEnum handType)
	{
		if (handType == WeaponHandsEnum.HandRight)
		{
			if (RightHandWeaponComponent != null)
			{
				if (RightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
					RightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

				if (RightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
					RightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);
			}
		}
		else if (handType == WeaponHandsEnum.HandLeft)
		{
			if (LeftHandWeaponComponent != null)
			{
				if (LeftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
					LeftHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

				if (LeftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
					LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);
			}
		}
		else
		{
			throw new ArgumentException("Неверный тип руки.");
		}
	}

	public List<GameObject> CollectActiveWeapons()
	{
		return new List<GameObject>(UnlockedWeapons.Values);
	}

	public void SaveData(ref GameData data)
	{
		data.UnlockedWeapons = new List<string>(UnlockedWeapons.Keys);

		List<WeaponRangedData> rangedWeaponIds = new List<WeaponRangedData>();

		foreach (var weaponEntry in UnlockedWeapons)
		{
			var weaponComp = weaponEntry.Value.GetComponent<WeaponRangedAbstract>();
			if (weaponComp != null)
			{
				WeaponsRangedEnum weaponEnumType;
				System.Enum.TryParse(weaponComp.WeaponNameSystem, out weaponEnumType);

				WeaponRangedData dataToAdd = new WeaponRangedData();
				dataToAdd.RagnedWeaponSystem = weaponEnumType;
				dataToAdd.RagnedWeaponJson = weaponEnumType.ToString(); // Добавляем строковое представление
            
				// Получаем данные об оружии из менеджера ресурсов по его типу
				if (_ammoManager.WeaponsRangedDictionary.TryGetValue(weaponEnumType, out WeaponRangedData weaponState))
				{
					dataToAdd.MagazineAmmoCurrent = weaponState.MagazineAmmoCurrent;
					dataToAdd.AmmoTypeSystem = weaponState.AmmoTypeSystem;
					dataToAdd.AmmoTypeJson = weaponState.AmmoTypeSystem.ToString(); // Добавляем строковое представление
				}

				rangedWeaponIds.Add(dataToAdd);
			}
		}

		data.UnlockedRangedWeapons = rangedWeaponIds;

		// Сохранение активного оружия в руках
		data.WeaponRightHand = RightHandWeapon?.GetComponent<WeaponAbstract>()?.WeaponNameSystem;
		data.WeaponLeftHand = LeftHandWeapon?.GetComponent<WeaponAbstract>()?.WeaponNameSystem;
	}

	public void LoadData(GameData data)
	{
		if (data.UnlockedWeapons != null)
		{
			ResetAllWeapons();
			foreach (string weaponKey in data.UnlockedWeapons)
			{
				GameObject weaponPrefab = Resources.Load<GameObject>("Weapons/" + weaponKey);
				if (weaponPrefab != null)
				{
					UnlockWeapon(weaponPrefab);
				}
			}
		}

		// Загрузка состояния дальнобойного оружия (патроны и т.д.)
		if (data.UnlockedRangedWeapons != null)
		{
			foreach (var loadedWeaponData in data.UnlockedRangedWeapons)
			{
				// 1. Пробуем распарсить строковое представление типа оружия обратно в Enum
				if (Enum.TryParse(loadedWeaponData.RagnedWeaponJson, out WeaponsRangedEnum parsedWeaponType))
				{
					// 2. Ищем шаблон этого оружия в словаре менеджера ресурсов
					if (_ammoManager.WeaponsRangedDictionary.ContainsKey(parsedWeaponType))
					{
						// 3. Получаем ссылку на данные шаблона
						var weaponState = _ammoManager.WeaponsRangedDictionary[parsedWeaponType];

						// 4. Применяем загруженные данные о состоянии магазина
						weaponState.MagazineAmmoCurrent = loadedWeaponData.MagazineAmmoCurrent;

						// Если нужно, можно восстановить и тип патронов из JSON
						if (Enum.TryParse(loadedWeaponData.AmmoTypeJson, out AmmoTypes parsedAmmoType))
						{
							weaponState.AmmoTypeSystem = parsedAmmoType;
						}

						// 5. ОБЯЗАТЕЛЬНО сохраняем обновленные данные обратно в словарь,
						// так как структуры (struct) передаются по значению, а не по ссылке.
						_ammoManager.WeaponsRangedDictionary[parsedWeaponType] = weaponState;
					}
				}
			}
		}

		// Загрузка активного оружия в правую руку
		if (!string.IsNullOrEmpty(data.WeaponRightHand))
		{
			foreach (var unlockedWeapon in UnlockedWeapons)
			{
				WeaponAbstract comp = unlockedWeapon.Value.GetComponent<WeaponAbstract>();
				if (comp != null && comp.WeaponNameSystem == data.WeaponRightHand)
				{
					SelectWeapon(unlockedWeapon.Value);
					break;
				}
			}
		}
		else
		{
			if (RightHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.HandRight);
			}
		}

		// Загрузка активного оружия в левую руку
		if (!string.IsNullOrEmpty(data.WeaponLeftHand))
		{
			IsLeftHand = true; // Устанавливаем флаг, что выбираем для левой руки
			foreach (var unlockedWeapon in UnlockedWeapons)
			{
				WeaponAbstract comp = unlockedWeapon.Value.GetComponent<WeaponAbstract>();
				if (comp != null && comp.WeaponNameSystem == data.WeaponLeftHand)
				{
					SelectWeapon(unlockedWeapon.Value);
					break;
				}
			}
		}
		else
		{
			if (LeftHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.HandLeft);
			}
		}
	}
}
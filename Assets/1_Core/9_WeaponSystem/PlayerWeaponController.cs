using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public delegate void OnWeaponEqiupment();

	public delegate void WeaponVisibilityHandler(WeaponAbstract weapon);
	public event WeaponVisibilityHandler OnShowWeapon;
	public event WeaponVisibilityHandler OnHideWeapon;

	private bool _wasRightButtonPressedLastFrame;
	private bool _wasLeftButtonPressedLastFrame;
	public int LayersToDamage => _layersToDamage;
	private int _layersToDamage;
	public int LayersOrganisms => _layersOrganisms;
	private int _layersOrganisms;
	public int LayersHeads => _layersHeads;
	private int _layersHeads;

	public int LayerNPC => _layerNPC;
	private int _layerNPC;

	public delegate void WeaponShootHandler(WeaponHandsEnum weaponHandType);
	public event WeaponShootHandler OnWeaponShoot;

	public delegate void WeaponUnlockHandler(GameObject weaponPrefab);
	public event WeaponUnlockHandler OnAnyWeaponUnlocked;

	public delegate void WeaponChangeHandler(WeaponHandsEnum activeHand);
	public event WeaponChangeHandler OnWeaponChanged;

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

		_layersToDamage = LayerMask.GetMask("HitboxBody_Organism", "HitboxBody_Robot", "HitboxHead_Organism", "HitboxHead_Robot");
		_layersOrganisms = LayerMask.GetMask("HitboxBody_Organism", "HitboxHead_Organism");
		_layersHeads = LayerMask.GetMask("HitboxHead_Organism", "HitboxHead_Robot");

		_layerNPC = LayerMask.GetMask("NPC");

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
				HideWeapon(WeaponHandsEnum.Right);
			}
		};
		_menuManager.OnClosePauseMenu += () =>
		{
			StopAutoShootingRightWeaponPlayer();
			StopAutoShootingLeftWeaponPlayer();
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
			StopAutoShootingRightWeaponPlayer();
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
			StopAutoShootingLeftWeaponPlayer();
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

	private void LateUpdate()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		IsLeftHand = _inputDevice.GetKeyLeftHandWeaponWheel();
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
			ShowWeapon(WeaponHandsEnum.Right);
		}

		yield return null;
	}

	private void OnPlayerArmed()
	{
		if (RightHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.Right); 
		}

		if (LeftHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.Left);
		}
	}

	private void OnPlayerDisarmed()
	{
		if (RightHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.Right); 
		}

		if (LeftHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.Left); 
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
		AnyWeaponIsUnlocked();

		var eugenicComponent = weaponPrefab.GetComponent<WeaponEugenicAbstract>();
		if (eugenicComponent != null)
		{
			EugenicWeaponIsUnlocked();
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

	private void AnyWeaponIsUnlocked()
	{
		HasAnyWeapon = true;
		_HUDhealthAndManaController.ShowHealthBar();
	}

	private void EugenicWeaponIsUnlocked()
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
			WeaponNames key = (WeaponNames)System.Enum.Parse(typeof(WeaponNames), rangedToSave.WeaponName.ToString());
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
			HideWeapon(WeaponHandsEnum.Right);
			DestroyWeapon(WeaponHandsEnum.Right);
		}
		else if (!IsLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			HideWeapon(WeaponHandsEnum.Left);
			DestroyWeapon(WeaponHandsEnum.Left);
		}

		if (weaponComponent is WeaponRangedAbstract rangedNew)
		{
			WeaponNames newKey = (WeaponNames)System.Enum.Parse(typeof(WeaponNames), rangedNew.WeaponName.ToString());
			if (_ammoManager.WeaponsRangedDictionary.TryGetValue(newKey, out var newData))
			{
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
			OnWeaponChanged?.Invoke(WeaponHandsEnum.Left);

			weaponComponent.InstantiateWeaponPlayer(this, WeaponHandsEnum.Left);
			if (weaponComponent is not WeaponEugenicAbstract)
			{
				weaponComponent.MirrorWeaponPlayerModel();
			}

			LeftHandWeaponComponent = weaponComponent;

			if (_playerBehaviour.IsPlayerArmed)
			{
				ShowWeapon(WeaponHandsEnum.Left);
			}

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
			OnWeaponChanged?.Invoke(WeaponHandsEnum.Right);

			weaponComponent.InstantiateWeaponPlayer(this, WeaponHandsEnum.Right);

			RightHandWeaponComponent = weaponComponent;

			if (_playerBehaviour.IsPlayerArmed)
			{
				ShowWeapon(WeaponHandsEnum.Right);
			}

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

	public void StopAutoShootingRightWeaponPlayer()
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

	public void StopAutoShootingLeftWeaponPlayer()
	{
		if (LeftHandWeapon != null)
		{
			LeftHandWeaponComponent.StopAutoAttacking();
		}
	}

	public void DestroyWeapon(WeaponHandsEnum handType)
	{
		if (handType == WeaponHandsEnum.Right)
		{
			if (RightHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				WeaponNames key = (WeaponNames)System.Enum.Parse(typeof(WeaponNames), rangedWeapon.WeaponName.ToString());
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
		else if (handType == WeaponHandsEnum.Left)
		{
			if (LeftHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				WeaponNames key = (WeaponNames)System.Enum.Parse(typeof(WeaponNames), rangedWeapon.WeaponName.ToString());
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
		if (handType == WeaponHandsEnum.Right)
		{
			if (RightHandWeaponComponent != null)
			{
				if (RightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				{
					RightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);
				}

				if (RightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				{
					RightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
				}

				OnShowWeapon?.Invoke(RightHandWeaponComponent);
			}
		}
		else if (handType == WeaponHandsEnum.Left)
		{
			if (LeftHandWeaponComponent != null)
			{
				if (LeftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				{
					LeftHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);
				}

				if (LeftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				{
					LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
				}

				OnShowWeapon?.Invoke(LeftHandWeaponComponent);
			}
		}
	}

	public void HideWeapon(WeaponHandsEnum handType)
	{
		if (handType == WeaponHandsEnum.Right)
		{
			if (RightHandWeaponComponent != null)
			{
				if (RightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
					RightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

				if (RightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
					RightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);

				OnHideWeapon?.Invoke(RightHandWeaponComponent);
			}
		}
		else if (handType == WeaponHandsEnum.Left)
		{
			if (LeftHandWeaponComponent != null)
			{
				if (LeftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
					LeftHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

				if (LeftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
					LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);

				OnHideWeapon?.Invoke(LeftHandWeaponComponent);
			}
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
				WeaponNames weaponEnumType;
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
				if (Enum.TryParse(loadedWeaponData.RagnedWeaponJson, out WeaponNames parsedWeaponType))
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
				DestroyWeapon(WeaponHandsEnum.Right);
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
				DestroyWeapon(WeaponHandsEnum.Left);
			}
		}
	}
}
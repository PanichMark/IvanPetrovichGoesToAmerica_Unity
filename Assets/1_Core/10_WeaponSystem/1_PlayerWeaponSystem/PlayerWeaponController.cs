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
	private PlayerInteractionController _interactionController;
	private PlayerWeaponAmmoController _ammoManager;
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

	public delegate void WeaponShootHandler(WeaponHandType weaponHandType);
	public event WeaponShootHandler OnWeaponShoot;

	public delegate void WeaponUnlockHandler(GameObject weaponPrefab);
	public event WeaponUnlockHandler OnAnyWeaponUnlocked;

	public delegate void WeaponChangeHandler(WeaponHandType activeHand);
	public event WeaponChangeHandler OnWeaponChanged;

	public bool IsLeftHand {  get; private set; }
	public bool IsAbleToUseRightWeapon { get; private set; }
	public bool IsAbleToUseLeftWeapon { get; private set; }

	public bool HasAnyWeapon { get; private set; }
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
		PlayerWeaponAmmoController ammoManager,
		PlayerInteractionController interactionController)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_playerBehaviour = playerBehaviour;
		_ammoManager = ammoManager;
		_interactionController = interactionController;
		_HUDhealthAndManaController = HUDhealthAndManaController;

		_layersToDamage = LayerMask.GetMask("Default", "Outline", "HitboxBody_Organism", "HitboxBody_Robot", "HitboxHead_Organism", "HitboxHead_Robot");
		_layersOrganisms = LayerMask.GetMask("HitboxBody_Organism", "HitboxHead_Organism");
		_layersHeads = LayerMask.GetMask("HitboxHead_Organism", "HitboxHead_Robot");

		_layerNPC = LayerMask.GetMask("NPC");

		IsAbleToUseRightWeapon = true;
		IsAbleToUseLeftWeapon = true;

		_playerBehaviour.OnPlayerArmed += OnPlayerArmed;
		_playerBehaviour.OnPlayerDisarmed += OnPlayerDisarmed;

		_interactionController.OnPickUpNonThrowable += (InteractionObjectsPickableTypes pickableType) =>
		{
			IsAbleToUseRightWeapon = false;
			IsAbleToUseLeftWeapon = false;
		};
		_interactionController.OnPickUpThrowable += (InteractionObjectsPickableTypes pickableType) =>
		{
			IsAbleToUseRightWeapon = false;
			IsAbleToUseLeftWeapon = true;

			if (RightHandWeapon != null)
			{
				HideWeapon(WeaponHandType.Right);
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
		if (_inputDevice.GetKeyRightHandWeaponAttack() && !_wasRightButtonPressedLastFrame && !_menuManager.IsAnyMenuOpened)
		{
			RightWeaponAttack();

			if (RightHandWeapon != null && RightHandWeapon.activeInHierarchy)
			{
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
		if (_inputDevice.GetKeyLeftHandWeaponAttack() && !_wasLeftButtonPressedLastFrame && !_menuManager.IsAnyMenuOpened)
		{
			LeftWeaponAttack();

			if (LeftHandWeapon != null && LeftHandWeapon.activeInHierarchy)
			{
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
			ShowWeapon(WeaponHandType.Right);
		}

		yield return null;
	}

	private void OnPlayerArmed()
	{
		if (RightHandWeaponComponent != null)
		{
			if (_interactionController.CurrentIThrowable == null)
			{
				ShowWeapon(WeaponHandType.Right);
			}
		}

		if (LeftHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandType.Left);
		}
	}

	private void OnPlayerDisarmed()
	{
		if (RightHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandType.Right); 
		}

		if (LeftHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandType.Left); 
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
			PlayerWeaponNames key = (PlayerWeaponNames)System.Enum.Parse(typeof(PlayerWeaponNames), rangedToSave.WeaponName.ToString());
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
			HideWeapon(WeaponHandType.Right);
			DestroyWeapon(WeaponHandType.Right);
		}
		else if (!IsLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			HideWeapon(WeaponHandType.Left);
			DestroyWeapon(WeaponHandType.Left);
		}

		if (weaponComponent is WeaponRangedAbstract rangedNew)
		{
			PlayerWeaponNames newKey = (PlayerWeaponNames)System.Enum.Parse(typeof(PlayerWeaponNames), rangedNew.WeaponName.ToString());
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
			OnWeaponChanged?.Invoke(WeaponHandType.Left);

			weaponComponent.InstantiateWeaponPlayer(this, WeaponHandType.Left);
			if (weaponComponent is not WeaponEugenicAbstract)
			{
				weaponComponent.MirrorWeaponPlayerModel();
			}

			LeftHandWeaponComponent = weaponComponent;

			if (_playerBehaviour.IsPlayerArmed)
			{
				ShowWeapon(WeaponHandType.Left);
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
			OnWeaponChanged?.Invoke(WeaponHandType.Right);

			weaponComponent.InstantiateWeaponPlayer(this, WeaponHandType.Right);

			RightHandWeaponComponent = weaponComponent;

			if (_playerBehaviour.IsPlayerArmed)
			{
				ShowWeapon(WeaponHandType.Right);
			}

			_playerBehaviour.ArmPlayer();
		}
	}

	public void RightWeaponAttack()
	{
		//Debug.Log("RIGHT!");

		if (RightHandWeapon != null && _playerBehaviour.IsPlayerArmed && IsAbleToUseRightWeapon)
		{
			RightHandWeaponComponent.WeaponAttack();
		}
		if (_interactionController.CurrentPickableObject != null)
		{
			var pickableWeapon = _interactionController.CurrentPickableObject.GetComponent<InteractionObjectPickableAbstract>();

			if (pickableWeapon is IPickableWeapon rangedWeapon)
			{
				rangedWeapon.AttackRight();
			}
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
		if (LeftHandWeapon != null && _playerBehaviour.IsPlayerArmed && IsAbleToUseLeftWeapon)
		{
			LeftHandWeaponComponent.WeaponAttack();
		}
		if (_interactionController.CurrentPickableObject != null)
		{
			var pickableWeapon = _interactionController.CurrentPickableObject.GetComponent<InteractionObjectPickableAbstract>();

			if (pickableWeapon is IPickableWeapon rangedWeapon)
			{
				rangedWeapon.AttackLeft();
			}
		}
	}

	public void StopAutoShootingLeftWeaponPlayer()
	{
		if (LeftHandWeapon != null)
		{
			LeftHandWeaponComponent.StopAutoAttacking();
		}
	}

	public void DestroyWeapon(WeaponHandType handType)
	{
		if (handType == WeaponHandType.Right)
		{
			if (RightHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				PlayerWeaponNames key = (PlayerWeaponNames)System.Enum.Parse(typeof(PlayerWeaponNames), rangedWeapon.WeaponName.ToString());
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
		else if (handType == WeaponHandType.Left)
		{
			if (LeftHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				PlayerWeaponNames key = (PlayerWeaponNames)System.Enum.Parse(typeof(PlayerWeaponNames), rangedWeapon.WeaponName.ToString());
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

	public void ShowWeapon(WeaponHandType handType)
	{
		if (handType == WeaponHandType.Right)
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
		else if (handType == WeaponHandType.Left)
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

	public void HideWeapon(WeaponHandType handType)
	{
		if (handType == WeaponHandType.Right)
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
		else if (handType == WeaponHandType.Left)
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

	public IEnumerator SaveData(GameData data)
	{
		data.PlayerWeapons.UnlockedPlayerWeapons = new List<string>(UnlockedWeapons.Keys);

		List<WeaponRangedData> rangedWeaponIds = new List<WeaponRangedData>();

		foreach (var weaponEntry in UnlockedWeapons)
		{
			var weaponComp = weaponEntry.Value.GetComponent<WeaponRangedAbstract>();
			if (weaponComp != null)
			{
				PlayerWeaponNames weaponEnumType;
				System.Enum.TryParse(weaponComp.WeaponNameSystem, out weaponEnumType);

				WeaponRangedData dataToAdd = new WeaponRangedData();
				dataToAdd.RagnedWeapon = weaponEnumType;

				if (_ammoManager.WeaponsRangedDictionary.TryGetValue(weaponEnumType, out WeaponRangedData weaponState))
				{
					dataToAdd.MagazineAmmoCurrent = weaponState.MagazineAmmoCurrent;
					dataToAdd.AmmoType = weaponState.AmmoType;
				}

				rangedWeaponIds.Add(dataToAdd);
			}
		}

		data.PlayerWeapons.PlayerWeaponRightHand = RightHandWeapon?.GetComponent<WeaponAbstract>()?.WeaponNameSystem;
		data.PlayerWeapons.PlayerWeaponLeftHand = LeftHandWeapon?.GetComponent<WeaponAbstract>()?.WeaponNameSystem;

		yield return null;
	}

	public IEnumerator LoadData(GameData data)
	{
		//Debug.Log("LOAD WEAPONS");

		HideWeapon(WeaponHandType.Right);
		DestroyWeapon(WeaponHandType.Right);

		HideWeapon(WeaponHandType.Left);
		DestroyWeapon(WeaponHandType.Left);

		if (data.PlayerWeapons.UnlockedPlayerWeapons != null)
		{
			ResetAllWeapons();
			foreach (string weaponKey in data.PlayerWeapons.UnlockedPlayerWeapons)
			{
				GameObject weaponPrefab = Resources.Load<GameObject>("WeaponSystem/WeaponsPlayer/" + weaponKey);
				if (weaponPrefab != null)
				{
					UnlockWeapon(weaponPrefab);
				}
			}
		}

		/*
		// Загрузка состояния дальнобойного оружия (патроны и т.д.)
		if (data.PlayerWeapons.UnlockedPlayerRangedWeapons != null)
		{
			foreach (var loadedWeaponData in data.PlayerWeapons.UnlockedPlayerRangedWeapons)
			{
				if (Enum.TryParse(loadedWeaponData.RagnedWeapon, out PlayerWeaponNames parsedWeaponType))
				{
					if (_ammoManager.WeaponsRangedDictionary.ContainsKey(parsedWeaponType))
					{
						var weaponState = _ammoManager.WeaponsRangedDictionary[parsedWeaponType];
						weaponState.MagazineAmmoCurrent = loadedWeaponData.MagazineAmmoCurrent;

						if (Enum.TryParse(loadedWeaponData.AmmoType, out AmmoTypes parsedAmmoType))
						{
							weaponState.AmmoType = parsedAmmoType;
						}
						_ammoManager.WeaponsRangedDictionary[parsedWeaponType] = weaponState;
					}
				}
			}
		}
		*/

		if (data.PlayerWeapons.PlayerWeaponRightHand != null)
		{
			foreach (var unlockedWeapon in UnlockedWeapons)
			{
				WeaponAbstract comp = unlockedWeapon.Value.GetComponent<WeaponAbstract>();
				if (comp != null && comp.WeaponNameSystem == data.PlayerWeapons.PlayerWeaponRightHand)
				{
					SelectWeapon(unlockedWeapon.Value);
					break;
				}
			}
		}

		if (data.PlayerWeapons.PlayerWeaponLeftHand != null)
		{
			IsLeftHand = true;

			foreach (var unlockedWeapon in UnlockedWeapons)
			{
				WeaponAbstract comp = unlockedWeapon.Value.GetComponent<WeaponAbstract>();
				if (comp != null && comp.WeaponNameSystem == data.PlayerWeapons.PlayerWeaponLeftHand)
				{
					SelectWeapon(unlockedWeapon.Value);
					break;
				}
			}
		}

		yield return null;
	}
}
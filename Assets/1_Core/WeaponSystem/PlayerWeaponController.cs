using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnAnyWeaponUnlocked(GameObject weaponPrefab);

public delegate void OnWeaponChanged(string activeHand);

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
				HideWeapon(WeaponHandsEnum.RightHand);
			}
		};
		_interactionController.OnGetRidOfPickable += OnGetRidOfPickableHandler;
		_gameController.OnPlayerEarlyDeath += DisarmPlayerOnDeath;

		ResetAllWeapons(); 

		Debug.Log("PlayerWeaponController");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		if (_inputDevice.GetKeyRightHandWeaponAttack() && !_menuManager.IsAnyMenuOpened && IsAbleToUseRightWeapon)
		{
			RightWeaponAttack();
		}

		if (_inputDevice.GetKeyLeftHandWeaponAttack() && !_menuManager.IsAnyMenuOpened && IsAbleToUseLeftWeapon)
		{
			LeftWeaponAttack();
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

	private void OnGetRidOfPickableHandler()
	{
		StartCoroutine(OnGetRidOfPickableCourutine()); 
	}

	private IEnumerator OnGetRidOfPickableCourutine()
	{
		yield return new WaitForSecondsRealtime(0.05f);
		IsAbleToUseRightWeapon = true;
		IsAbleToUseLeftWeapon = true;

		if (RightHandWeapon != null)
		{
			ShowWeapon(WeaponHandsEnum.RightHand);
		}
		yield return null;
	}

	private void OnPlayerArmed()
	{
		if (RightHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.RightHand); 
		}

		if (LeftHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.LeftHand);
		}
	}

	private void OnPlayerDisarmed()
	{
		if (RightHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.RightHand); 
		}

		if (LeftHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.LeftHand); 
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
			WeaponRangedTypes key = (WeaponRangedTypes)System.Enum.Parse(typeof(WeaponRangedTypes), rangedToSave.WeaponNameSystem);
			if (_ammoManager.WeaponDictionary.TryGetValue(key, out var data))
			{
				data.MagazineAmmoCurrent = rangedToSave.PlayerMagazineAmmoCurrent;
				_ammoManager.WeaponDictionary[key] = data;
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
			DestroyWeapon(WeaponHandsEnum.RightHand);
		}
		else if (!IsLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			DestroyWeapon(WeaponHandsEnum.LeftHand);
		}

		if (weaponComponent is WeaponRangedAbstract rangedNew)
		{
			WeaponRangedTypes newKey = (WeaponRangedTypes)System.Enum.Parse(typeof(WeaponRangedTypes), rangedNew.WeaponNameSystem);
			if (_ammoManager.WeaponDictionary.TryGetValue(newKey, out var newData))
			{
				rangedNew.SetPlayerWeaponAmmoType(newData.AmmoType);
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
			OnWeaponChanged?.Invoke("left");

			weaponComponent.InstantiateWeapon(WeaponHandsEnum.LeftHand);
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
			OnWeaponChanged?.Invoke("right");

			weaponComponent.InstantiateWeapon(WeaponHandsEnum.RightHand);

			RightHandWeaponComponent = weaponComponent;

			_playerBehaviour.ArmPlayer();
		}
	}

	public void RightWeaponAttack()
	{
		if (RightHandWeapon != null)
		{
			RightHandWeaponComponent.WeaponAttack();
			_playerBehaviour.ArmPlayer();
		}
	}

	public void LeftWeaponAttack()
	{
		if (LeftHandWeapon != null)
		{
			LeftHandWeaponComponent.WeaponAttack();
			_playerBehaviour.ArmPlayer();
		}
	}

	public void DestroyWeapon(WeaponHandsEnum handType)
	{
		if (handType == WeaponHandsEnum.RightHand)
		{
			if (RightHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				WeaponRangedTypes key = (WeaponRangedTypes)System.Enum.Parse(typeof(WeaponRangedTypes), rangedWeapon.WeaponNameSystem);
				if (_ammoManager.WeaponDictionary.TryGetValue(key, out var data))
				{
					data.MagazineAmmoCurrent = rangedWeapon.PlayerMagazineAmmoCurrent;
					_ammoManager.WeaponDictionary[key] = data;
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
		else if (handType == WeaponHandsEnum.LeftHand)
		{
			if (LeftHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				WeaponRangedTypes key = (WeaponRangedTypes)System.Enum.Parse(typeof(WeaponRangedTypes), rangedWeapon.WeaponNameSystem);
				if (_ammoManager.WeaponDictionary.TryGetValue(key, out var data))
				{
					data.MagazineAmmoCurrent = rangedWeapon.PlayerMagazineAmmoCurrent;
					_ammoManager.WeaponDictionary[key] = data;
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
		if (handType == WeaponHandsEnum.RightHand)
		{
			if (RightHandWeaponComponent != null)
			{
				if (RightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
					RightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);

				if (RightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
					RightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
			}
		}
		else if (handType == WeaponHandsEnum.LeftHand)
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
		if (handType == WeaponHandsEnum.RightHand)
		{
			if (RightHandWeaponComponent != null)
			{
				if (RightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
					RightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

				if (RightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
					RightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);
			}
		}
		else if (handType == WeaponHandsEnum.LeftHand)
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

		if (RightHandWeapon != null)
		{
			data.WeaponRightHand = RightHandWeaponComponent.WeaponNameSystem;

			if (RightHandWeaponComponent is WeaponRangedAbstract rangedRight)
			{
				data.WeaponInRightHandMagazineAmmoCurrent = rangedRight.PlayerMagazineAmmoCurrent;
			}
			else
			{
				data.WeaponInRightHandMagazineAmmoCurrent = 0;
			}
		}
		else
		{
			data.WeaponRightHand = null;
			data.WeaponInRightHandMagazineAmmoCurrent = 0;
		}

		if (LeftHandWeapon != null)
		{
			data.WeaponLefrHand = LeftHandWeaponComponent.WeaponNameSystem;
		
			if (LeftHandWeaponComponent is WeaponRangedAbstract rangedLeft)
			{
				data.WeaponInLeftHandMagazineAmmoCurrent = rangedLeft.PlayerMagazineAmmoCurrent;
			}
			else
			{
				data.WeaponInLeftHandMagazineAmmoCurrent = 0;
			}
		}
		else
		{
			data.WeaponLefrHand = null;
			data.WeaponInLeftHandMagazineAmmoCurrent = 0;
		}
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

		if (!string.IsNullOrEmpty(data.WeaponRightHand) && UnlockedWeapons.ContainsKey(data.WeaponRightHand))
		{
			SelectWeapon(UnlockedWeapons[data.WeaponRightHand]);

			if (RightHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				rangedWeapon.PlayerMagazineAmmoCurrent = data.WeaponInRightHandMagazineAmmoCurrent;
			}
		}

		if (!string.IsNullOrEmpty(data.WeaponLefrHand) && UnlockedWeapons.ContainsKey(data.WeaponLefrHand))
		{
			IsLeftHand = true;
			SelectWeapon(UnlockedWeapons[data.WeaponLefrHand]);

			if (LeftHandWeaponComponent is WeaponRangedAbstract rangedWeapon)
			{
				rangedWeapon.PlayerMagazineAmmoCurrent = data.WeaponInLeftHandMagazineAmmoCurrent;
			}
		}
	}
}
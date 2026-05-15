using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnWeaponUnlocked(GameObject weaponPrefab);
public delegate void OnWeaponChanged(string activeHand);

public class PlayerWeaponController : MonoBehaviour
{
	private IInputDevice _inputDevice;
	private MenuManager _menuManager;
	private PlayerBehaviourController _playerBehaviour;
	private InteractionController _interactionController;


	public Dictionary<string, GameObject> UnlockedWeapons = new Dictionary<string, GameObject>();

	public event OnWeaponUnlocked OnWeaponUnlocked; 

	public event OnWeaponChanged OnWeaponChanged;
	public bool IsLeftHand {  get; private set; }
	public bool IsAbleToUseRightWeapon { get; private set; }
	public bool IsAbleToUseLeftWeapon { get; private set; }

	public bool HasAnyWeapon { get; private set; } = false;

	public GameObject LeftHandWeapon { get; private set; }
	public GameObject RightHandWeapon { get; private set; }

	public WeaponAbstract LeftHandWeaponComponent { get; private set; }
	public WeaponAbstract RightHandWeaponComponent { get; private set; }

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviourController playerBehaviour, InteractionController interactionController)
	{
		_inputDevice = inputDevice;
		_menuManager = menuManager;
		_playerBehaviour = playerBehaviour;
		_interactionController = interactionController;

		IsAbleToUseRightWeapon = true;
		IsAbleToUseLeftWeapon = true;

		_playerBehaviour.OnPlayerArmed += OnPlayerArmed;
		_playerBehaviour.OnPlayerDisarmed += OnPlayerDisarmed;

		_interactionController.OnPickUpNonThrowableObject += () =>
		{
			IsAbleToUseRightWeapon = false;
			IsAbleToUseLeftWeapon = false;
		};
		_interactionController.OnPickUpThrowableObject += () =>
		{
			IsAbleToUseRightWeapon = false;
			IsAbleToUseLeftWeapon = true;

			if (RightHandWeapon != null)
			{
				HideWeapon(WeaponHandsEnum.RightHand);
			}
		};
		_interactionController.OnGetRidOfPickableObject += OnGetRidOfPickableHandler;

		ResetAllWeapons(); 
		
		_isInitialized = true;
		Debug.Log("WeaponController Initialized");
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

	private bool _isInitialized = false;

	private void Update()
	{
		if (!_isInitialized)
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

	public void UnlockWeapon(GameObject weaponPrefab)
	{
		string[] parts = weaponPrefab.name.Split('_');
		if (parts.Length != 2)
		{
			Debug.LogError("Некорректное имя префаба оружия!");
			return;
		}

		string weaponName = parts[0]; 
		int index = int.Parse(parts[1]); 

		string key = $"{weaponName}_{index}";

		UnlockedWeapons[key] = weaponPrefab;
		SetHasAnyWeapon();

		OnWeaponUnlocked?.Invoke(weaponPrefab);

		Debug.Log($"Unlocked {weaponPrefab}");
	}

	private void SetHasAnyWeapon()
	{
		HasAnyWeapon = true;
	}

	public void ResetAllWeapons()
	{
		UnlockedWeapons.Clear();
		HasAnyWeapon = false;
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


		if (IsLeftHand)
		{
			if (LeftHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.LeftHand);
			}

			LeftHandWeapon = weaponInstance;
			OnWeaponChanged?.Invoke("left");

			weaponComponent.InstantiateWeapon(WeaponHandsEnum.LeftHand);
			weaponComponent.FlipWeapon();

			LeftHandWeaponComponent = weaponComponent;

			_playerBehaviour.ArmPlayer();
		}
		else
		{
			if (RightHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.RightHand);
			}

			RightHandWeapon = weaponInstance;
			OnWeaponChanged?.Invoke("right");

			weaponComponent.InstantiateWeapon(WeaponHandsEnum.RightHand);

			RightHandWeaponComponent = weaponComponent;

			_playerBehaviour.ArmPlayer();
		}

		Debug.Log("LeftHand: " + (LeftHandWeapon ? LeftHandWeapon.name : "null") +
				  " | RightHand: " + (RightHandWeapon ? RightHandWeapon.name : "null"));
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
		switch (handType)
		{
			case WeaponHandsEnum.RightHand:
				if (RightHandWeapon != null)
				{
					Destroy(RightHandWeapon);
					RightHandWeaponComponent.DestroyWeapon();

					RightHandWeapon = null;
					RightHandWeaponComponent = null;
				}
				break;

			case WeaponHandsEnum.LeftHand:
				if (LeftHandWeapon != null)
				{
					Destroy(LeftHandWeapon);
					LeftHandWeaponComponent.DestroyWeapon();

					LeftHandWeapon = null;
					LeftHandWeaponComponent = null;
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

		if (handString == "RightHand" && RightHandWeaponComponent != null)
		{
			if (RightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				RightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);

			if (RightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				RightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
		}
		else if (handString == "LeftHand" && LeftHandWeaponComponent != null)
		{
			if (LeftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				LeftHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(true);

			if (LeftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(true);
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

		if (handString == "RightHand" && RightHandWeaponComponent != null)
		{
			if (RightHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				RightHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

			if (RightHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				RightHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);
		}
		else if (handString == "LeftHand" && LeftHandWeaponComponent != null)
		{
			if (LeftHandWeaponComponent.FirstPersonWeaponModelInstance != null)
				LeftHandWeaponComponent.FirstPersonWeaponModelInstance.SetActive(false);

			if (LeftHandWeaponComponent.ThirdPersonWeaponModelInstance != null)
				LeftHandWeaponComponent.ThirdPersonWeaponModelInstance.SetActive(false);
		}
	}

	public List<GameObject> CollectActiveWeapons()
	{
		return new List<GameObject>(UnlockedWeapons.Values);
	}
}
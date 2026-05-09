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


	public Dictionary<string, GameObject> unlockedWeapons = new Dictionary<string, GameObject>();

	public event OnWeaponUnlocked OnWeaponUnlocked; 

	public event OnWeaponChanged OnWeaponChanged;
	public bool isLeftHand {  get; private set; }
	public bool isAbleToUseRightWeapon { get; private set; }
	public bool isAbleToUseLeftWeapon { get; private set; }

	public bool hasAnyWeapon { get; private set; } = false;

	public GameObject LeftHandWeapon { get; private set; }
	public GameObject RightHandWeapon { get; private set; }

	public WeaponAbstract leftHandWeaponComponent { get; private set; }
	public WeaponAbstract rightHandWeaponComponent { get; private set; }

	public void Initialize(IInputDevice inputDevice, MenuManager menuManager, PlayerBehaviour playerBehaviour, InteractionController interactionController)
	{
		this.inputDevice = inputDevice;
		this.menuManager = menuManager;
		this.playerBehaviour = playerBehaviour;
		this.interactionController = interactionController;

		isAbleToUseRightWeapon = true;
		isAbleToUseLeftWeapon = true;

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
		if (rightHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.RightHand); 
		}

		if (leftHandWeaponComponent != null)
		{
			ShowWeapon(WeaponHandsEnum.LeftHand);
		}
	}
	private void OnPlayerDisarmed()
	{
		if (rightHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.RightHand); 
		}

		if (leftHandWeaponComponent != null)
		{
			HideWeapon(WeaponHandsEnum.LeftHand); 
		}
	}

	private bool _isInitialized = false;

	private void Update()
	{
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

		if (inputDevice.GetKeyReload())
		{
			WeaponAbstract leftWeapon = LeftHandWeapon?.GetComponent<WeaponAbstract>();
			WeaponAbstract rightWeapon = RightHandWeapon?.GetComponent<WeaponAbstract>();

			if (leftHandWeaponComponent != null && leftWeapon is WeaponRangedAbstract)
			{
				(leftWeapon as WeaponRangedAbstract).Reload();
			}
			if (rightHandWeaponComponent != null && rightWeapon is WeaponRangedAbstract)
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

		unlockedWeapons[key] = weaponPrefab;
		SetHasAnyWeapon();

		OnWeaponUnlocked?.Invoke(weaponPrefab);

		Debug.Log($"Unlocked {weaponPrefab}");
	}

	private void SetHasAnyWeapon()
	{
		hasAnyWeapon = true;
	}

	public void ResetAllWeapons()
	{
		unlockedWeapons.Clear();
		hasAnyWeapon = false;
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

		bool isSameObject = (isLeftHand && LeftHandWeapon == weaponInstance) || (!isLeftHand && RightHandWeapon == weaponInstance);
		if (isSameObject)
		{
			return; 
		}

		string newWeaponSystemName = weaponComponent.WeaponNameSystem;

		if (isLeftHand && RightHandWeapon != null && RightHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			DestroyWeapon(WeaponHandsEnum.RightHand);
		}
		else if (!isLeftHand && LeftHandWeapon != null && LeftHandWeapon.GetComponent<WeaponAbstract>().WeaponNameSystem == newWeaponSystemName)
		{
			DestroyWeapon(WeaponHandsEnum.LeftHand);
		}


		if (isLeftHand)
		{
			if (LeftHandWeapon != null)
			{
				DestroyWeapon(WeaponHandsEnum.LeftHand);
			}

			LeftHandWeapon = weaponInstance;
			OnWeaponChanged?.Invoke("left");

			weaponComponent.InstantiateWeapon(WeaponHandsEnum.LeftHand);
			weaponComponent.FlipWeapon();

			leftHandWeaponComponent = weaponComponent;

			playerBehaviour.ArmPlayer();
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

			rightHandWeaponComponent = weaponComponent;

			playerBehaviour.ArmPlayer();
		}

		Debug.Log("LeftHand: " + (LeftHandWeapon ? LeftHandWeapon.name : "null") +
				  " | RightHand: " + (RightHandWeapon ? RightHandWeapon.name : "null"));
	}

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

	public void DestroyWeapon(WeaponHandsEnum handType)
	{
		switch (handType)
		{
			case WeaponHandsEnum.RightHand:
				if (RightHandWeapon != null)
				{
					Destroy(RightHandWeapon);
					rightHandWeaponComponent.DestroyWeapon();

					RightHandWeapon = null;
					rightHandWeaponComponent = null;
				}
				break;

			case WeaponHandsEnum.LeftHand:
				if (LeftHandWeapon != null)
				{
					Destroy(LeftHandWeapon);
					leftHandWeaponComponent.DestroyWeapon();

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

	public List<GameObject> CollectActiveWeapons()
	{
		return new List<GameObject>(unlockedWeapons.Values);
	}
}
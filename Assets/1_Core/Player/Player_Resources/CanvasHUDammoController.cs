using TMPro;
using UnityEngine;

public class CanvasHUDammoController : MonoBehaviour
{
	private MenuManager menuManager;
	private GameObject canvasHUDammo;
	private GameSceneManager gameSceneManager;
	private GameController gameController;
	private PlayerWeaponController weaponController;
	private PlayerResourcesAmmoManager playerResourcesAmmoManager;
	private PlayerBehaviour playerBehaviour;

	private GameObject RightWeaponAmmoMagazine;
	private GameObject RightWeaponAmmoReserve;
	private GameObject RightWeaponAmmoSeparator;
	private GameObject LeftWeaponAmmoMagazine;
	private GameObject LeftWeaponAmmoReserve;
	private GameObject LeftWeaponAmmoSeparator;

	private TMP_Text RightWeaponAmmoMagazineText;
	private TMP_Text RightWeaponAmmoReserveText;
	private TMP_Text LeftWeaponAmmoMagazineText;
	private TMP_Text LeftWeaponAmmoReserveText;

	public void Initialize(GameSceneManager gameSceneManager, GameController gameController, MenuManager menuManager, GameObject canvasHUDPlayerResources, PlayerWeaponController weaponController,
		PlayerResourcesAmmoManager playerResourcesAmmoManager, PlayerBehaviour playerBehaviour,
		GameObject RightWeaponAmmoMagazine, GameObject RightWeaponAmmoReserve, GameObject RightWeaponAmmoSeparator,
		GameObject LeftWeaponAmmoMagazine, GameObject LeftWeaponAmmoReserve, GameObject LeftWeaponAmmoSeparator)
	{
		this.gameSceneManager = gameSceneManager;
		this.menuManager = menuManager;
		this.canvasHUDammo = canvasHUDPlayerResources;
		this.weaponController = weaponController;
		this.playerResourcesAmmoManager = playerResourcesAmmoManager;
		this.gameController	= gameController;
		this.playerBehaviour = playerBehaviour;

		this.RightWeaponAmmoMagazine = RightWeaponAmmoMagazine;
		this.RightWeaponAmmoReserve = RightWeaponAmmoReserve;
		this.RightWeaponAmmoSeparator = RightWeaponAmmoSeparator;
		this.LeftWeaponAmmoMagazine = LeftWeaponAmmoMagazine;
		this.LeftWeaponAmmoReserve = LeftWeaponAmmoReserve;
		this.LeftWeaponAmmoSeparator = LeftWeaponAmmoSeparator;

		RightWeaponAmmoMagazineText = RightWeaponAmmoMagazine.GetComponent<TMP_Text>();
		RightWeaponAmmoReserveText = RightWeaponAmmoReserve.GetComponent<TMP_Text>();
		LeftWeaponAmmoMagazineText = LeftWeaponAmmoMagazine.GetComponent<TMP_Text>();
		LeftWeaponAmmoReserveText = LeftWeaponAmmoReserve.GetComponent<TMP_Text>();

		this.menuManager.OnOpenPauseMenu += HideCanvasHUDammo;
		this.menuManager.OnClosePauseMenu += ShowCanvasHUDammo;
		this.menuManager.OnOpenInteractionMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseInteractionMenu += ShowCanvasHUDammo;
		this.menuManager.OnOpenDialogueMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseDialogueMenu += ShowCanvasHUDammo;


		this.playerBehaviour.OnPlayerArmed += ShowCanvasHUDammo;
		this.playerBehaviour.OnPlayerDisarmed += HideCanvasHUDammo;

		this.gameSceneManager.OnBeginLoadMainMenuScene += HideCanvasHUDammo;
		this.gameSceneManager.OnBeginLoadGameplayScene += ShowCanvasHUDammo;

		this.menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDammo;

		this.playerResourcesAmmoManager.OnReserveAmmoChanged += UpdateReserveDisplay;
		this.playerResourcesAmmoManager.OnMagazineAmmoChanged += UpdateMagazineDisplay;

		this.weaponController.OnWeaponChanged += UpdateAmmoDisplayForActiveWeapon;

		HideRightWeaponAmmo();
		HideLeftWeaponAmmo();

		Debug.Log("CanvasHUDammo Initialized");
	}

	private void ShowCanvasHUDammo()
	{
		if (!menuManager.IsInteractionMenuOpened && !menuManager.IsDialogueMenuOpened && !gameController.IsMainMenuOpen && !menuManager.IsWeaponWheelMenuOpened)
		{
			canvasHUDammo.SetActive(true);
		}
	}

	public void HideCanvasHUDammo()
	{
		canvasHUDammo.SetActive(false);
	}
	private void UpdateAmmoDisplayForActiveWeapon(string activeHand)
	{
		if (activeHand == "left")
		{
			var ranged = weaponController.LeftHandWeapon?.GetComponent<WeaponRangedAbstract>();
			if (weaponController.LeftHandWeapon != null && ranged != null)
			{
				ShowLeftWeaponAmmo();

				LeftWeaponAmmoMagazineText.text = ranged.MagazineAmmoCurrent.ToString();

				if (playerResourcesAmmoManager.AmmoDictionary.TryGetValue(ranged.WeaponAmmoType, out var ammoData))
				{
					LeftWeaponAmmoReserveText.text = ammoData.TotalAmmoCurrent.ToString();
				}
			}
			else
			{
				HideLeftWeaponAmmo();
			}
		}
		else 
		{
			var ranged = weaponController.RightHandWeapon?.GetComponent<WeaponRangedAbstract>();
			if (weaponController.RightHandWeapon != null && ranged != null)
			{
				ShowRightWeaponAmmo();

				RightWeaponAmmoMagazineText.text = ranged.MagazineAmmoCurrent.ToString();

				if (playerResourcesAmmoManager.AmmoDictionary.TryGetValue(ranged.WeaponAmmoType, out var ammoData))
				{
					RightWeaponAmmoReserveText.text = ammoData.TotalAmmoCurrent.ToString();
				}
			}
			else
			{
				HideRightWeaponAmmo();
			}
		}

		if(weaponController.RightHandWeapon == null)
		{
			HideRightWeaponAmmo();
		}
		if (weaponController.LeftHandWeapon == null)
		{
			HideLeftWeaponAmmo();
		}

	}

	private void UpdateReserveDisplay(AmmoTypes type, int newTotalAmount)
	{
		if (weaponController.RightHandWeapon != null)
		{
			var rightRanged = weaponController.RightHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (rightRanged != null && rightRanged.WeaponAmmoType == type)
			{
				RightWeaponAmmoReserveText.text = newTotalAmount.ToString();
				return;
			}
		}

		if (weaponController.LeftHandWeapon != null)
		{
			var leftRanged = weaponController.LeftHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (leftRanged != null && leftRanged.WeaponAmmoType == type)
			{
				LeftWeaponAmmoReserveText.text = newTotalAmount.ToString();
				return;
			}
		}
	}

	private void UpdateMagazineDisplay(AmmoTypes type, int newMagazineAmount)
	{
		if (weaponController.RightHandWeapon != null)
		{
			var rightRanged = weaponController.RightHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (rightRanged != null && rightRanged.WeaponAmmoType == type)
			{
				RightWeaponAmmoMagazineText.text = newMagazineAmount.ToString();
				return;
			}
		}

		if (weaponController.LeftHandWeapon != null)
		{
			var leftRanged = weaponController.LeftHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (leftRanged != null && leftRanged.WeaponAmmoType == type)
			{
				LeftWeaponAmmoMagazineText.text = newMagazineAmount.ToString();
				return;
			}
		}
	}

	public void ShowRightWeaponAmmo()
	{
		RightWeaponAmmoMagazine.SetActive(true);
		RightWeaponAmmoReserve.SetActive(true);
		RightWeaponAmmoSeparator.SetActive(true);
	}

	public void HideRightWeaponAmmo()
	{
		RightWeaponAmmoMagazine.SetActive(false);
		RightWeaponAmmoReserve.SetActive(false);
		RightWeaponAmmoSeparator.SetActive(false);
	}

	public void ShowLeftWeaponAmmo()
	{
		LeftWeaponAmmoMagazine.SetActive(true);
		LeftWeaponAmmoReserve.SetActive(true);
		LeftWeaponAmmoSeparator.SetActive(true);
	}

	public void HideLeftWeaponAmmo()
	{
		LeftWeaponAmmoMagazine.SetActive(false);
		LeftWeaponAmmoReserve.SetActive(false);
		LeftWeaponAmmoSeparator.SetActive(false);
	}
}
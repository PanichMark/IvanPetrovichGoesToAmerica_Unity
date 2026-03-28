using UnityEngine;

public class CanvasHUDammoController : MonoBehaviour
{
	private MenuManager menuManager;
	private GameObject canvasHUDammo;
	private GameSceneManager gameSceneManager;
	private GameController gameController;
	private WeaponController weaponController;

	private GameObject RightWeaponAmmoMagazine;
	private GameObject RightWeaponAmmoReserve;
	private GameObject RightWeaponAmmoSeparator;
	private GameObject LeftWeaponAmmoMagazine;
	private GameObject LeftWeaponAmmoReserve;
	private GameObject LeftWeaponAmmoSeparator;

	public void Initialize(GameSceneManager gameSceneManager, GameController gameController, MenuManager menuManager, GameObject canvasHUDPlayerResources, WeaponController weaponController,
			GameObject RightWeaponAmmoMagazine, GameObject RightWeaponAmmoReserve, GameObject RightWeaponAmmoSeparator,
			GameObject LeftWeaponAmmoMagazine, GameObject LeftWeaponAmmoReserve, GameObject LeftWeaponAmmoSeparator)
	{
		this.gameSceneManager = gameSceneManager;
		this.menuManager = menuManager;
		this.canvasHUDammo = canvasHUDPlayerResources;
		this.weaponController = weaponController;

		this.RightWeaponAmmoMagazine = RightWeaponAmmoMagazine;
		this.RightWeaponAmmoReserve = RightWeaponAmmoReserve;
		this.RightWeaponAmmoSeparator = RightWeaponAmmoSeparator;

		this.LeftWeaponAmmoMagazine = LeftWeaponAmmoMagazine;
		this.LeftWeaponAmmoReserve = LeftWeaponAmmoReserve;
		this. LeftWeaponAmmoSeparator = LeftWeaponAmmoSeparator;



		this.menuManager.OnOpenPauseMenu += HideCanvasHUDammo;
		this.menuManager.OnClosePauseMenu += ShowCanvasHUDammo;
		this.menuManager.OnOpenInteractionMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseInteractionMenu += ShowCanvasHUDammo;
		this.menuManager.OnOpenDialogueMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseDialogueMenu += ShowCanvasHUDammo;
		Debug.Log("CanvasHUDammo Initialized");
		this.gameController = gameController;
		this.gameSceneManager.OnBeginLoadMainMenuScene += HideCanvasHUDammo;
		this.gameSceneManager.OnBeginLoadGameplayScene += ShowCanvasHUDammo;

		this.weaponController.OnWeaponChanged += WeaponAmmo;

		this.menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDammo;
	}
	private void ShowCanvasHUDammo()
	{
		if (!menuManager.IsInteractionMenuOpened && !menuManager.IsDialogueMenuOpened && !gameController.IsMainMenuOpen)
		{

			canvasHUDammo.SetActive(true);
			Debug.Log("Show canvasHUDammo");
		}
	}
	public void HideCanvasHUDammo()
	{
		canvasHUDammo.SetActive(false);
		Debug.Log("Hide canvasHUDammo");
	}

	private void WeaponAmmo(string activeHand)
	{
		// Определяем новое активное оружие
		if (activeHand == "left")
		{
			
		}
		else
		{
			
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

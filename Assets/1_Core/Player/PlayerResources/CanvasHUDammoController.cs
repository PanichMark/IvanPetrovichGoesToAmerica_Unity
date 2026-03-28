using TMPro;
using UnityEngine;

public class CanvasHUDammoController : MonoBehaviour
{
	private MenuManager menuManager;
	private GameObject canvasHUDammo;
	private GameSceneManager gameSceneManager;
	private GameController gameController;
	private WeaponController weaponController;
	private PlayerResourcesAmmoManager playerResourcesAmmoManager;

	// Ссылки на родительские объекты панелей
	private GameObject RightWeaponAmmoMagazine;
	private GameObject RightWeaponAmmoReserve;
	private GameObject RightWeaponAmmoSeparator;
	private GameObject LeftWeaponAmmoMagazine;
	private GameObject LeftWeaponAmmoReserve;
	private GameObject LeftWeaponAmmoSeparator;

	// Ссылки на текстовые компоненты
	private TMP_Text RightWeaponAmmoMagazineText;
	private TMP_Text RightWeaponAmmoReserveText;
	private TMP_Text LeftWeaponAmmoMagazineText;
	private TMP_Text LeftWeaponAmmoReserveText;

	public void Initialize(GameSceneManager gameSceneManager, GameController gameController, MenuManager menuManager, GameObject canvasHUDPlayerResources, WeaponController weaponController,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		GameObject RightWeaponAmmoMagazine, GameObject RightWeaponAmmoReserve, GameObject RightWeaponAmmoSeparator,
		GameObject LeftWeaponAmmoMagazine, GameObject LeftWeaponAmmoReserve, GameObject LeftWeaponAmmoSeparator)
	{
		this.gameSceneManager = gameSceneManager;
		this.menuManager = menuManager;
		this.canvasHUDammo = canvasHUDPlayerResources;
		this.weaponController = weaponController;
		this.playerResourcesAmmoManager = playerResourcesAmmoManager;
		this.gameController	= gameController;

		this.RightWeaponAmmoMagazine = RightWeaponAmmoMagazine;
		this.RightWeaponAmmoReserve = RightWeaponAmmoReserve;
		this.RightWeaponAmmoSeparator = RightWeaponAmmoSeparator;
		this.LeftWeaponAmmoMagazine = LeftWeaponAmmoMagazine;
		this.LeftWeaponAmmoReserve = LeftWeaponAmmoReserve;
		this.LeftWeaponAmmoSeparator = LeftWeaponAmmoSeparator;

		// Получаем компоненты TextMeshPro из переданных GameObjects
		RightWeaponAmmoMagazineText = RightWeaponAmmoMagazine.GetComponent<TMP_Text>();
		RightWeaponAmmoReserveText = RightWeaponAmmoReserve.GetComponent<TMP_Text>();
		LeftWeaponAmmoMagazineText = LeftWeaponAmmoMagazine.GetComponent<TMP_Text>();
		LeftWeaponAmmoReserveText = LeftWeaponAmmoReserve.GetComponent<TMP_Text>();

		// Подписка на события меню
		this.menuManager.OnOpenPauseMenu += HideCanvasHUDammo;
		this.menuManager.OnClosePauseMenu += ShowCanvasHUDammo;
		this.menuManager.OnOpenInteractionMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseInteractionMenu += ShowCanvasHUDammo;
		this.menuManager.OnOpenDialogueMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseDialogueMenu += ShowCanvasHUDammo;




		// Подписка на события сцен
		this.gameSceneManager.OnBeginLoadMainMenuScene += HideCanvasHUDammo;
		this.gameSceneManager.OnBeginLoadGameplayScene += ShowCanvasHUDammo;

		// Подписка на игровые события
		this.menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDammo;
		this.menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDammo;

		// НОВЫЕ ПОДПИСКИ (раздельные)
		this.playerResourcesAmmoManager.OnReserveAmmoChanged += UpdateReserveDisplay;
		this.playerResourcesAmmoManager.OnMagazineAmmoChanged += UpdateMagazineDisplay;

		this.weaponController.OnWeaponChanged += UpdateAmmoDisplayForActiveWeapon;
		

		// Скрываем панели по умолчанию
		HideRightWeaponAmmo();
		HideLeftWeaponAmmo();

		Debug.Log("CanvasHUDammo Initialized");
	}

	private void ShowCanvasHUDammo()
	{
		if (!menuManager.IsInteractionMenuOpened && !menuManager.IsDialogueMenuOpened && !gameController.IsMainMenuOpen)
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
		// Логика теперь нужна и для показа UI, И для обновления цифр при переключении оружия.
		if (activeHand == "left")
		{
			var ranged = weaponController.LeftHandWeapon?.GetComponent<RangedWeaponAbstract>();
			if (weaponController.LeftHandWeapon != null && ranged != null)
			{
				ShowLeftWeaponAmmo();

				// --- ДОБАВЬТЕ ЭТИ СТРОЧКИ ---
				// Обновляем текст магазина из оружия
				LeftWeaponAmmoMagazineText.text = ranged.MagazineAmmoCurrent.ToString();
				
				// Обновляем текст запаса из менеджера
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
		else // Правая рука
		{
			var ranged = weaponController.RightHandWeapon?.GetComponent<RangedWeaponAbstract>();
			if (weaponController.RightHandWeapon != null && ranged != null)
			{
				ShowRightWeaponAmmo();

				// --- ДОБАВЬТЕ ЭТИ СТРОЧКИ ---
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


	// Этот метод вызывается, когда меняется ТОЛЬКО общий запас (например, подобрали патроны)
	private void UpdateReserveDisplay(AmmoTypes type, int newTotalAmount)
	{
		// Проверяем правую руку
		if (weaponController.RightHandWeapon != null)
		{
			var rightRanged = weaponController.RightHandWeapon.GetComponent<RangedWeaponAbstract>();
			if (rightRanged != null && rightRanged.WeaponAmmoType == type)
			{
				RightWeaponAmmoReserveText.text = newTotalAmount.ToString();
				return;
			}
		}

		// Проверяем левую руку
		if (weaponController.LeftHandWeapon != null)
		{
			var leftRanged = weaponController.LeftHandWeapon.GetComponent<RangedWeaponAbstract>();
			if (leftRanged != null && leftRanged.WeaponAmmoType == type)
			{
				LeftWeaponAmmoReserveText.text = newTotalAmount.ToString();
				return;
			}
		}
	}

	// Этот метод вызывается, когда меняется ТОЛЬКО магазин (выстрел или перезарядка)
	private void UpdateMagazineDisplay(AmmoTypes type, int newMagazineAmount)
	{
		// Проверяем правую руку
		if (weaponController.RightHandWeapon != null)
		{
			var rightRanged = weaponController.RightHandWeapon.GetComponent<RangedWeaponAbstract>();
			if (rightRanged != null && rightRanged.WeaponAmmoType == type)
			{
				RightWeaponAmmoMagazineText.text = newMagazineAmount.ToString();
				return;
			}
		}

		// Проверяем левую руку
		if (weaponController.LeftHandWeapon != null)
		{
			var leftRanged = weaponController.LeftHandWeapon.GetComponent<RangedWeaponAbstract>();
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
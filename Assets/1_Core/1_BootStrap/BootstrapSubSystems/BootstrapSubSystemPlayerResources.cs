using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubSystemPlayerResources
{
	private Bootstrap bootstrap;

	private GameObject playerResourcesGameObject;
	private CanvasHUDhealthAndManaController canvasHUDhealthAndManaController;
	public CanvasHUDammoController canvasHUDammoController { get; private set; }
	private GameObject canvasHUDhealthAndMana;
	private GameObject canvasHUDammo;

	private PlayerResourcesMoneyManager playerResourcesMoneyManager;
	private TMP_Text playerMoneyTextGameObject;

	private PlayerResourcesHealthManager playerResourcesHealthManager;
	private Slider HealthBarSlider;
	private Button HealingItemButton;
	private TextMeshProUGUI HealingItemNumber;

	private PlayerResourcesManaManager playerResourcesManaManager;
	private Slider ManaBarSlider;
	private Button ManaReplenishtemButton;
	private TextMeshProUGUI ManaReplenishItemNumber;

	public PlayerResourcesAmmoManager playerResourcesAmmoManager { get; private set; }
	public GameObject RightWeaponAmmoMagazine { get; private set; }
	public GameObject RightWeaponAmmoReserve { get; private set; }
	public GameObject RightWeaponAmmoSeparator { get; private set; }
	public GameObject LeftWeaponAmmoMagazine { get; private set; }
	public GameObject LeftWeaponAmmoReserve { get; private set; }
	public GameObject LeftWeaponAmmoSeparator { get; private set; }

	private GameObject canvasPauseMenu;
	private GameObject canvasMenuWeaponWheel;
	private BootstrapSubSystemScene bootstrapSubSystemScene;
	private GameController gameController;
	private BootstrapSubSystemMenu bootstrapSubSystemMenu;
	private BootstrapSubSystemPlayerSystems bootstrapSubSystemPlayerSystems;

	public BootstrapSubSystemPlayerResources(
		GameObject canvasPauseMenu,
		GameObject canvasMenuWeaponWheel,
		BootstrapSubSystemScene bootstrapSubSystemScene,
		GameController gameController,
		BootstrapSubSystemMenu bootstrapSubSystemMenu,
		BootstrapSubSystemPlayerSystems bootstrapSubSystemPlayerSystems,
		GameObject canvasHUDhealthAndMana,
		Bootstrap bootstrap,
		GameObject canvasHUDammo)
	{
		this.canvasPauseMenu = canvasPauseMenu;
		this.canvasMenuWeaponWheel = canvasMenuWeaponWheel;
		this.bootstrapSubSystemScene = bootstrapSubSystemScene;
		this.gameController = gameController;
		this.bootstrapSubSystemMenu = bootstrapSubSystemMenu;
		this.bootstrapSubSystemPlayerSystems = bootstrapSubSystemPlayerSystems;
		this.canvasHUDhealthAndMana = canvasHUDhealthAndMana;
		this.bootstrap = bootstrap;
		this.canvasHUDammo = canvasHUDammo;
	}

	public IEnumerator InitializePlayerResources()
	{
		playerResourcesGameObject = new GameObject("PlayerResources");

		playerMoneyTextGameObject = canvasPauseMenu.transform.Find("PauseMenu PlayerMoneyNumber").GetComponent<TMP_Text>();
		HealthBarSlider = canvasHUDhealthAndMana.transform.Find("Health Slider").GetComponent<Slider>();
		HealingItemButton = bootstrap.FindDeepChildByName(canvasMenuWeaponWheel, "HealingItemButton").GetComponent<Button>();
		HealingItemNumber = bootstrap.FindDeepChildByName(canvasMenuWeaponWheel, "HealingItemsNumber").GetComponent<TextMeshProUGUI>();
		ManaBarSlider = canvasHUDhealthAndMana.transform.Find("Mana Slider").GetComponent<Slider>();
		ManaReplenishtemButton = bootstrap.FindDeepChildByName(canvasMenuWeaponWheel, "ManaReplenishItemButton ").GetComponent<Button>();
		ManaReplenishItemNumber = bootstrap.FindDeepChildByName(canvasMenuWeaponWheel, "ManaReplenishItemsNumber").GetComponent<TextMeshProUGUI>();

		canvasHUDhealthAndManaController = playerResourcesGameObject.AddComponent<CanvasHUDhealthAndManaController>();
		canvasHUDammoController = playerResourcesGameObject.AddComponent<CanvasHUDammoController>();
		playerResourcesMoneyManager = playerResourcesGameObject.AddComponent<PlayerResourcesMoneyManager>();
		playerResourcesHealthManager = playerResourcesGameObject.AddComponent<PlayerResourcesHealthManager>();
		playerResourcesManaManager = playerResourcesGameObject.AddComponent<PlayerResourcesManaManager>();
		playerResourcesAmmoManager = playerResourcesGameObject.AddComponent<PlayerResourcesAmmoManager>();

		canvasHUDhealthAndManaController.Initialize(bootstrapSubSystemScene.gameSceneManager, gameController, bootstrapSubSystemMenu.menuManager, canvasHUDhealthAndMana);
		playerResourcesMoneyManager.Initialize(playerMoneyTextGameObject);
		playerResourcesHealthManager.Initialize(gameController, bootstrapSubSystemPlayerSystems.playerBehaviour, HealthBarSlider, HealingItemButton, HealingItemNumber);
		playerResourcesManaManager.Initialize(ManaBarSlider, ManaReplenishtemButton, ManaReplenishItemNumber);

		RightWeaponAmmoMagazine = canvasHUDammo.transform.Find("RightWeaponAmmoMagazine").gameObject;
		RightWeaponAmmoReserve = canvasHUDammo.transform.Find("RightWeaponAmmoReserve").gameObject;
		RightWeaponAmmoSeparator = canvasHUDammo.transform.Find("RightWeaponAmmoSeparator").gameObject;
		LeftWeaponAmmoMagazine = canvasHUDammo.transform.Find("LeftWeaponAmmoMagazine").gameObject;
		LeftWeaponAmmoReserve = canvasHUDammo.transform.Find("LeftWeaponAmmoReserve").gameObject;
		LeftWeaponAmmoSeparator = canvasHUDammo.transform.Find("LeftWeaponAmmoSeparator").gameObject;

		ServiceLocator.Register("PlayerResourcesMoneyManager", playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesHealthManager", playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", playerResourcesManaManager);
		ServiceLocator.Register("playerResourcesAmmoManager", playerResourcesAmmoManager);

		Debug.Log("PLAYER RESOURCES INITIALIZED");

		yield break;
	}
}
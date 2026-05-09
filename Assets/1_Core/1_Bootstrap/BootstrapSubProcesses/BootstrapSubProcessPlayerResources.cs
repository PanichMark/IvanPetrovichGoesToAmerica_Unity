using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubProcessPlayerResources
{
	private Bootstrap _bootstrap;

	private GameObject _playerResourcesGameObject;
	private CanvasHUDhealthAndManaController _canvasHUDhealthAndManaController;
	public CanvasHUDammoController CanvasHUDammoController { get; private set; }
	private GameObject _canvasHUDhealthAndMana;
	private GameObject _canvasHUDammo;

	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;
	private TMP_Text _playerMoneyTextGameObject;

	private PlayerResourcesHealthManager _playerResourcesHealthManager;
	private Slider _sliderHealthBar;
	private Button _buttonUseHealingItem;
	private TextMeshProUGUI _textHealingItemNumber;

	private PlayerResourcesManaManager _playerResourcesManaManager;
	private Slider _sliderManaBar;
	private Button _buttonUseManaReplenishItem;
	private TextMeshProUGUI _textManaReplenishItemNumber;

	public PlayerResourcesAmmoManager PlayerResourcesAmmoManager { get; private set; }
	public GameObject RightWeaponAmmoMagazine { get; private set; }
	public GameObject RightWeaponAmmoReserve { get; private set; }
	public GameObject RightWeaponAmmoSeparator { get; private set; }
	public GameObject LeftWeaponAmmoMagazine { get; private set; }
	public GameObject LeftWeaponAmmoReserve { get; private set; }
	public GameObject LeftWeaponAmmoSeparator { get; private set; }

	private GameObject _canvasPauseMenu;
	private GameObject _canvasMenuWeaponWheel;
	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;
	private GameController _gameController;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;

	public BootstrapSubProcessPlayerResources(
		GameObject canvasPauseMenu,
		GameObject canvasMenuWeaponWheel,
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		GameController gameController,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		GameObject canvasHUDhealthAndMana,
		Bootstrap bootstrap,
		GameObject canvasHUDammo)
	{
		_canvasPauseMenu = canvasPauseMenu;
		_canvasMenuWeaponWheel = canvasMenuWeaponWheel;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_gameController = gameController;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_bootstrapSubProcessPlayerSystems = bootstrapSubProcessPlayerSystems;
		_canvasHUDhealthAndMana = canvasHUDhealthAndMana;
		_bootstrap = bootstrap;
		_canvasHUDammo = canvasHUDammo;
	}

	public IEnumerator InitializePlayerResources()
	{
		_playerResourcesGameObject = new GameObject("PlayerResources");

		_playerMoneyTextGameObject = _canvasPauseMenu.transform.Find("PauseMenu PlayerMoneyNumber").GetComponent<TMP_Text>();
		_sliderHealthBar = _canvasHUDhealthAndMana.transform.Find("Health Slider").GetComponent<Slider>();
		_buttonUseHealingItem = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "HealingItemButton").GetComponent<Button>();
		_textHealingItemNumber = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "HealingItemsNumber").GetComponent<TextMeshProUGUI>();
		_sliderManaBar = _canvasHUDhealthAndMana.transform.Find("Mana Slider").GetComponent<Slider>();
		_buttonUseManaReplenishItem = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "ManaReplenishItemButton ").GetComponent<Button>();
		_textManaReplenishItemNumber = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "ManaReplenishItemsNumber").GetComponent<TextMeshProUGUI>();

		_canvasHUDhealthAndManaController = _playerResourcesGameObject.AddComponent<CanvasHUDhealthAndManaController>();
		CanvasHUDammoController = _playerResourcesGameObject.AddComponent<CanvasHUDammoController>();
		_playerResourcesMoneyManager = _playerResourcesGameObject.AddComponent<PlayerResourcesMoneyManager>();
		_playerResourcesHealthManager = _playerResourcesGameObject.AddComponent<PlayerResourcesHealthManager>();
		_playerResourcesManaManager = _playerResourcesGameObject.AddComponent<PlayerResourcesManaManager>();
		PlayerResourcesAmmoManager = _playerResourcesGameObject.AddComponent<PlayerResourcesAmmoManager>();

		_canvasHUDhealthAndManaController.Initialize(_bootstrapSubProcessSceneSystem.gameSceneManager, _gameController, _bootstrapSubProcessMenuSystem.MenuManager, _canvasHUDhealthAndMana);
		_playerResourcesMoneyManager.Initialize(_playerMoneyTextGameObject);
		_playerResourcesHealthManager.Initialize(_gameController, _bootstrapSubProcessPlayerSystems.PlayerBehaviour, _sliderHealthBar, _buttonUseHealingItem, _textHealingItemNumber);
		_playerResourcesManaManager.Initialize(_sliderManaBar, _buttonUseManaReplenishItem, _textManaReplenishItemNumber);

		RightWeaponAmmoMagazine = _canvasHUDammo.transform.Find("RightWeaponAmmoMagazine").gameObject;
		RightWeaponAmmoReserve = _canvasHUDammo.transform.Find("RightWeaponAmmoReserve").gameObject;
		RightWeaponAmmoSeparator = _canvasHUDammo.transform.Find("RightWeaponAmmoSeparator").gameObject;
		LeftWeaponAmmoMagazine = _canvasHUDammo.transform.Find("LeftWeaponAmmoMagazine").gameObject;
		LeftWeaponAmmoReserve = _canvasHUDammo.transform.Find("LeftWeaponAmmoReserve").gameObject;
		LeftWeaponAmmoSeparator = _canvasHUDammo.transform.Find("LeftWeaponAmmoSeparator").gameObject;

		ServiceLocator.Register("PlayerResourcesMoneyManager", _playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesHealthManager", _playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", _playerResourcesManaManager);
		ServiceLocator.Register("playerResourcesAmmoManager", PlayerResourcesAmmoManager);

		Debug.Log("PLAYER RESOURCES INITIALIZED");

		yield break;
	}
}
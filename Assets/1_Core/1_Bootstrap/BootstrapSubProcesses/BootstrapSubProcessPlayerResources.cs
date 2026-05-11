using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubProcessPlayerResources
{
	private Bootstrap _bootstrap;

	private GameController _gameController;
	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;

	private GameObject _canvasPauseMenu;
	private GameObject _canvasMenuWeaponWheel;

	private GameObject _gameObjectBootstrapPlayerResources;

	private CanvasHUDhealthAndManaController _canvasHUDhealthAndManaController;
	private GameObject _canvasHUDhealthAndMana;

	private PlayerResourcesHealthManager _playerResourcesHealthManager;
	private Slider _sliderHealthBar;
	private Button _buttonUseHealingItem;
	private TextMeshProUGUI _textHealingItemNumber;

	private PlayerResourcesManaManager _playerResourcesManaManager;
	private Slider _sliderManaBar;
	private Button _buttonUseManaReplenishItem;
	private TextMeshProUGUI _textManaReplenishItemNumber;

	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;
	private TMP_Text _textPlayerMoneyNumber;

	public PlayerResourcesAmmoManager PlayerResourcesAmmoManager { get; private set; }
	public CanvasHUDammoController CanvasHUDammoController { get; private set; }
	private GameObject _canvasHUDammo;
	public GameObject TextRightWeaponAmmoMagazineNumber { get; private set; }
	public GameObject TextRightWeaponAmmoReserveNumber { get; private set; }
	public GameObject RightWeaponAmmoSeparator { get; private set; }
	public GameObject TextLeftWeaponAmmoMagazineNumber { get; private set; }
	public GameObject TextLeftWeaponAmmoReserveNumber { get; private set; }
	public GameObject LeftWeaponAmmoSeparator { get; private set; }

	public BootstrapSubProcessPlayerResources(
		Bootstrap bootstrap,
		GameController gameController,
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		GameObject canvasPauseMenu,
		GameObject canvasHUDhealthAndMana,
		GameObject canvasHUDammo,
		GameObject canvasMenuWeaponWheel)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_bootstrapSubProcessPlayerSystems = bootstrapSubProcessPlayerSystems;
		_canvasPauseMenu = canvasPauseMenu;
		_canvasHUDhealthAndMana = canvasHUDhealthAndMana;
		_canvasHUDammo = canvasHUDammo;
		_canvasMenuWeaponWheel = canvasMenuWeaponWheel;
	}

	public IEnumerator InitializePlayerResources()
	{
		_gameObjectBootstrapPlayerResources = new GameObject("Bootstrap_PlayerResources");

		_playerResourcesHealthManager = _gameObjectBootstrapPlayerResources.AddComponent<PlayerResourcesHealthManager>();
		_playerResourcesManaManager = _gameObjectBootstrapPlayerResources.AddComponent<PlayerResourcesManaManager>();
		_canvasHUDhealthAndManaController = _gameObjectBootstrapPlayerResources.AddComponent<CanvasHUDhealthAndManaController>();
		_playerResourcesMoneyManager = _gameObjectBootstrapPlayerResources.AddComponent<PlayerResourcesMoneyManager>();
		PlayerResourcesAmmoManager = _gameObjectBootstrapPlayerResources.AddComponent<PlayerResourcesAmmoManager>();
		CanvasHUDammoController = _gameObjectBootstrapPlayerResources.AddComponent<CanvasHUDammoController>();

		_sliderHealthBar = _canvasHUDhealthAndMana.transform.Find("SliderHealthBar").GetComponent<Slider>();
		_buttonUseHealingItem = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "ButtonUseHealingItem").GetComponent<Button>();
		_textHealingItemNumber = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "TextHealingItemNumber").GetComponent<TextMeshProUGUI>();
		_sliderManaBar = _canvasHUDhealthAndMana.transform.Find("SliderManaBar").GetComponent<Slider>();
		_buttonUseManaReplenishItem = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "ButtonUseManaReplenishItem").GetComponent<Button>();
		_textManaReplenishItemNumber = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "TextManaReplenishItemNumber").GetComponent<TextMeshProUGUI>();
		_textPlayerMoneyNumber = _canvasPauseMenu.transform.Find("TextPlayerMoneyNumber").GetComponent<TMP_Text>();

		TextRightWeaponAmmoMagazineNumber = _canvasHUDammo.transform.Find("TextRightWeaponAmmoMagazineNumber").gameObject;
		TextRightWeaponAmmoReserveNumber = _canvasHUDammo.transform.Find("TextRightWeaponAmmoReserveNumber").gameObject;
		RightWeaponAmmoSeparator = _canvasHUDammo.transform.Find("RightWeaponAmmoSeparator").gameObject;
		TextLeftWeaponAmmoMagazineNumber = _canvasHUDammo.transform.Find("TextLeftWeaponAmmoMagazineNumber").gameObject;
		TextLeftWeaponAmmoReserveNumber = _canvasHUDammo.transform.Find("TextLeftWeaponAmmoReserveNumber").gameObject;
		LeftWeaponAmmoSeparator = _canvasHUDammo.transform.Find("LeftWeaponAmmoSeparator").gameObject;

		_playerResourcesHealthManager.Initialize(_gameController, _bootstrapSubProcessPlayerSystems.PlayerBehaviour, _sliderHealthBar, _buttonUseHealingItem, _textHealingItemNumber);
		_playerResourcesManaManager.Initialize(_sliderManaBar, _buttonUseManaReplenishItem, _textManaReplenishItemNumber);
		_canvasHUDhealthAndManaController.Initialize(_gameController, _bootstrapSubProcessSceneSystem.GameSceneManager, _bootstrapSubProcessMenuSystem.MenuManager, _canvasHUDhealthAndMana);
		_playerResourcesMoneyManager.Initialize(_textPlayerMoneyNumber);
		PlayerResourcesAmmoManager.Initialize();

		ServiceLocator.Register("PlayerResourcesHealthManager", _playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", _playerResourcesManaManager);
		ServiceLocator.Register("PlayerResourcesMoneyManager", _playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesAmmoManager", PlayerResourcesAmmoManager);

		Debug.Log("PLAYER RESOURCES INITIALIZED");
		yield break;
	}
}
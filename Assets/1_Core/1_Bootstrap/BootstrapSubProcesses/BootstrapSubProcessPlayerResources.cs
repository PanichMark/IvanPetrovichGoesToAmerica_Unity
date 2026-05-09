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
	private TMP_Text _textPlayerMoneyNumber;

	private PlayerResourcesHealthManager _playerResourcesHealthManager;
	private Slider _sliderHealthBar;
	private Button _buttonUseHealingItem;
	private TextMeshProUGUI _textHealingItemNumber;

	private PlayerResourcesManaManager _playerResourcesManaManager;
	private Slider _sliderManaBar;
	private Button _buttonUseManaReplenishItem;
	private TextMeshProUGUI _textManaReplenishItemNumber;

	public PlayerResourcesAmmoManager PlayerResourcesAmmoManager { get; private set; }
	public GameObject TextRightWeaponAmmoMagazineNumber { get; private set; }
	public GameObject TextRightWeaponAmmoReserveNumber { get; private set; }
	public GameObject RightWeaponAmmoSeparator { get; private set; }
	public GameObject TextLeftWeaponAmmoMagazineNumber { get; private set; }
	public GameObject TextLeftWeaponAmmoReserveNumber { get; private set; }
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

		_textPlayerMoneyNumber = _canvasPauseMenu.transform.Find("TextPlayerMoneyNumber").GetComponent<TMP_Text>();
		_sliderHealthBar = _canvasHUDhealthAndMana.transform.Find("SliderHealthBar").GetComponent<Slider>();
		_buttonUseHealingItem = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "ButtonUseHealingItem").GetComponent<Button>();
		_textHealingItemNumber = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "TextHealingItemNumber").GetComponent<TextMeshProUGUI>();
		_sliderManaBar = _canvasHUDhealthAndMana.transform.Find("SliderManaBar").GetComponent<Slider>();
		_buttonUseManaReplenishItem = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "ButtonUseManaReplenishItem").GetComponent<Button>();
		_textManaReplenishItemNumber = _bootstrap.FindDeepGameObject(_canvasMenuWeaponWheel, "TextManaReplenishItemNumber").GetComponent<TextMeshProUGUI>();

		_canvasHUDhealthAndManaController = _playerResourcesGameObject.AddComponent<CanvasHUDhealthAndManaController>();
		CanvasHUDammoController = _playerResourcesGameObject.AddComponent<CanvasHUDammoController>();
		_playerResourcesMoneyManager = _playerResourcesGameObject.AddComponent<PlayerResourcesMoneyManager>();
		_playerResourcesHealthManager = _playerResourcesGameObject.AddComponent<PlayerResourcesHealthManager>();
		_playerResourcesManaManager = _playerResourcesGameObject.AddComponent<PlayerResourcesManaManager>();
		PlayerResourcesAmmoManager = _playerResourcesGameObject.AddComponent<PlayerResourcesAmmoManager>();

		_canvasHUDhealthAndManaController.Initialize(_bootstrapSubProcessSceneSystem.GameSceneManager, _gameController, _bootstrapSubProcessMenuSystem.MenuManager, _canvasHUDhealthAndMana);
		_playerResourcesMoneyManager.Initialize(_textPlayerMoneyNumber);
		_playerResourcesHealthManager.Initialize(_gameController, _bootstrapSubProcessPlayerSystems.PlayerBehaviour, _sliderHealthBar, _buttonUseHealingItem, _textHealingItemNumber);
		_playerResourcesManaManager.Initialize(_sliderManaBar, _buttonUseManaReplenishItem, _textManaReplenishItemNumber);

		TextRightWeaponAmmoMagazineNumber = _canvasHUDammo.transform.Find("TextRightWeaponAmmoMagazineNumber").gameObject;
		TextRightWeaponAmmoReserveNumber = _canvasHUDammo.transform.Find("TextRightWeaponAmmoReserveNumber").gameObject;
		RightWeaponAmmoSeparator = _canvasHUDammo.transform.Find("RightWeaponAmmoSeparator").gameObject;
		TextLeftWeaponAmmoMagazineNumber = _canvasHUDammo.transform.Find("TextLeftWeaponAmmoMagazineNumber").gameObject;
		TextLeftWeaponAmmoReserveNumber = _canvasHUDammo.transform.Find("TextLeftWeaponAmmoReserveNumber").gameObject;
		LeftWeaponAmmoSeparator = _canvasHUDammo.transform.Find("LeftWeaponAmmoSeparator").gameObject;

		ServiceLocator.Register("PlayerResourcesMoneyManager", _playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesHealthManager", _playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", _playerResourcesManaManager);
		ServiceLocator.Register("PlayerResourcesAmmoManager", PlayerResourcesAmmoManager);

		Debug.Log("PLAYER RESOURCES INITIALIZED");

		yield break;
	}
}
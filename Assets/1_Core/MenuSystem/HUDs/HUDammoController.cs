using TMPro;
using UnityEngine;

public class HUDammoController : MonoBehaviour
{
	private MenuManager _menuManager;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private GameObject _canvasHUDammo;
	private GameSceneManager _gameSceneManager;
	private GameController _gameController;
	private PlayerWeaponController _playerWeaponController;
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private PlayerBehaviourController _playerBehaviour;
	private InteractionController _interactionController;

	private GameObject _rightWeaponAmmoMagazine;
	private GameObject _rightWeaponAmmoReserve;
	private GameObject _rightWeaponAmmoSeparator;
	private GameObject _leftWeaponAmmoMagazine;
	private GameObject _leftWeaponAmmoReserve;
	private GameObject _leftWeaponAmmoSeparator;

	private TMP_Text _rightWeaponAmmoMagazineText;
	private TMP_Text _rightWeaponAmmoReserveText;
	private TMP_Text _leftWeaponAmmoMagazineText;
	private TMP_Text _leftWeaponAmmoReserveText;

	private GameObject _HUDammo;

	public void Initialize(
		GameController gameController,
		GameSceneManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		PlayerBehaviourController playerBehaviour,
		PlayerWeaponController weaponController,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		InteractionController interactionController,
		GameObject canvasHUDammo,
		ViewModelHUDAmmo viewModelHUDAmmo)
	{
		_gameSceneManager = gameSceneManager;
		_menuManager = menuManager;
		_pauseSubMenuSettingsSectionGeneralController = pauseSubMenuSettingsSectionGeneralController;
		_canvasHUDammo = canvasHUDammo;
		_playerWeaponController = weaponController;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;
		_gameController	= gameController;
		_playerBehaviour = playerBehaviour;
		_interactionController = interactionController;

		_rightWeaponAmmoMagazine = viewModelHUDAmmo.TextRightWeaponAmmoMagazineNumber;
		_rightWeaponAmmoReserve = viewModelHUDAmmo.TextRightWeaponAmmoReserveNumber;
		_rightWeaponAmmoSeparator = viewModelHUDAmmo.RightWeaponAmmoSeparator;
		_leftWeaponAmmoMagazine = viewModelHUDAmmo.TextLeftWeaponAmmoMagazineNumber;
		_leftWeaponAmmoReserve = viewModelHUDAmmo.TextLeftWeaponAmmoReserveNumber;
		_leftWeaponAmmoSeparator = viewModelHUDAmmo.LeftWeaponAmmoSeparator;

		_rightWeaponAmmoMagazineText = _rightWeaponAmmoMagazine.GetComponent<TMP_Text>();
		_rightWeaponAmmoReserveText = _rightWeaponAmmoReserve.GetComponent<TMP_Text>();
		_leftWeaponAmmoMagazineText = _leftWeaponAmmoMagazine.GetComponent<TMP_Text>();
		_leftWeaponAmmoReserveText = _leftWeaponAmmoReserve.GetComponent<TMP_Text>();

		_HUDammo = viewModelHUDAmmo.HUDammo;

		_menuManager.OnOpenPauseMenu += HideCanvasHUDammo;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDammo;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDammo;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDammo;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDammo;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDammo;

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowAmmoDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += HideAmmoDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += ShowAmmoDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideAmmoDisplay;

		_playerBehaviour.OnPlayerArmed += ShowCanvasHUDammo;
		_playerBehaviour.OnPlayerDisarmed += HideCanvasHUDammo;

		_gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasHUDammo;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDammo;

		_menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDammo;
		_menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDammo;
	
		_playerResourcesAmmoManager.OnReserveAmmoChanged += UpdateReserveDisplay;
		_playerResourcesAmmoManager.OnMagazineAmmoChanged += UpdateMagazineDisplay;

		_playerWeaponController.OnWeaponChanged += UpdateAmmoDisplayForActiveWeapon;

		_interactionController.OnPickUpThrowable += HideRightWeaponAmmo;
		_interactionController.OnGetRidOfThrowable += () => 
		{
			if (_playerWeaponController.RightHandWeapon != null)
			{
				ShowRightWeaponAmmo();
			}
		};

		HideRightWeaponAmmo();
		HideLeftWeaponAmmo();

		Debug.Log("HUDammoController Initialized");
	}

	private void ShowCanvasHUDammo()
	{
		if (!_menuManager.IsInteractionMenuOpened && !_menuManager.IsDialogueMenuOpened && !_gameController.IsMainMenuOpen && !_menuManager.IsWeaponWheelMenuOpened && !_menuManager.IsMainMenuBeingLoaded && _playerBehaviour.IsPlayerArmed)
		{
			_canvasHUDammo.SetActive(true);

			Debug.Log("Show canvasAmmo");
		}
	}

	public void HideCanvasHUDammo()
	{
		_canvasHUDammo.SetActive(false);

		Debug.Log("Hide canvasAmmo");
	}

	private void ShowAmmoDisplay()
	{
		_HUDammo.SetActive(true);
	}

	private void HideAmmoDisplay()
	{
		_HUDammo.SetActive(false);
	}

	private void UpdateAmmoDisplayForActiveWeapon(WeaponHandsEnum activeHand)
	{
		if (activeHand == WeaponHandsEnum.HandLeft)
		{
			var ranged = _playerWeaponController.LeftHandWeapon?.GetComponent<WeaponRangedAbstract>();
			if (_playerWeaponController.LeftHandWeapon != null && ranged != null)
			{
				ShowLeftWeaponAmmo();

				_leftWeaponAmmoMagazineText.text = ranged.PlayerMagazineAmmoCurrent.ToString();

				if (_playerResourcesAmmoManager.AmmoDictionary.TryGetValue(ranged.PlayerWeaponAmmoType, out var ammoData))
				{
					_leftWeaponAmmoReserveText.text = ammoData.AmmoReserve.ToString();
				}
			}
			else
			{
				HideLeftWeaponAmmo();
			}
		}
		if (activeHand == WeaponHandsEnum.HandRight)
		{
			var ranged = _playerWeaponController.RightHandWeapon?.GetComponent<WeaponRangedAbstract>();
			if (_playerWeaponController.RightHandWeapon != null && ranged != null)
			{
				ShowRightWeaponAmmo();

				_rightWeaponAmmoMagazineText.text = ranged.PlayerMagazineAmmoCurrent.ToString();

				if (_playerResourcesAmmoManager.AmmoDictionary.TryGetValue(ranged.PlayerWeaponAmmoType, out var ammoData))
				{
					_rightWeaponAmmoReserveText.text = ammoData.AmmoReserve.ToString();
				}
			}
			else
			{
				HideRightWeaponAmmo();
			}
		}

		if(_playerWeaponController.RightHandWeapon == null)
		{
			HideRightWeaponAmmo();
		}
		if (_playerWeaponController.LeftHandWeapon == null)
		{
			HideLeftWeaponAmmo();
		}
	}

	private void UpdateReserveDisplay(AmmoTypes type, int newTotalAmount)
	{
		if (_playerWeaponController.RightHandWeapon != null)
		{
			var rightRanged = _playerWeaponController.RightHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (rightRanged != null && rightRanged.PlayerWeaponAmmoType == type)
			{
				_rightWeaponAmmoReserveText.text = newTotalAmount.ToString();
			}
		}

		if (_playerWeaponController.LeftHandWeapon != null)
		{
			var leftRanged = _playerWeaponController.LeftHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (leftRanged != null && leftRanged.PlayerWeaponAmmoType == type)
			{
				_leftWeaponAmmoReserveText.text = newTotalAmount.ToString();
			}
		}
	}

	private void UpdateMagazineDisplay(WeaponNames weaponType, AmmoTypes ammoType, int newAmount)
	{
		if (_playerWeaponController.RightHandWeapon != null)
		{
			var rightComp = _playerWeaponController.RightHandWeapon.GetComponent<WeaponAbstract>();
			if (rightComp != null && rightComp.WeaponName == weaponType)
			{
				_rightWeaponAmmoMagazineText.text = newAmount.ToString();
				return;
			}
		}

		if (_playerWeaponController.LeftHandWeapon != null)
		{
			var leftComp = _playerWeaponController.LeftHandWeapon.GetComponent<WeaponAbstract>();
			if (leftComp != null && leftComp.WeaponName == weaponType)
			{
				_leftWeaponAmmoMagazineText.text = newAmount.ToString();
				return;
			}
		}
	}

	public void ShowRightWeaponAmmo()
	{
		_rightWeaponAmmoMagazine.SetActive(true);
		_rightWeaponAmmoReserve.SetActive(true);
		_rightWeaponAmmoSeparator.SetActive(true);
	}

	public void HideRightWeaponAmmo()
	{
		_rightWeaponAmmoMagazine.SetActive(false);
		_rightWeaponAmmoReserve.SetActive(false);
		_rightWeaponAmmoSeparator.SetActive(false);
	}

	public void ShowLeftWeaponAmmo()
	{
		_leftWeaponAmmoMagazine.SetActive(true);
		_leftWeaponAmmoReserve.SetActive(true);
		_leftWeaponAmmoSeparator.SetActive(true);
	}

	public void HideLeftWeaponAmmo()
	{
		_leftWeaponAmmoMagazine.SetActive(false);
		_leftWeaponAmmoReserve.SetActive(false);
		_leftWeaponAmmoSeparator.SetActive(false);
	}
}
using TMPro;
using UnityEngine;

public class HUDammoController : MonoBehaviour
{
	private MenuManager _menuManager;
	private GameObject _canvasHUDammo;
	private GameSceneManager _gameSceneManager;
	private GameController _gameController;
	private PlayerWeaponController _playerWeaponController;
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private PlayerBehaviourController _playerBehaviour;

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

	public void Initialize(
		GameController gameController,
		GameSceneManager gameSceneManager,
		MenuManager menuManager,
		PlayerBehaviourController playerBehaviour,
		PlayerWeaponController weaponController,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		GameObject canvasHUDammo,
		GameObject RightWeaponAmmoMagazine,
		GameObject RightWeaponAmmoReserve,
		GameObject RightWeaponAmmoSeparator,
		GameObject LeftWeaponAmmoMagazine,
		GameObject LeftWeaponAmmoReserve,
		GameObject LeftWeaponAmmoSeparator)
	{
		_gameSceneManager = gameSceneManager;
		_menuManager = menuManager;
		_canvasHUDammo = canvasHUDammo;
		_playerWeaponController = weaponController;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;
		_gameController	= gameController;
		_playerBehaviour = playerBehaviour;

		_rightWeaponAmmoMagazine = RightWeaponAmmoMagazine;
		_rightWeaponAmmoReserve = RightWeaponAmmoReserve;
		_rightWeaponAmmoSeparator = RightWeaponAmmoSeparator;
		_leftWeaponAmmoMagazine = LeftWeaponAmmoMagazine;
		_leftWeaponAmmoReserve = LeftWeaponAmmoReserve;
		_leftWeaponAmmoSeparator = LeftWeaponAmmoSeparator;

		_rightWeaponAmmoMagazineText = RightWeaponAmmoMagazine.GetComponent<TMP_Text>();
		_rightWeaponAmmoReserveText = RightWeaponAmmoReserve.GetComponent<TMP_Text>();
		_leftWeaponAmmoMagazineText = LeftWeaponAmmoMagazine.GetComponent<TMP_Text>();
		_leftWeaponAmmoReserveText = LeftWeaponAmmoReserve.GetComponent<TMP_Text>();

		_menuManager.OnOpenPauseMenu += HideCanvasHUDammo;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDammo;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDammo;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDammo;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDammo;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDammo;


		_playerBehaviour.OnPlayerArmed += ShowCanvasHUDammo;
		_playerBehaviour.OnPlayerDisarmed += HideCanvasHUDammo;

		_gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasHUDammo;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDammo;

		_menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDammo;
		_menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDammo;

		_playerResourcesAmmoManager.OnReserveAmmoChanged += UpdateReserveDisplay;
		_playerResourcesAmmoManager.OnMagazineAmmoChanged += UpdateMagazineDisplay;

		_playerWeaponController.OnWeaponChanged += UpdateAmmoDisplayForActiveWeapon;

		HideRightWeaponAmmo();
		HideLeftWeaponAmmo();

		Debug.Log("CanvasHUDammoController Initialized");
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

	private void UpdateAmmoDisplayForActiveWeapon(string activeHand)
	{
		if (activeHand == "left")
		{
			var ranged = _playerWeaponController.LeftHandWeapon?.GetComponent<WeaponRangedAbstract>();
			if (_playerWeaponController.LeftHandWeapon != null && ranged != null)
			{
				ShowLeftWeaponAmmo();

				_leftWeaponAmmoMagazineText.text = ranged.PlayerMagazineAmmoCurrent.ToString();

				if (_playerResourcesAmmoManager.AmmoDictionary.TryGetValue(ranged.PlayerWeaponAmmoType, out var ammoData))
				{
					_leftWeaponAmmoReserveText.text = ammoData.TotalAmmoCurrent.ToString();
				}
			}
			else
			{
				HideLeftWeaponAmmo();
			}
		}
		else 
		{
			var ranged = _playerWeaponController.RightHandWeapon?.GetComponent<WeaponRangedAbstract>();
			if (_playerWeaponController.RightHandWeapon != null && ranged != null)
			{
				ShowRightWeaponAmmo();

				_rightWeaponAmmoMagazineText.text = ranged.PlayerMagazineAmmoCurrent.ToString();

				if (_playerResourcesAmmoManager.AmmoDictionary.TryGetValue(ranged.PlayerWeaponAmmoType, out var ammoData))
				{
					_rightWeaponAmmoReserveText.text = ammoData.TotalAmmoCurrent.ToString();
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
				return;
			}
		}

		if (_playerWeaponController.LeftHandWeapon != null)
		{
			var leftRanged = _playerWeaponController.LeftHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (leftRanged != null && leftRanged.PlayerWeaponAmmoType == type)
			{
				_leftWeaponAmmoReserveText.text = newTotalAmount.ToString();
				return;
			}
		}
	}

	private void UpdateMagazineDisplay(AmmoTypes type, int newMagazineAmount)
	{
		if (_playerWeaponController.RightHandWeapon != null)
		{
			var rightRanged = _playerWeaponController.RightHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (rightRanged != null && rightRanged.PlayerWeaponAmmoType == type)
			{
				_rightWeaponAmmoMagazineText.text = newMagazineAmount.ToString();
				return;
			}
		}

		if (_playerWeaponController.LeftHandWeapon != null)
		{
			var leftRanged = _playerWeaponController.LeftHandWeapon.GetComponent<WeaponRangedAbstract>();
			if (leftRanged != null && leftRanged.PlayerWeaponAmmoType == type)
			{
				_leftWeaponAmmoMagazineText.text = newMagazineAmount.ToString();
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
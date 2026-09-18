using TMPro;
using UnityEngine;
using System.Collections;

public class HUDweaponsController : MonoBehaviour, IJsonSaveLoad
{
	private MenuManager _menuManager;
	private PauseSubMenuSettingsSectionGeneralController _pauseSubMenuSettingsSectionGeneralController;
	private GameObject _canvasHUDammo;
	private GameScenesManager _gameSceneManager;
	private GameController _gameController;
	private PlayerWeaponController _playerWeaponController;
	private PlayerWeaponAmmoController _playerResourcesAmmoManager;
	private PlayerBehaviourController _playerBehaviour;
	private PlayerInteractionController _interactionController;

	private GameObject _HUDammo;

	private GameObject _rightWeaponAmmoMagazine;
	private GameObject _rightWeaponAmmoReserve;
	private GameObject _rightWeaponAmmoBox;
	private GameObject _leftWeaponAmmoMagazine;
	private GameObject _leftWeaponAmmoReserve;
	private GameObject _leftWeaponAmmoBox;

	private TMP_Text _rightWeaponAmmoMagazineText;
	private TMP_Text _rightWeaponAmmoReserveText;
	private TMP_Text _leftWeaponAmmoMagazineText;
	private TMP_Text _leftWeaponAmmoReserveText;

	private GameObject _HUDcrosshairs;

	private GameObject _crosshairRevolver;

	private GameObject _crosshairAutoPistol;
	private GameObject[] _listCrosshairPartsAutoPistol = new GameObject[4];

	private GameObject _crosshairShotgun;
	private GameObject[] _listCrosshairPartsShotgun = new GameObject[4];

	private GameObject _crosshairTranquilizer;

	public void Initialize(
		GameController gameController,
		GameScenesManager gameSceneManager,
		MenuManager menuManager,
		PauseSubMenuSettingsSectionGeneralController pauseSubMenuSettingsSectionGeneralController,
		PlayerBehaviourController playerBehaviour,
		PlayerWeaponController weaponController,
		PlayerWeaponAmmoController playerResourcesAmmoManager,
		PlayerInteractionController interactionController,
		GameObject canvasHUDammo,
		ViewModelHUDWeapons viewModelHUDWeapons)
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

		_HUDammo = viewModelHUDWeapons.HUDammo;

		_rightWeaponAmmoMagazine = viewModelHUDWeapons.TextRightWeaponAmmoMagazineNumber;
		_rightWeaponAmmoReserve = viewModelHUDWeapons.TextRightWeaponAmmoReserveNumber;
		_rightWeaponAmmoBox = viewModelHUDWeapons.RightWeaponAmmoBox;
		_leftWeaponAmmoMagazine = viewModelHUDWeapons.TextLeftWeaponAmmoMagazineNumber;
		_leftWeaponAmmoReserve = viewModelHUDWeapons.TextLeftWeaponAmmoReserveNumber;
		_leftWeaponAmmoBox = viewModelHUDWeapons.LeftWeaponAmmoBox;

		_rightWeaponAmmoMagazineText = _rightWeaponAmmoMagazine.GetComponent<TMP_Text>();
		_rightWeaponAmmoReserveText = _rightWeaponAmmoReserve.GetComponent<TMP_Text>();
		_leftWeaponAmmoMagazineText = _leftWeaponAmmoMagazine.GetComponent<TMP_Text>();
		_leftWeaponAmmoReserveText = _leftWeaponAmmoReserve.GetComponent<TMP_Text>();

		_HUDcrosshairs = viewModelHUDWeapons.HUDcrosshiars;

		_crosshairRevolver = viewModelHUDWeapons.CrosshairRevolver;

		_crosshairAutoPistol = viewModelHUDWeapons.CrosshairAutoPistol;
		_listCrosshairPartsAutoPistol = viewModelHUDWeapons.ListCrosshairPartsAutoPistol;

		_crosshairShotgun = viewModelHUDWeapons.CrosshairShotgun;
		_listCrosshairPartsShotgun = viewModelHUDWeapons.ListCrosshairPartsShotgun;

		_crosshairTranquilizer = viewModelHUDWeapons.CrosshairTranquilizer;

		_menuManager.OnOpenPauseMenu += HideCanvasHUDammo;
		_menuManager.OnClosePauseMenu += ShowCanvasHUDammo;
		_menuManager.OnOpenInteractionMenu += HideCanvasHUDammo;
		_menuManager.OnCloseInteractionMenu += ShowCanvasHUDammo;
		_menuManager.OnOpenDialogueMenu += HideCanvasHUDammo;
		_menuManager.OnCloseDialogueMenu += ShowCanvasHUDammo;

		_pauseSubMenuSettingsSectionGeneralController.OnHUDfull += ShowHUDWeaponDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesOnly += HideHUDWeaponDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDdialoguesHide += ShowHUDWeaponDisplay;
		_pauseSubMenuSettingsSectionGeneralController.OnHUDturnOff += HideHUDWeaponDisplay;

		_playerBehaviour.OnPlayerArmed += ShowCanvasHUDammo;
		_playerBehaviour.OnPlayerDisarmed += HideCanvasHUDammo;

		_gameSceneManager.OnBeginLoadingMainMenuScene += HideCanvasHUDammo;
		_gameSceneManager.OnBeginLoadingGameplayScene += ShowCanvasHUDammo;

		_menuManager.OnOpenWeaponWheelMenu += HideCanvasHUDammo;
		_menuManager.OnCloseWeaponWheelMenu += ShowCanvasHUDammo;
	
		_playerResourcesAmmoManager.OnReserveAmmoChanged += UpdateReserveDisplay;
		_playerResourcesAmmoManager.OnMagazineAmmoChanged += UpdateMagazineDisplay;

		_playerWeaponController.OnWeaponChanged += UpdateAmmoDisplayForActiveWeapon;

		_interactionController.OnPickUpThrowable += (InteractionObjectsPickableTypes pickableType) => { HideRightWeaponAmmo(); };
		_interactionController.OnGetRidOfThrowable += () => 
		{
			if (_playerWeaponController.RightHandWeapon != null && _playerWeaponController.RightHandWeaponComponent is WeaponRangedAbstract)
			{
				ShowRightWeaponAmmo();
			}
		};

		_playerWeaponController.OnShowWeapon += ShowWeaponCrosshair;
		_playerWeaponController.OnHideWeapon += HideWeaponCrosshair;

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

	private void ShowHUDWeaponDisplay()
	{
		_HUDammo.SetActive(true);
		_HUDcrosshairs.SetActive(true);
	}

	private void HideHUDWeaponDisplay()
	{
		_HUDammo.SetActive(false);
		_HUDcrosshairs.SetActive(false);
	}

	private void UpdateAmmoDisplayForActiveWeapon(WeaponHandType activeHand)
	{
		if (activeHand == WeaponHandType.Left)
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
		if (activeHand == WeaponHandType.Right)
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

	private void UpdateMagazineDisplay(PlayerWeaponNames weaponType, AmmoTypes ammoType, int newAmount)
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
		_rightWeaponAmmoBox.SetActive(true);
	}

	public void HideRightWeaponAmmo()
	{
		_rightWeaponAmmoBox.SetActive(false);
	}

	public void ShowLeftWeaponAmmo()
	{

		_leftWeaponAmmoBox.SetActive(true);
	}

	public void HideLeftWeaponAmmo()
	{
		_leftWeaponAmmoBox.SetActive(false);
	}

	public void ShowWeaponCrosshair(WeaponAbstract WeaponName)
	{
		if (WeaponName.WeaponName == PlayerWeaponNames.Revolver)
		{
			_crosshairRevolver.SetActive(true);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.AutoPistol)
		{
			_crosshairAutoPistol.SetActive(true);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Shotgun)
		{
			_crosshairShotgun.SetActive(true);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Tranquilizer)
		{
			_crosshairTranquilizer.SetActive(true);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Crossbow)
		{

		}
	}

	public void HideWeaponCrosshair(WeaponAbstract WeaponName)
	{
		if (WeaponName.WeaponName == PlayerWeaponNames.Revolver)
		{
			_crosshairRevolver.SetActive(false);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.AutoPistol)
		{
			_crosshairAutoPistol.SetActive(false);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Shotgun)
		{
			_crosshairShotgun.SetActive(false);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Tranquilizer)
		{
			_crosshairTranquilizer.SetActive(false);
		}
		if (WeaponName.WeaponName == PlayerWeaponNames.Crossbow)
		{

		}
	}

	public void AnimateWeaponCrosshairOnShoot(WeaponAbstract WeaponName)
	{
		if (WeaponName.WeaponName == PlayerWeaponNames.AutoPistol)
		{
			StartCoroutine(AnimateAutoPistolCrosshair());
		}
		else if (WeaponName.WeaponName == PlayerWeaponNames.Shotgun)
		{
			StartCoroutine(AnimateShotgunCrosshair());
		}
	}

	private IEnumerator AnimateAutoPistolCrosshair()
	{
		var offsets = new Vector2[4]
		{
			new Vector2(25f, -25f),
			new Vector2(-25f, -25f),
			new Vector2(-25f, 25f),
			new Vector2(25f, 25f)
		};

		for (int i = 0; i < _listCrosshairPartsAutoPistol.Length; i++)
		{
			var part = _listCrosshairPartsAutoPistol[i];
			var startPos = part.transform.localPosition;
			part.transform.localPosition = new Vector3(startPos.x + offsets[i].x, startPos.y + offsets[i].y, startPos.z);
		}

		yield return new WaitForSeconds(0.1f);

		for (int i = 0; i < _listCrosshairPartsAutoPistol.Length; i++)
		{
			var part = _listCrosshairPartsAutoPistol[i];
			var startPos = part.transform.localPosition;
			part.transform.localPosition = new Vector3(startPos.x - offsets[i].x, startPos.y - offsets[i].y, startPos.z);
		}
	}

	private IEnumerator AnimateShotgunCrosshair()
	{
		var offsets = new Vector2[4]
		{
			new Vector2(100f, 0f),
			new Vector2(0f, -100f),
			new Vector2(-100f, 0f),
			new Vector2(0f, 100f)
		};

		for (int i = 0; i < _listCrosshairPartsShotgun.Length; i++)
		{
			var part = _listCrosshairPartsShotgun[i];
			var startPos = part.transform.localPosition;
			part.transform.localPosition = new Vector3(startPos.x + offsets[i].x, startPos.y + offsets[i].y, startPos.z);
		}

		yield return new WaitForSeconds(0.1f);

		for (int i = 0; i < _listCrosshairPartsShotgun.Length; i++)
		{
			var part = _listCrosshairPartsShotgun[i];
			var startPos = part.transform.localPosition;
			part.transform.localPosition = new Vector3(startPos.x - offsets[i].x, startPos.y - offsets[i].y, startPos.z);
		}
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		if (_playerWeaponController != null && _playerWeaponController.RightHandWeapon != null)
		{
			UpdateAmmoDisplayForActiveWeapon(WeaponHandType.Right);
		}

		if (_playerWeaponController != null && _playerWeaponController.LeftHandWeapon != null)
		{
			UpdateAmmoDisplayForActiveWeapon(WeaponHandType.Left);
		}

		yield return null;
	}
}
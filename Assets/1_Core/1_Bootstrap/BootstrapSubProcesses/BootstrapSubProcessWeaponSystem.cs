using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubProcessWeaponSystem
{
	private GameObject _weaponSystemGameObject;
	public PlayerWeaponController WeaponController { get; private set; }
	private LegKickAttack _legKickAttack;
	private WeaponAnimationController _weaponAnimationController;
	private WeaponFirstPersonRender _weaponFirstPersonRender;
	private GameObject _firstPersonLeftHandWeaponSlotGameObject;
	private GameObject _firstPersonRightHandWeaponSlotGameObject;
	private GameObject _thirdPersonLeftHandWeaponSlotGameObject;
	private GameObject _thirdPersonRightHandWeaponSlotGameObject;
	private WeaponWheelMenuController _weaponWheelController;
	private GameObject _canvasMenuWeaponWheel;
	private GameObject _weaponWheelSegmentPrefab;
	private TextMeshProUGUI _textWeaponWheelWeapon;
	private TextMeshProUGUI _textWeaponWheelHandType;
	private Image _imageWeaponWheelWeapon;
	private GameObject _textChokeNPC;

	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;
	private GameController _gameController;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private IInputDevice _inputDevice;
	private GameObject _playerGameObject;
	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;
	private CanvasHUDammoController _canvasHUDammoController;
	private GameObject _canvasHUDammo;
	private GameObject _rightWeaponAmmoMagazine;
	private GameObject _rightWeaponAmmoReserve;
	private GameObject _rightWeaponAmmoSeparator;
	private GameObject _leftWeaponAmmoMagazine;
	private GameObject _leftWeaponAmmoReserve;
	private GameObject _leftWeaponAmmoSeparator;

	public BootstrapSubProcessWeaponSystem(
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		GameController gameController,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		IInputDevice inputDevice,
		GameObject playerGameObject,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		BootstrapSubProcessInteractionSystem bootstrapSubSystemInteraction,
		CanvasHUDammoController canvasHUDammoController,
		GameObject canvasHUDammo,
		GameObject RightWeaponAmmoMagazine,
		GameObject RightWeaponAmmoReserve,
		GameObject RightWeaponAmmoSeparator,
		GameObject LeftWeaponAmmoMagazine,
		GameObject LeftWeaponAmmoReserve,
		GameObject LeftWeaponAmmoSeparator,
		GameObject canvasMenuWeaponWheel)
	{
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_gameController = gameController;
		_bootstrapSubProcessPlayerSystems = bootstrapSubProcessPlayerSystems;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_inputDevice = inputDevice;
		_playerGameObject = playerGameObject;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;
		_bootstrapSubProcessInteractionSystem = bootstrapSubSystemInteraction;
		_canvasHUDammoController = canvasHUDammoController;
		_canvasHUDammo = canvasHUDammo;

		_rightWeaponAmmoMagazine = RightWeaponAmmoMagazine;
		_rightWeaponAmmoReserve = RightWeaponAmmoReserve;
		_rightWeaponAmmoSeparator = RightWeaponAmmoSeparator;

		_leftWeaponAmmoMagazine = LeftWeaponAmmoMagazine;
		_leftWeaponAmmoReserve = LeftWeaponAmmoReserve;
		_leftWeaponAmmoSeparator = LeftWeaponAmmoSeparator;

		_canvasMenuWeaponWheel = canvasMenuWeaponWheel;
	}

	public IEnumerator InitializeWeaponSystem()
	{
		_weaponSystemGameObject = new GameObject("WeaponSystem");

		WeaponController = _weaponSystemGameObject.AddComponent<PlayerWeaponController>();
		_legKickAttack = _weaponSystemGameObject.AddComponent<LegKickAttack>();
		_weaponWheelController = _weaponSystemGameObject.AddComponent<WeaponWheelMenuController>();
		_weaponAnimationController = _weaponSystemGameObject.AddComponent<WeaponAnimationController>();
		_weaponFirstPersonRender = _weaponSystemGameObject.AddComponent<WeaponFirstPersonRender>();

		_weaponWheelSegmentPrefab = Resources.Load<GameObject>("WeaponWheelButtons/WeaponWheelSegmentPrefab");

		_textWeaponWheelWeapon = _canvasMenuWeaponWheel.transform.Find("TextWeaponWheelWeapon").GetComponent<TextMeshProUGUI>();
		_textWeaponWheelHandType = _canvasMenuWeaponWheel.transform.Find("TextWeaponWheelHandType").GetComponent<TextMeshProUGUI>();
		_imageWeaponWheelWeapon = _canvasMenuWeaponWheel.transform.Find("ImageWeaponWheelWeapon").GetComponent<Image>();

		_firstPersonLeftHandWeaponSlotGameObject = GameObject.Find("FirstPersonWeaponSlot.L");
		_thirdPersonLeftHandWeaponSlotGameObject = GameObject.Find("ThirdPersonWeaponSlot.L");
		_firstPersonRightHandWeaponSlotGameObject = GameObject.Find("FirstPersonWeaponSlot.R");
		_thirdPersonRightHandWeaponSlotGameObject = GameObject.Find("ThirdPersonWeaponSlot.R");

		_textChokeNPC = _canvasHUDammo.transform.Find("TextChokeNPC").gameObject;

		WeaponController.Initialize(_inputDevice, _bootstrapSubProcessMenuSystem.MenuManager, _bootstrapSubProcessPlayerSystems.PlayerBehaviour, _bootstrapSubProcessInteractionSystem.InteractionController);
		_legKickAttack.Initialize(_inputDevice, _playerGameObject, _bootstrapSubProcessPlayerSystems.PlayerMovementController);
	
		_weaponWheelController.Initialize(
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			WeaponController,
			_weaponWheelSegmentPrefab,
			_canvasMenuWeaponWheel,
			_textWeaponWheelWeapon,
			_textWeaponWheelHandType,
			_imageWeaponWheelWeapon);

		_weaponAnimationController.Initialize(
			_playerGameObject,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerCameraController,
			WeaponController,
			_legKickAttack);

		_weaponFirstPersonRender.Initialize(
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessPlayerSystems.PlayerCameraController,
			WeaponController,
			_bootstrapSubProcessPlayerSystems.PlayerFirstPersonHandRightGameObject,
			_bootstrapSubProcessPlayerSystems.PlayerFirstPersonHandLeftGameObject,
			_bootstrapSubProcessPlayerSystems.PlayerThirdPersonHandRightParentGameObject,
			_bootstrapSubProcessPlayerSystems.PlayerThirdPersonHandLeftParentGameObject);

		_canvasHUDammoController.Initialize(
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_gameController,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_canvasHUDammo,
			WeaponController,
			_playerResourcesAmmoManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_rightWeaponAmmoMagazine,
			_rightWeaponAmmoReserve,
			_rightWeaponAmmoSeparator,
			_leftWeaponAmmoMagazine,
			_leftWeaponAmmoReserve,
			_leftWeaponAmmoSeparator);

		ServiceLocator.Register("WeaponController", WeaponController);
		ServiceLocator.Register("FirstPersonLeftHandWeaponSlotGameObject", _firstPersonLeftHandWeaponSlotGameObject);
		ServiceLocator.Register("FirstPersonRightHandWeaponSlotGameObject", _firstPersonRightHandWeaponSlotGameObject);
		ServiceLocator.Register("ThirdPersonLeftHandWeaponSlotGameObject", _thirdPersonLeftHandWeaponSlotGameObject);
		ServiceLocator.Register("ThirdPersonRightHandWeaponSlotGameObject", _thirdPersonRightHandWeaponSlotGameObject);
		ServiceLocator.Register("TextChokeNPC", _textChokeNPC);

		Debug.Log("WEAPON SYSTEM INITIALIZED");

		yield break;
	}
}
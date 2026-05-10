using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubProcessWeaponSystem
{
	private GameObject _GameObjectBootstrapWeaponSystem;
	public PlayerWeaponController WeaponController { get; private set; }
	private LegKickAttackController _legKickAttackController;
	private WeaponAnimationController _weaponAnimationController;
	private WeaponFirstPersonRender _weaponFirstPersonRender;
	private GameObject _gameObjectFirstPersonLeftHandWeaponSlot;
	private GameObject _gameObjectFirstPersonRightHandWeaponSlot;
	private GameObject _gameObjectThirdPersonLeftHandWeaponSlot;
	private GameObject _gameObjectThirdPersonRightHandWeaponSlot;
	private WeaponWheelMenuController _weaponWheelController;
	private GameObject _canvasMenuWeaponWheel;
	private GameObject _gameObjectWeaponWheelSegment;
	private TextMeshProUGUI _textWeaponWheelWeapon;
	private TextMeshProUGUI _textWeaponWheelHandType;
	private Image _imageWeaponWheelIconWeapon;
	private GameObject _textChokeNPC;

	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;
	private GameController _gameController;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private IInputDevice _inputDevice;
	private GameObject _gameObjectPlayer;
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
		_gameObjectPlayer = playerGameObject;
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
		_GameObjectBootstrapWeaponSystem = new GameObject("Bootstrap_WeaponSystem");

		WeaponController = _GameObjectBootstrapWeaponSystem.AddComponent<PlayerWeaponController>();
		_weaponWheelController = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponWheelMenuController>();
		_weaponAnimationController = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponAnimationController>();
		_weaponFirstPersonRender = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponFirstPersonRender>();
		_legKickAttackController = _GameObjectBootstrapWeaponSystem.AddComponent<LegKickAttackController>();

		_gameObjectWeaponWheelSegment = Resources.Load<GameObject>("WeaponWheel/WeaponWheelSegment");

		_textWeaponWheelWeapon = _canvasMenuWeaponWheel.transform.Find("TextWeaponWheelWeapon").GetComponent<TextMeshProUGUI>();
		_textWeaponWheelHandType = _canvasMenuWeaponWheel.transform.Find("TextWeaponWheelHandType").GetComponent<TextMeshProUGUI>();
		_imageWeaponWheelIconWeapon = _canvasMenuWeaponWheel.transform.Find("ImageWeaponWheelWeapon").GetComponent<Image>();

		_gameObjectFirstPersonLeftHandWeaponSlot = GameObject.Find("FirstPersonWeaponSlot.L");
		_gameObjectThirdPersonLeftHandWeaponSlot = GameObject.Find("ThirdPersonWeaponSlot.L");
		_gameObjectFirstPersonRightHandWeaponSlot = GameObject.Find("FirstPersonWeaponSlot.R");
		_gameObjectThirdPersonRightHandWeaponSlot = GameObject.Find("ThirdPersonWeaponSlot.R");

		_textChokeNPC = _canvasHUDammo.transform.Find("TextChokeNPC").gameObject;

		WeaponController.Initialize(_inputDevice, _bootstrapSubProcessMenuSystem.MenuManager, _bootstrapSubProcessPlayerSystems.PlayerBehaviour, _bootstrapSubProcessInteractionSystem.InteractionController);
		_legKickAttackController.Initialize(_inputDevice, _gameObjectPlayer, _bootstrapSubProcessPlayerSystems.PlayerMovementController);
	
		_weaponWheelController.Initialize(
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			WeaponController,
			_gameObjectWeaponWheelSegment,
			_canvasMenuWeaponWheel,
			_textWeaponWheelWeapon,
			_textWeaponWheelHandType,
			_imageWeaponWheelIconWeapon);

		_weaponAnimationController.Initialize(
			_gameObjectPlayer,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerCameraController,
			WeaponController,
			_legKickAttackController);

		_weaponFirstPersonRender.Initialize(
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessPlayerSystems.PlayerCameraController,
			WeaponController,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandRight,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandLeft,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandRight,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandLeft);

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
		ServiceLocator.Register("FirstPersonLeftHandWeaponSlotGameObject", _gameObjectFirstPersonLeftHandWeaponSlot);
		ServiceLocator.Register("FirstPersonRightHandWeaponSlotGameObject", _gameObjectFirstPersonRightHandWeaponSlot);
		ServiceLocator.Register("ThirdPersonLeftHandWeaponSlotGameObject", _gameObjectThirdPersonLeftHandWeaponSlot);
		ServiceLocator.Register("ThirdPersonRightHandWeaponSlotGameObject", _gameObjectThirdPersonRightHandWeaponSlot);
		ServiceLocator.Register("TextChokeNPC", _textChokeNPC);

		Debug.Log("WEAPON SYSTEM INITIALIZED");

		yield break;
	}
}
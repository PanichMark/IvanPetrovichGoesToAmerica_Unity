using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubProcessWeaponSystem
{
	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;

	private GameObject _GameObjectBootstrapWeaponSystem;

	public PlayerWeaponController WeaponController { get; private set; }
	private GameObject _gameObjectFirstPersonRightHandWeaponSlot;
	private GameObject _gameObjectFirstPersonLeftHandWeaponSlot;
	private GameObject _gameObjectThirdPersonRightHandWeaponSlot;
	private GameObject _gameObjectThirdPersonLeftHandWeaponSlot;

	private WeaponWheelMenuController _weaponWheelController;
	private GameObject _canvasMenuWeaponWheel;
	private GameObject _gameObjectWeaponWheelSegment;
	private TextMeshProUGUI _textWeaponWheelWeaponName;
	private Image _imageWeaponWheelWeaponIcon;
	private TextMeshProUGUI _textWeaponWheelHandType;

	private WeaponAnimationController _weaponAnimationController;

	private WeaponFirstPersonRender _weaponFirstPersonRender;

	private LegKickAttackController _legKickAttackController;
	private GameObject _gameObjectPlayer;

	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private HUDammoController _HUDammoController;
	private GameObject _canvasHUDammo;
	private GameObject _rightWeaponAmmoMagazine;
	private GameObject _rightWeaponAmmoReserve;
	private GameObject _rightWeaponAmmoSeparator;
	private GameObject _leftWeaponAmmoMagazine;
	private GameObject _leftWeaponAmmoReserve;
	private GameObject _leftWeaponAmmoSeparator;
	private GameObject _textChokeNPC;

	public BootstrapSubProcessWeaponSystem(
		GameController gameController,
		IInputDevice inputDevice,
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		BootstrapSubProcessInteractionSystem bootstrapSubSystemInteraction,
		GameObject canvasMenuWeaponWheel,
		GameObject playerGameObject,
		PlayerResourcesAmmoManager playerResourcesAmmoManager,
		GameObject canvasHUDammo,
		GameObject RightWeaponAmmoMagazine,
		GameObject RightWeaponAmmoReserve,
		GameObject RightWeaponAmmoSeparator,
		GameObject LeftWeaponAmmoMagazine,
		GameObject LeftWeaponAmmoReserve,
		GameObject LeftWeaponAmmoSeparator)
	{
		_gameController = gameController;
		_inputDevice = inputDevice;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_bootstrapSubProcessPlayerSystems = bootstrapSubProcessPlayerSystems;
		_bootstrapSubProcessInteractionSystem = bootstrapSubSystemInteraction;
		_canvasMenuWeaponWheel = canvasMenuWeaponWheel;
		_gameObjectPlayer = playerGameObject;
		_playerResourcesAmmoManager = playerResourcesAmmoManager;
		_canvasHUDammo = canvasHUDammo;
		_rightWeaponAmmoMagazine = RightWeaponAmmoMagazine;
		_rightWeaponAmmoReserve = RightWeaponAmmoReserve;
		_rightWeaponAmmoSeparator = RightWeaponAmmoSeparator;
		_leftWeaponAmmoMagazine = LeftWeaponAmmoMagazine;
		_leftWeaponAmmoReserve = LeftWeaponAmmoReserve;
		_leftWeaponAmmoSeparator = LeftWeaponAmmoSeparator;
	}

	public IEnumerator InitializeWeaponSystem()
	{
		_GameObjectBootstrapWeaponSystem = new GameObject("Bootstrap_WeaponSystem");

		WeaponController = _GameObjectBootstrapWeaponSystem.AddComponent<PlayerWeaponController>();
		_weaponWheelController = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponWheelMenuController>();
		_weaponAnimationController = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponAnimationController>();
		_weaponFirstPersonRender = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponFirstPersonRender>();
		_legKickAttackController = _GameObjectBootstrapWeaponSystem.AddComponent<LegKickAttackController>();
		_HUDammoController = _GameObjectBootstrapWeaponSystem.AddComponent<HUDammoController>();

		_gameObjectWeaponWheelSegment = Resources.Load<GameObject>("WeaponWheel/WeaponWheelSegment");

		_textWeaponWheelWeaponName = _canvasMenuWeaponWheel.transform.Find("TextWeaponWheelWeapon").GetComponent<TextMeshProUGUI>();
		_imageWeaponWheelWeaponIcon = _canvasMenuWeaponWheel.transform.Find("ImageWeaponWheelWeapon").GetComponent<Image>();
		_textWeaponWheelHandType = _canvasMenuWeaponWheel.transform.Find("TextWeaponWheelHandType").GetComponent<TextMeshProUGUI>();

		_gameObjectFirstPersonRightHandWeaponSlot = GameObject.Find("FirstPersonWeaponSlot.R");
		_gameObjectFirstPersonLeftHandWeaponSlot = GameObject.Find("FirstPersonWeaponSlot.L");
		_gameObjectThirdPersonRightHandWeaponSlot = GameObject.Find("ThirdPersonWeaponSlot.R");
		_gameObjectThirdPersonLeftHandWeaponSlot = GameObject.Find("ThirdPersonWeaponSlot.L");
	
		_textChokeNPC = _canvasHUDammo.transform.Find("TextChokeNPC").gameObject;

		WeaponController.Initialize(
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerResourcesAmmoManager,
			_bootstrapSubProcessInteractionSystem.InteractionController);

		_legKickAttackController.Initialize(
			_inputDevice,
			_bootstrapSubProcessPlayerSystems.PlayerMovementController,
			_bootstrapSubProcessPlayerSystems.PlayerMovementStateMachineController,
			_gameObjectPlayer);
	
		_weaponWheelController.Initialize(
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			WeaponController,
			_canvasMenuWeaponWheel,
			_gameObjectWeaponWheelSegment,
			_textWeaponWheelWeaponName,
			_imageWeaponWheelWeaponIcon,
			_textWeaponWheelHandType);

		_weaponAnimationController.Initialize(
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController,
			WeaponController,
			_legKickAttackController,
			_gameObjectPlayer);

		_weaponFirstPersonRender.Initialize(
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController,
			WeaponController,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandRight,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandLeft,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandRight,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandLeft);

		_HUDammoController.Initialize(
			_gameController,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			WeaponController,
			_playerResourcesAmmoManager,
			_canvasHUDammo,
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
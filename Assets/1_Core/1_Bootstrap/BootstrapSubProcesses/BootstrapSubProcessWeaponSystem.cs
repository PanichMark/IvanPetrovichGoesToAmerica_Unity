using System.Collections;
using UnityEngine;

public class BootstrapSubProcessWeaponSystem
{
	private BootstrapSubProcessSceneSystem _bootstrapSubProcessSceneSystem;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;
	private BootstrapSubProcessPlayerSystems _bootstrapSubProcessPlayerSystems;
	private BootstrapSubProcessInteractionSystem _bootstrapSubProcessInteractionSystem;

	private Bootstrap _bootstrap;
	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private GameObject _GameObjectBootstrapWeaponSystem;
	private GameObject _gameObjectPlayerCamera;
	public PlayerWeaponController WeaponController { get; private set; }
	private GameObject _gameObjectFirstPersonRightHandWeaponSlot;
	private GameObject _gameObjectFirstPersonLeftHandWeaponSlot;
	private GameObject _gameObjectThirdPersonRightHandWeaponSlot;
	private GameObject _gameObjectThirdPersonLeftHandWeaponSlot;

	private WeaponWheelMenuController _weaponWheelController;

	private WeaponAnimationController _weaponAnimationController;

	private WeaponFirstPersonRender _weaponFirstPersonRender;

	private LegKickAttackController _legKickAttackController;
	private GameObject _gameObjectPlayer;

	private PlayerResourcesAmmoManager _playerResourcesAmmoManager;
	private HUDammoController _HUDammoController;


	public BootstrapSubProcessWeaponSystem(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameObject playerGameObject,
		GameObject playerCamera,
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		BootstrapSubProcessInteractionSystem bootstrapSubSystemInteraction)
	{
		_bootstrap = bootstrap;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_gameObjectPlayerCamera = playerCamera;
		_bootstrapSubProcessSceneSystem = bootstrapSubProcessSceneSystem;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_bootstrapSubProcessPlayerSystems = bootstrapSubProcessPlayerSystems;
		_bootstrapSubProcessInteractionSystem = bootstrapSubSystemInteraction;
		_gameObjectPlayer = playerGameObject;
		_playerResourcesAmmoManager = bootstrapSubProcessPlayerSystems.PlayerResourcesAmmoManager;
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

		_gameObjectFirstPersonRightHandWeaponSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "WeaponSlotHand.R");
		_gameObjectFirstPersonLeftHandWeaponSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "WeaponSlotHand.L");
		_gameObjectThirdPersonRightHandWeaponSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "WeaponSlotHand.R");
		_gameObjectThirdPersonLeftHandWeaponSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "WeaponSlotHand.L");

		WeaponController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessMenuSystem.HUDhealthAndManaController,
			_bootstrapSubProcessPlayerSystems.PlayerResourcesAmmoManager,
			_bootstrapSubProcessInteractionSystem.InteractionController);

		_legKickAttackController.Initialize(
		_bootstrap,
		_inputDevice,
		_bootstrapSubProcessPlayerSystems.PlayerMovementController,
		_bootstrapSubProcessPlayerSystems.PlayerMovementStateMachineController,
		_gameObjectPlayer,
		WeaponController);

		_weaponWheelController.Initialize(
			_bootstrap,
			_inputDevice,
			_localizationManager,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerResourcesAmmoManager,
			WeaponController,
			_bootstrapSubProcessMenuSystem.CanvasMenuWeaponWheel,
			_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel);

		_weaponAnimationController.Initialize(
			_bootstrap,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController,
			WeaponController,
			_legKickAttackController,
			_gameObjectPlayer);

		_weaponFirstPersonRender.Initialize(
			_bootstrap,
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
			_bootstrapSubProcessMenuSystem.CanvasHUDammo,
			_bootstrapSubProcessMenuSystem.ViewModelHUDAmmo.TextRightWeaponAmmoMagazineNumber,
			_bootstrapSubProcessMenuSystem.ViewModelHUDAmmo.TextRightWeaponAmmoReserveNumber,
			_bootstrapSubProcessMenuSystem.ViewModelHUDAmmo.RightWeaponAmmoSeparator,
			_bootstrapSubProcessMenuSystem.ViewModelHUDAmmo.TextLeftWeaponAmmoMagazineNumber,
			_bootstrapSubProcessMenuSystem.ViewModelHUDAmmo.TextLeftWeaponAmmoReserveNumber,
			_bootstrapSubProcessMenuSystem.ViewModelHUDAmmo.LeftWeaponAmmoSeparator);

		ServiceLocator.Register("WeaponController", WeaponController);

		ServiceLocator.Register("FirstPersonLeftHandWeaponSlotGameObject", _gameObjectFirstPersonLeftHandWeaponSlot);
		ServiceLocator.Register("FirstPersonRightHandWeaponSlotGameObject", _gameObjectFirstPersonRightHandWeaponSlot);
		ServiceLocator.Register("ThirdPersonLeftHandWeaponSlotGameObject", _gameObjectThirdPersonLeftHandWeaponSlot);
		ServiceLocator.Register("ThirdPersonRightHandWeaponSlotGameObject", _gameObjectThirdPersonRightHandWeaponSlot);

		yield break;
	}
}
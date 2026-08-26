using System.Collections;
using UnityEngine;

public class BootstrapSubProcessWeaponSystem
{
	private BootstrapSubProcessScenesSystem _bootstrapSubProcessSceneSystem;
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

	public PlayerWeaponAmmoController PlayerResourcesAmmoManager { get; private set; }

	private IWeaponWheelMenuController _weaponWheelMenuController;

	private PlayerWeaponAnimationController _weaponAnimationController;

	private PlayerWeaponFirstPersonRenderer _weaponFirstPersonRender;

	private PlayerLegKickAttackController _legKickAttackController;
	private GameObject _gameObjectPlayer;

	private HUDammoController _HUDammoController;

	public BootstrapSubProcessWeaponSystem(
		Bootstrap bootstrap,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameObject playerGameObject,
		GameObject playerCamera,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
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
	}

	public IEnumerator Initialize()
	{
		_GameObjectBootstrapWeaponSystem = new GameObject("Bootstrap_WeaponSystem");

		WeaponController = _GameObjectBootstrapWeaponSystem.AddComponent<PlayerWeaponController>();

		PlayerResourcesAmmoManager = _GameObjectBootstrapWeaponSystem.AddComponent<PlayerWeaponAmmoController>();
		_weaponWheelMenuController = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponWheelMenuController2D>();

		_weaponAnimationController = _GameObjectBootstrapWeaponSystem.AddComponent<PlayerWeaponAnimationController>();
		_weaponFirstPersonRender = _GameObjectBootstrapWeaponSystem.AddComponent<PlayerWeaponFirstPersonRenderer>();
		_legKickAttackController = _GameObjectBootstrapWeaponSystem.AddComponent<PlayerLegKickAttackController>();
		_HUDammoController = _GameObjectBootstrapWeaponSystem.AddComponent<HUDammoController>();

		_gameObjectFirstPersonRightHandWeaponSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "WeaponSlot_Hand.R");
		_gameObjectFirstPersonLeftHandWeaponSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "WeaponSlot_Hand.L");
		_gameObjectThirdPersonRightHandWeaponSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "WeaponSlot_Hand.R");
		_gameObjectThirdPersonLeftHandWeaponSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "WeaponSlot_Hand.L");

		PlayerResourcesAmmoManager.Initialize();

		WeaponController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessMenuSystem.HUDhealthAndManaController,
			PlayerResourcesAmmoManager,
			_bootstrapSubProcessInteractionSystem.InteractionController);

		_legKickAttackController.Initialize(
		_bootstrap,
		_inputDevice,
		_bootstrapSubProcessPlayerSystems.PlayerMovementController,
		_bootstrapSubProcessPlayerSystems.PlayerMovementStateMachineController,
		_gameObjectPlayer,
		WeaponController);

		_weaponWheelMenuController.Initialize(
		_bootstrap,
		_inputDevice,
		_localizationManager,
		_bootstrapSubProcessMenuSystem.MenuManager,
		_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
		_bootstrapSubProcessInteractionSystem.InteractionController,
		PlayerResourcesAmmoManager,
		WeaponController,
		_bootstrapSubProcessMenuSystem.CanvasMenuWeaponWheel,
		_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel,
		_bootstrap.GameObjectPlayerCamera);
		
		_weaponAnimationController.Initialize(
			_bootstrap,
			_gameController,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			_bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController,
			_bootstrapSubProcessInteractionSystem.InteractionController,
			WeaponController,
			_legKickAttackController,
			_bootstrapSubProcessPlayerSystems.TransferBonesFirstPerson,
			_bootstrapSubProcessPlayerSystems.TransferBonesThirdPerson,
			_gameObjectPlayer,
			_gameObjectPlayerCamera);

		_weaponFirstPersonRender.Initialize(
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController,
			_bootstrapSubProcessInteractionSystem.InteractionController,
			WeaponController,
			_weaponAnimationController,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandRight,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandLeft,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandRight,
			_bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandLeft);

		_HUDammoController.Initialize(
			_gameController,
			_bootstrapSubProcessSceneSystem.GameSceneManager,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
			WeaponController,
			PlayerResourcesAmmoManager,
			_bootstrapSubProcessInteractionSystem.InteractionController,
			_bootstrapSubProcessMenuSystem.CanvasHUDammo,
			_bootstrapSubProcessMenuSystem.ViewModelHUDAmmo);

ServiceLocator.Register<PlayerWeaponAmmoController>(PlayerResourcesAmmoManager);
ServiceLocator.Register<PlayerWeaponController	>(WeaponController);
ServiceLocator.Register<PlayerWeaponAnimationController>(_weaponAnimationController);
ServiceLocator.Register<PlayerWeaponFirstPersonRenderer>(_weaponFirstPersonRender);

	ServiceLocator.Register(EnumServiceLocatorGameObjects.FirstPersonLeftHandWeaponSlotGameObject, _gameObjectFirstPersonLeftHandWeaponSlot);
ServiceLocator.Register(EnumServiceLocatorGameObjects.FirstPersonRightHandWeaponSlotGameObject, _gameObjectFirstPersonRightHandWeaponSlot);
ServiceLocator.Register(EnumServiceLocatorGameObjects.ThirdPersonLeftHandWeaponSlotGameObject, _gameObjectThirdPersonLeftHandWeaponSlot);
ServiceLocator.Register(EnumServiceLocatorGameObjects.ThirdPersonRightHandWeaponSlotGameObject, _gameObjectThirdPersonRightHandWeaponSlot);

		yield break;
	}

	public void ChangeWeaponWheelType(WeaponWheelMenuTypes weaponWheelMenuTypes)
	{
		if ((weaponWheelMenuTypes == WeaponWheelMenuTypes._2D) && !(_weaponWheelMenuController is WeaponWheelMenuController2D))
		{
			Object.Destroy(_weaponWheelMenuController as Component);

			_weaponWheelMenuController = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponWheelMenuController2D>();

			_weaponWheelMenuController.Initialize(
				_bootstrap,
				_inputDevice,
				_localizationManager,
				_bootstrapSubProcessMenuSystem.MenuManager,
				_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
				_bootstrapSubProcessInteractionSystem.InteractionController,
				PlayerResourcesAmmoManager,
				WeaponController,
				_bootstrapSubProcessMenuSystem.CanvasMenuWeaponWheel,
				_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel,
				_bootstrap.GameObjectPlayerCamera
			);
		}
		if ((weaponWheelMenuTypes == WeaponWheelMenuTypes._3D) && !(_weaponWheelMenuController is WeaponWheelMenuController3D))
		{
			Object.Destroy(_weaponWheelMenuController as Component);

			_weaponWheelMenuController = _GameObjectBootstrapWeaponSystem.AddComponent<WeaponWheelMenuController3D>();

			_weaponWheelMenuController.Initialize(
				_bootstrap,
				_inputDevice,
				_localizationManager,
				_bootstrapSubProcessMenuSystem.MenuManager,
				_bootstrapSubProcessPlayerSystems.PlayerBehaviour,
				_bootstrapSubProcessInteractionSystem.InteractionController,
				PlayerResourcesAmmoManager,
				WeaponController,
				_bootstrapSubProcessMenuSystem.CanvasMenuWeaponWheel,
				_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel,
				_bootstrap.GameObjectPlayerCamera
			);
		}
	}
}
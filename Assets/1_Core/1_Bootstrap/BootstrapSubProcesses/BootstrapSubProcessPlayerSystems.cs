using System.Collections;
using UnityEngine;

public class BootstrapSubProcessPlayerSystems
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private GameSceneManager _gameSceneManager;

	private GameObject _gameObjectPlayer;
	private GameObject _gameObjectPlayerCollider;
	private GameObject _canvasMenuBackground;

	private GameObject _gameObjectPlayerHead;
	public GameObject GameObjectPlayerThirdPersonHandRight { get; private set; }
	public GameObject GameObjectPlayerThirdPersonHandLeft { get; private set; }
	public GameObject GameObjectPlayerFirstPersonHandRight { get; private set; }
	public GameObject GameObjectPlayerFirstPersonHandLeft { get; private set; }

	public GameObject GameObkectPlayerHatSlot { get; private set; }

	private GameObject _gameObjectPlayerCamera;
	public GameObject PlayerCameraFirstPerson { get; private set; }
	public GameObject PlayerCameraPostProcessing {  get; private set; }
	private GameObject _gameobjectPlayerEyesLookAt;
	public PlayerBehaviourController PlayerBehaviour { get; private set; }
	public PlayerMovementController PlayerMovementController { get; private set; }
	public PlayerMovementStateMachineController PlayerMovementStateMachineController { get; private set; }
	private PlayerColliderController _playerColliderController;
	
	public PlayerCameraController PlayerCameraController { get; private set; }
	public PlayerCameraStateMachineController PlayerCameraStateMachineController { get; private set; }
	private PlayerCameraBlurFilter _playerCameraBlurFilter;
	private PlayerCameraFirstPersonRenderer _playerCameraFirstPersonRender;

	private PlayerMovementAnimationController _playerMovementAnimationController;

	private PlayerResourcesHealthManager _playerResourcesHealthManager;

	private PlayerResourcesManaManager _playerResourcesManaManager;

	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;

	public PlayerResourcesAmmoManager PlayerResourcesAmmoManager { get; private set; }

	public BootstrapSubProcessPlayerSystems(
		Bootstrap bootstrap,
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameController gameController,
		IInputDevice inputDevice,
		GameObject canvasMenuBackground,
		GameObject playerGameObject,
		GameObject playerMainCameraGameObject)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
		_canvasMenuBackground = canvasMenuBackground;
		_gameObjectPlayer = playerGameObject;
		_gameObjectPlayerCamera = playerMainCameraGameObject;
	}

	public IEnumerator InitializePlayerSystems()
	{
		_gameObjectPlayerCollider = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerCollider");
		PlayerCameraFirstPerson = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "CameraFirstPerson");
		PlayerCameraPostProcessing = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "CameraIgnorePostProcessing");
		
		PlayerBehaviour = _gameObjectPlayer.GetComponent<PlayerBehaviourController>();
		PlayerMovementController = _gameObjectPlayer.GetComponent<PlayerMovementController>();
		PlayerMovementStateMachineController = _gameObjectPlayer.GetComponent<PlayerMovementStateMachineController>();
		_playerColliderController = _gameObjectPlayer.GetComponentInChildren<PlayerColliderController>();
		_playerMovementAnimationController = _gameObjectPlayer.GetComponent<PlayerMovementAnimationController>();

		PlayerCameraController = _gameObjectPlayerCamera.GetComponent<PlayerCameraController>();
		PlayerCameraStateMachineController = _gameObjectPlayerCamera.GetComponent<PlayerCameraStateMachineController>();
		_playerCameraBlurFilter = _gameObjectPlayerCamera.GetComponent<PlayerCameraBlurFilter>();
		_playerCameraFirstPersonRender = _gameObjectPlayerCamera.GetComponent<PlayerCameraFirstPersonRenderer>();

		_playerResourcesHealthManager = _gameObjectPlayer.GetComponent<PlayerResourcesHealthManager>();
		_playerResourcesManaManager = _gameObjectPlayer.GetComponent<PlayerResourcesManaManager>();
		_playerResourcesMoneyManager = _gameObjectPlayer.GetComponent<PlayerResourcesMoneyManager>();
		PlayerResourcesAmmoManager = _gameObjectPlayer.GetComponent<PlayerResourcesAmmoManager>();

		_gameObjectPlayerHead = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerHead");
		_gameobjectPlayerEyesLookAt = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "EyesLookAt");
		GameObkectPlayerHatSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerHatSlot");

		GameObjectPlayerFirstPersonHandRight = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "PlayerFirstPersonArmRight");
		GameObjectPlayerFirstPersonHandLeft = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "PlayerFirstPersonArmLeft");
		GameObjectPlayerThirdPersonHandRight = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerThirdPersonArmRight");
		GameObjectPlayerThirdPersonHandLeft = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerThirdPersonArmLeft");

		var canvasComponentBackgroundMenu = _canvasMenuBackground.GetComponent<Canvas>();
		var PlayerCameraComponentPostProcessing = PlayerCameraPostProcessing.GetComponent<Camera>();
		canvasComponentBackgroundMenu.worldCamera = PlayerCameraComponentPostProcessing;
		canvasComponentBackgroundMenu.planeDistance = 2;

		PlayerBehaviour.Initialize(
			_bootstrap,
			_inputDevice);

		PlayerMovementController.Initialize(_bootstrap,
			_inputDevice,
			_gameSceneManager,
			PlayerBehaviour);

		PlayerMovementStateMachineController.Initialize(
			_bootstrap,
			_inputDevice,
			_gameSceneManager,
			PlayerMovementController);

		_playerColliderController.Initialize(
			_bootstrap,
			PlayerMovementStateMachineController);

		PlayerCameraController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionControlsController,
			PlayerMovementController,
			_playerColliderController,
			_gameObjectPlayer,
			_gameObjectPlayerCamera);

		PlayerCameraStateMachineController.Initialize(
			_bootstrap,
			_inputDevice,
			_gameSceneManager,
			PlayerMovementController,
			PlayerMovementStateMachineController,
			PlayerCameraController);

		_playerCameraBlurFilter.Initialize(
			_bootstrapSubProcessMenuSystem.MenuManager,
			PlayerCameraFirstPerson);

		_playerCameraFirstPersonRender.Initialize(
			PlayerCameraStateMachineController,
			_gameObjectPlayerHead,
			GameObkectPlayerHatSlot);

		_playerMovementAnimationController.Initialize(
			_bootstrap,
			_inputDevice,
			PlayerBehaviour,
			PlayerMovementStateMachineController,
			PlayerCameraStateMachineController,
			_gameObjectPlayer);

		_playerResourcesHealthManager.Initialize(
			_gameController,
			_bootstrapSubProcessMenuSystem.ViewModelHUDhealthAndMana,
			_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel);

		_playerResourcesManaManager.Initialize(
			_bootstrapSubProcessMenuSystem.ViewModelHUDhealthAndMana,
			_bootstrapSubProcessMenuSystem.ViewModelWeaponWheel);

		_playerResourcesMoneyManager.Initialize(
			_bootstrapSubProcessMenuSystem.ViewModelPauseMenu.TextCurrentPlayerMoneyDisplay);

		PlayerResourcesAmmoManager.Initialize();

		ServiceLocator.Register("PlayerBehaviour", PlayerBehaviour);
		ServiceLocator.Register("PlayerMovementController", PlayerMovementController);
		ServiceLocator.Register("PlayerMovementStateMachineController", PlayerMovementStateMachineController);
		ServiceLocator.Register("PlayerCameraController", PlayerCameraController);
		ServiceLocator.Register("PlayerCameraStateMachineController", PlayerCameraStateMachineController);
		ServiceLocator.Register("PlayerCameraBlurFilter", _playerCameraBlurFilter);

		ServiceLocator.Register("GameObjectPlayer", _gameObjectPlayer);
		ServiceLocator.Register("GameObjectPlayerEyesLookAt", _gameobjectPlayerEyesLookAt);
		ServiceLocator.Register("GameObjectPlayerHead", _gameObjectPlayerHead);
		ServiceLocator.Register("GameObjectPlayerCollider", _gameObjectPlayerCollider);
		ServiceLocator.Register("GameObjectPlayerCamera", _gameObjectPlayerCamera);

		ServiceLocator.Register("PlayerResourcesHealthManager", _playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", _playerResourcesManaManager);
		ServiceLocator.Register("PlayerResourcesMoneyManager", _playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesAmmoManager", PlayerResourcesAmmoManager);

		yield break;
	}
}
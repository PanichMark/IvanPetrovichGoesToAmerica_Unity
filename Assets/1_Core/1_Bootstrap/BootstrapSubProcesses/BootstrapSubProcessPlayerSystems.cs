using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapSubProcessPlayerSystems
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private GameSceneManager _gameSceneManager;

	private GameObject _gameObjectPlayer;
	private GameObject _gameObjectPlayerCollider;
	private GameObject _gameObjectPlayerHead;
	public GameObject GameObjectPlayerThirdPersonHandRight { get; private set; }
	public GameObject GameObjectPlayerThirdPersonHandLeft { get; private set; }
	public GameObject GameObjectPlayerFirstPersonHandRight { get; private set; }
	public GameObject GameObjectPlayerFirstPersonHandLeft { get; private set; }

	private GameObject _gameObjectPlayerCamera;

	public PlayerBehaviourController PlayerBehaviour { get; private set; }
	public PlayerMovementController PlayerMovementController { get; private set; }
	public PlayerMovementStateMachineController PlayerMovementStateMachineController { get; private set; }
	private PlayerColliderController _playerColliderController;
	
	public PlayerCameraController PlayerCameraController { get; private set; }
	public PlayerCameraStateMachineController PlayerCameraStateMachineController { get; private set; }
	private PlayerCameraBlurFilter _playerCameraBlurFilter;
	private PlayerCameraFirstPersonRender _playerCameraFirstPersonRender;

	private PlayerMovementAnimationController _playerMovementAnimationController;

	private PlayerResourcesHealthManager _playerResourcesHealthManager;

	private PlayerResourcesManaManager _playerResourcesManaManager;

	private PlayerResourcesMoneyManager _playerResourcesMoneyManager;

	public PlayerResourcesAmmoManager PlayerResourcesAmmoManager { get; private set; }

	public BootstrapSubProcessPlayerSystems(
		Bootstrap bootstrap,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		GameController gameController,
		IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		GameObject playerGameObject,
		GameObject playerMainCameraGameObject)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_gameObjectPlayer = playerGameObject;
		_gameObjectPlayerCamera = playerMainCameraGameObject;
	}

	public IEnumerator InitializePlayerSystems()
	{
		_gameObjectPlayerCollider = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "Collider");

		PlayerBehaviour = _gameObjectPlayer.GetComponent<PlayerBehaviourController>();
		PlayerMovementController = _gameObjectPlayer.GetComponent<PlayerMovementController>();
		PlayerMovementStateMachineController = _gameObjectPlayer.GetComponent<PlayerMovementStateMachineController>();
		_playerColliderController = _gameObjectPlayer.GetComponentInChildren<PlayerColliderController>();
		_playerMovementAnimationController = _gameObjectPlayer.GetComponent<PlayerMovementAnimationController>();

		PlayerCameraController = _gameObjectPlayerCamera.GetComponent<PlayerCameraController>();
		PlayerCameraStateMachineController = _gameObjectPlayerCamera.GetComponent<PlayerCameraStateMachineController>();
		_playerCameraBlurFilter = _gameObjectPlayerCamera.GetComponent<PlayerCameraBlurFilter>();
		_playerCameraFirstPersonRender = _gameObjectPlayerCamera.GetComponent<PlayerCameraFirstPersonRender>();

		_playerResourcesHealthManager = _gameObjectPlayer.GetComponent<PlayerResourcesHealthManager>();
		_playerResourcesManaManager = _gameObjectPlayer.GetComponent<PlayerResourcesManaManager>();
		_playerResourcesMoneyManager = _gameObjectPlayer.GetComponent<PlayerResourcesMoneyManager>();
		PlayerResourcesAmmoManager = _gameObjectPlayer.GetComponent<PlayerResourcesAmmoManager>();

		_gameObjectPlayerHead = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerHeadGameObject");
		GameObjectPlayerFirstPersonHandRight = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "PlayerFirstPersonHandRightGameObject");
		GameObjectPlayerFirstPersonHandLeft = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "PlayerFirstPersonHandLeftGameObject");
		GameObjectPlayerThirdPersonHandRight = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerThirdPersonHandRightGameObject");
		GameObjectPlayerThirdPersonHandLeft = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerThirdPersonHandLeftGameObject");

		PlayerBehaviour.Initialize(_inputDevice);
		PlayerMovementController.Initialize(_inputDevice, _gameSceneManager, PlayerBehaviour);
		PlayerMovementStateMachineController.Initialize(_inputDevice, _gameSceneManager, PlayerMovementController);

		_playerColliderController.Initialize(PlayerMovementStateMachineController);

		PlayerCameraController.Initialize(_inputDevice, _gameSceneManager, _bootstrapSubProcessMenuSystem.MenuManager, PlayerMovementController, PlayerMovementStateMachineController, _playerColliderController, _gameObjectPlayer);
		PlayerCameraStateMachineController.Initialize(_inputDevice, _gameSceneManager, PlayerMovementController, PlayerMovementStateMachineController, PlayerCameraController);
		_playerCameraBlurFilter.Initialize(_bootstrapSubProcessMenuSystem.MenuManager);
		_playerCameraFirstPersonRender.Initialize(PlayerCameraStateMachineController, _gameObjectPlayerHead);

		_playerMovementAnimationController.Initialize(_inputDevice, PlayerBehaviour, PlayerMovementStateMachineController, PlayerCameraStateMachineController, _gameObjectPlayer);

		_playerResourcesHealthManager.Initialize(_gameController, PlayerBehaviour, _bootstrapSubProcessMenuSystem.SliderHealthBar, _bootstrapSubProcessMenuSystem.ButtonUseHealingItem, _bootstrapSubProcessMenuSystem.TextHealingItemNumber);
		_playerResourcesManaManager.Initialize(_bootstrapSubProcessMenuSystem.SliderManaBar, _bootstrapSubProcessMenuSystem.ButtonUseManaReplenishItem, _bootstrapSubProcessMenuSystem.TextManaReplenishItemNumber);
		_playerResourcesMoneyManager.Initialize(_bootstrapSubProcessMenuSystem.TextPlayerMoneyNumber);
		PlayerResourcesAmmoManager.Initialize();

		ServiceLocator.Register("PlayerBehaviour", PlayerBehaviour);
		ServiceLocator.Register("PlayerMovementController", PlayerMovementController);
		ServiceLocator.Register("PlayerMovementStateMachineController", PlayerMovementStateMachineController);
		ServiceLocator.Register("PlayerCameraController", PlayerCameraController);
		ServiceLocator.Register("PlayerCameraStateMachineController", PlayerCameraStateMachineController);
		ServiceLocator.Register("PlayerCameraBlurFilter", _playerCameraBlurFilter);

		ServiceLocator.Register("GameObjectPlayer", _gameObjectPlayer);
		ServiceLocator.Register("GameObjectPlayerCollider", _gameObjectPlayerCollider);
		ServiceLocator.Register("GameObjectPlayerCamera", _gameObjectPlayerCamera);

		ServiceLocator.Register("PlayerResourcesHealthManager", _playerResourcesHealthManager);
		ServiceLocator.Register("PlayerResourcesManaManager", _playerResourcesManaManager);
		ServiceLocator.Register("PlayerResourcesMoneyManager", _playerResourcesMoneyManager);
		ServiceLocator.Register("PlayerResourcesAmmoManager", PlayerResourcesAmmoManager);

		Debug.Log("PLAYER SYSTEMS INITIALIZED");

		yield break;
	}
}
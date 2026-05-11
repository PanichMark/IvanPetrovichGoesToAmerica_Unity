using System.Collections;
using UnityEngine;

public class BootstrapSubProcessPlayerSystems
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

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

	public PlayerBehaviour PlayerBehaviour { get; private set; }
	public PlayerMovementController PlayerMovementController { get; private set; }
	private PlayerCapsuleCollider _playerColliderController;
	
	public PlayerCameraController PlayerCameraController { get; private set; }
	private PlayerCameraBlurFilter _playerCameraBlurFilter;
	private PlayerCameraFirstPersonRender _playerCameraFirstPersonRender;

	private PlayerMovementAnimationController _playerMovementAnimationController;

	public BootstrapSubProcessPlayerSystems(
		Bootstrap bootstrap,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		GameObject playerGameObject,
		GameObject playerMainCameraGameObject)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_gameObjectPlayer = playerGameObject;
		_gameObjectPlayerCamera = playerMainCameraGameObject;
	}

	public IEnumerator InitializePlayerSystems()
	{
		_gameObjectPlayerCollider = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "Collider");

		PlayerBehaviour = _gameObjectPlayer.GetComponent<PlayerBehaviour>();
		PlayerMovementController = _gameObjectPlayer.GetComponent<PlayerMovementController>();
		_playerColliderController = _gameObjectPlayer.GetComponentInChildren<PlayerCapsuleCollider>();
		_playerMovementAnimationController = _gameObjectPlayer.GetComponent<PlayerMovementAnimationController>();

		PlayerCameraController = _gameObjectPlayerCamera.GetComponent<PlayerCameraController>();
		_playerCameraBlurFilter = _gameObjectPlayerCamera.GetComponent<PlayerCameraBlurFilter>();
		_playerCameraFirstPersonRender = _gameObjectPlayerCamera.GetComponent<PlayerCameraFirstPersonRender>();

		GameObjectPlayerFirstPersonHandRight = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "PlayerFirstPersonHandRightGameObject");
		GameObjectPlayerFirstPersonHandLeft = _bootstrap.FindDeepGameObject(_gameObjectPlayerCamera, "PlayerFirstPersonHandLeftGameObject");
		_gameObjectPlayerHead = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerHeadGameObject");
		GameObjectPlayerThirdPersonHandRight = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerThirdPersonHandRightGameObject");
		GameObjectPlayerThirdPersonHandLeft = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "PlayerThirdPersonHandLeftGameObject");

		PlayerBehaviour.Initialize(_inputDevice);
		PlayerMovementController.Initialize(_inputDevice, _gameSceneManager, PlayerBehaviour);
		_playerColliderController.Initialize(PlayerMovementController);
		PlayerCameraController.Initialize(_inputDevice, _gameSceneManager, _bootstrapSubProcessMenuSystem.MenuManager, PlayerMovementController, _playerColliderController, _gameObjectPlayer);
		_playerCameraBlurFilter.Initialize(_bootstrapSubProcessMenuSystem.MenuManager);
		_playerMovementAnimationController.Initialize(_inputDevice, _gameObjectPlayer, PlayerBehaviour, PlayerMovementController, PlayerCameraController);
		_playerCameraFirstPersonRender.Initialize(PlayerCameraController, _gameObjectPlayerHead);

		ServiceLocator.Register("PlayerCameraController", PlayerCameraController);
		ServiceLocator.Register("PlayerMovementController", PlayerMovementController);

		ServiceLocator.Register("PlayerCameraBlurFilter", _playerCameraBlurFilter);
		ServiceLocator.Register("PlayerGameObject", _gameObjectPlayer);
		ServiceLocator.Register("PlayerCameraGameObject", _gameObjectPlayerCamera);
		ServiceLocator.Register("PlayerBehaviour", PlayerBehaviour);
		ServiceLocator.Register("PlayerColliderGameObject", _gameObjectPlayerCollider);

		Debug.Log("PLAYER SYSTEMS INITIALIZED");

		yield break;
	}
}
using System.Collections;
using UnityEngine;

public class BootstrapSubProcessPlayerSystems
{
	private IInputDevice _inputDevice;
	private GameSceneManager _gameSceneManager;
	private Bootstrap _bootstrap;
	private GameObject _gameObjectPlayer;
	private GameObject _gameObjectPlayerCamera;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameObject _gameObjectPlayerHead;
	private GameObject _gameObjectPlayerCollider;
	public GameObject GameObjectPlayerThirdPersonHandRight { get; private set; }
	public GameObject GameObjectPlayerThirdPersonHandLeft { get; private set; }
	public GameObject GameObjectPlayerFirstPersonHandRight { get; private set; }
	public GameObject GameObjectPlayerFirstPersonHandLeft { get; private set; }

	public PlayerBehaviour PlayerBehaviour { get; private set; }
	public PlayerMovementController PlayerMovementController { get; private set; }
	private PlayerCapsuleCollider _playerColliderController;
	private PlayerMovementAnimationController _playerMovementAnimationController;

	public PlayerCameraController PlayerCameraController { get; private set; }
	private PlayerCameraBlurFilter _playerCameraBlurFilter;
	private PlayerCameraFirstPersonRender _playerCameraFirstPersonRender;

	public BootstrapSubProcessPlayerSystems(IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		Bootstrap bootstrap,
		GameObject playerGameObject,
		GameObject playerMainCameraGameObject,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem)
	{
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_bootstrap = bootstrap;
		_gameObjectPlayer = playerGameObject;
		_gameObjectPlayerCamera = playerMainCameraGameObject;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
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
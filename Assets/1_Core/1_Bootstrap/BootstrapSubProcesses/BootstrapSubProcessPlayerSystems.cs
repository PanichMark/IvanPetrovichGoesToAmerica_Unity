using System.Collections;
using UnityEngine;

public class BootstrapSubProcessPlayerSystems
{
	private IInputDevice _inputDevice;
	private GameSceneManager _gameSceneManager;
	private Bootstrap _bootstrap;
	private GameObject _playerGameObject;
	private GameObject _playerCameraGameObject;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameObject _playerHeadParentGameObject;
	private GameObject _playerColliderGameObject;
	public GameObject PlayerThirdPersonHandRightParentGameObject { get; private set; }
	public GameObject PlayerThirdPersonHandLeftParentGameObject { get; private set; }
	public GameObject PlayerFirstPersonHandRightGameObject { get; private set; }
	public GameObject PlayerFirstPersonHandLeftGameObject { get; private set; }

	public PlayerBehaviour PlayerBehaviour { get; private set; }
	public PlayerMovementController PlayerMovementController { get; private set; }
	private PlayerCapsuleCollider _playerColliderController;
	private PlayerAnimationController _playerAnimationController;

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
		_playerGameObject = playerGameObject;
		_playerCameraGameObject = playerMainCameraGameObject;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
	}

	public IEnumerator InitializePlayerSystems()
	{
		_playerColliderGameObject = _bootstrap.FindDeepGameObject(_playerGameObject, "Collider");

		PlayerBehaviour = _playerGameObject.GetComponent<PlayerBehaviour>();
		PlayerMovementController = _playerGameObject.GetComponent<PlayerMovementController>();
		_playerColliderController = _playerGameObject.GetComponentInChildren<PlayerCapsuleCollider>();
		_playerAnimationController = _playerGameObject.GetComponent<PlayerAnimationController>();

		PlayerCameraController = _playerCameraGameObject.GetComponent<PlayerCameraController>();
		_playerCameraBlurFilter = _playerCameraGameObject.GetComponent<PlayerCameraBlurFilter>();
		_playerCameraFirstPersonRender = _playerCameraGameObject.GetComponent<PlayerCameraFirstPersonRender>();

		PlayerFirstPersonHandRightGameObject = _bootstrap.FindDeepGameObject(_playerCameraGameObject, "PlayerFirstPersonHandRightGameObject");
		PlayerFirstPersonHandLeftGameObject = _bootstrap.FindDeepGameObject(_playerCameraGameObject, "PlayerFirstPersonHandLeftGameObject");
		_playerHeadParentGameObject = _bootstrap.FindDeepGameObject(_playerGameObject, "PlayerHeadGameObject");
		PlayerThirdPersonHandRightParentGameObject = _bootstrap.FindDeepGameObject(_playerGameObject, "PlayerThirdPersonHandRightParentGameObject");
		PlayerThirdPersonHandLeftParentGameObject = _bootstrap.FindDeepGameObject(_playerGameObject, "PlayerThirdPersonHandLeftParentGameObject");

		PlayerBehaviour.Initialize(_inputDevice);
		PlayerMovementController.Initialize(_inputDevice, _gameSceneManager, PlayerBehaviour);
		_playerColliderController.Initialize(PlayerMovementController);
		PlayerCameraController.Initialize(_inputDevice, _gameSceneManager, _bootstrapSubProcessMenuSystem.MenuManager, PlayerMovementController, _playerColliderController, _playerGameObject);
		_playerCameraBlurFilter.Initialize(_bootstrapSubProcessMenuSystem.MenuManager);
		_playerAnimationController.Initialize(_inputDevice, _playerGameObject, PlayerBehaviour, PlayerMovementController, PlayerCameraController);
		_playerCameraFirstPersonRender.Initialize(PlayerCameraController, _playerHeadParentGameObject);

		ServiceLocator.Register("PlayerCameraController", PlayerCameraController);
		ServiceLocator.Register("PlayerMovementController", PlayerMovementController);

		ServiceLocator.Register("PlayerCameraBlurFilter", _playerCameraBlurFilter);
		ServiceLocator.Register("PlayerGameObject", _playerGameObject);
		ServiceLocator.Register("PlayerCameraGameObject", _playerCameraGameObject);
		ServiceLocator.Register("PlayerBehaviour", PlayerBehaviour);
		ServiceLocator.Register("PlayerColliderGameObject", _playerColliderGameObject);

		Debug.Log("PLAYER SYSTEMS INITIALIZED");

		yield break;
	}
}
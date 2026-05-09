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
	public GameObject PlayerHandRightParentGameObject { get; private set; }
	public GameObject PlayerHandLeftParentGameObject { get; private set; }
	public GameObject PlayerFirstPersonHandRightGameObject { get; private set; }
	public GameObject PlayerFirstPersonHandLeftGameObject { get; private set; }

	public PlayerBehaviour PlayerBehaviour { get; private set; }
	public PlayerMovementController PlayerMovementController { get; private set; }
	private PlayerCapsuleCollider playerColliderController;
	private PlayerAnimationController playerAnimationController;

	public PlayerCameraController playerCameraController { get; private set; }
	private PlayerCameraBlurFilter playerCameraBlurFilter;
	private PlayerCameraFirstPersonRender playerCameraFirstPersonRender;

	public BootstrapSubProcessPlayerSystems(IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		Bootstrap bootstrap,
		GameObject playerGameObject,
		GameObject playerMainCameraGameObject,
		BootstrapSubProcessMenuSystem bootstrapSubSystemMenu)
	{
		this._inputDevice = inputDevice;
		this._gameSceneManager = gameSceneManager;
		this._bootstrap = bootstrap;
		this._playerGameObject = playerGameObject;
		this._playerCameraGameObject = playerMainCameraGameObject;
		this._bootstrapSubProcessMenuSystem = bootstrapSubSystemMenu;
	}

	public IEnumerator InitializePlayerSystems()
	{
		_playerColliderGameObject = _bootstrap.FindDeepGameObject(_playerGameObject, "Collider");

		PlayerBehaviour = _playerGameObject.GetComponent<PlayerBehaviour>();
		PlayerMovementController = _playerGameObject.GetComponent<PlayerMovementController>();
		playerColliderController = _playerGameObject.GetComponentInChildren<PlayerCapsuleCollider>();
		playerAnimationController = _playerGameObject.GetComponent<PlayerAnimationController>();

		playerCameraController = _playerCameraGameObject.GetComponent<PlayerCameraController>();
		playerCameraBlurFilter = _playerCameraGameObject.GetComponent<PlayerCameraBlurFilter>();
		playerCameraFirstPersonRender = _playerCameraGameObject.GetComponent<PlayerCameraFirstPersonRender>();

		PlayerFirstPersonHandRightGameObject = _bootstrap.FindDeepGameObject(_playerCameraGameObject, "UNITY HandRight");
		PlayerFirstPersonHandLeftGameObject = _bootstrap.FindDeepGameObject(_playerCameraGameObject, "UNITY  HandLeft");
		_playerHeadParentGameObject = _bootstrap.FindDeepGameObject(_playerGameObject, "UNITY PlayerHead");
		PlayerHandRightParentGameObject = _bootstrap.FindDeepGameObject(_playerGameObject, "UNITY HandRight");
		PlayerHandLeftParentGameObject = _bootstrap.FindDeepGameObject(_playerGameObject, "UNITY  HandLeft");

		PlayerBehaviour.Initialize(_inputDevice);
		PlayerMovementController.Initialize(_inputDevice, _gameSceneManager, PlayerBehaviour);
		playerColliderController.Initialize(PlayerMovementController);
		playerCameraController.Initialize(_inputDevice, _gameSceneManager, _bootstrapSubProcessMenuSystem.MenuManager, PlayerMovementController, playerColliderController, _playerGameObject);
		playerCameraBlurFilter.Initialize(_bootstrapSubProcessMenuSystem.MenuManager);
		playerAnimationController.Initialize(_inputDevice, _playerGameObject, PlayerBehaviour, PlayerMovementController, playerCameraController);
		playerCameraFirstPersonRender.Initialize(playerCameraController, _playerHeadParentGameObject);

		ServiceLocator.Register("PlayerCameraController", playerCameraController);
		ServiceLocator.Register("PlayerMovementController", PlayerMovementController);

		ServiceLocator.Register("PlayerCameraBlurFilter", playerCameraBlurFilter);
		ServiceLocator.Register("Player", _playerGameObject);
		ServiceLocator.Register("playerMainCameraGameObject", _playerCameraGameObject);
		ServiceLocator.Register("PlayerBehaviour", PlayerBehaviour);
		ServiceLocator.Register("playerColliderGameObject", _playerColliderGameObject);

		Debug.Log("PLAYER SYSTEMS INITIALIZED");

		yield break;
	}
}
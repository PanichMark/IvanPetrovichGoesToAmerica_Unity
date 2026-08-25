using System.Collections;
using UnityEngine;

public class BootstrapSubProcessInteractionSystem
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;
	private GameObject _playerFirstPersonHandRight;
	private GameObject _playerThirdPersonHandRight;
	private GameScenesManager _gameSceneManager;
	public GameObject GameObjectSpineSlot {  get; private set; }
	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private PlayerInteractionFirstPersonRenderer _interactionFirstPersonRenderer;

	private GameObject _gameObjectBootstrapInteractionSystem;
	public PlayerInteractionController InteractionController { get; private set; }
	private PlayerInteractionAnimationController _interactionAnimationController;

	private GameObject _gameObjectPlayer;
	private GameObject _gameObjectPlayerCamera;

	private KeysManager _keysManager;

	public BootstrapSubProcessInteractionSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameObject gameObjectPlayer,
		GameObject gameObjectPlayerCamera)
	{
		_bootstrap = bootstrap;
		_bootstrapSubProcessMenuSystem = bootstrapSubProcessMenuSystem;
		_gameController = gameController;
		_inputDevice = inputDevice;
		_localizationManager = localizationManager;
		_gameSceneManager = bootstrapSubProcessSceneSystem.GameSceneManager;
		_playerBehaviour = bootstrapSubProcessPlayerSystems.PlayerBehaviour;
		_playerCameraController = bootstrapSubProcessPlayerSystems.PlayerCameraController;
		_playerCameraStateMachineController = bootstrapSubProcessPlayerSystems.PlayerCameraStateMachineController;

		_playerFirstPersonHandRight = bootstrapSubProcessPlayerSystems.GameObjectPlayerFirstPersonHandRight;
		_playerThirdPersonHandRight = bootstrapSubProcessPlayerSystems.GameObjectPlayerThirdPersonHandRight;

		_gameObjectPlayer = gameObjectPlayer;
		_gameObjectPlayerCamera = gameObjectPlayerCamera;
	}

	public IEnumerator Initialize()
	{
		_gameObjectBootstrapInteractionSystem = new GameObject("Bootstrap_InteractionSystem");

		InteractionController = _gameObjectBootstrapInteractionSystem.AddComponent<PlayerInteractionController>();
		_interactionAnimationController = _gameObjectBootstrapInteractionSystem.AddComponent<PlayerInteractionAnimationController>();
		_interactionFirstPersonRenderer = _gameObjectBootstrapInteractionSystem.AddComponent<PlayerInteractionFirstPersonRenderer>();

		GameObjectSpineSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "Spine");

		InteractionController.Initialize(
			_bootstrap,
			_gameController,
			_inputDevice,
			_localizationManager,
			_gameSceneManager,
			_bootstrapSubProcessMenuSystem.MenuManager,
			_bootstrapSubProcessMenuSystem.PauseSubMenuSettingsSectionGeneralController,
			_playerBehaviour,
			_playerCameraController,
			_playerCameraStateMachineController,
			_bootstrapSubProcessMenuSystem.CanvasHUDinteraction,
			_bootstrapSubProcessMenuSystem.ViewModelHUDInteraction);

		_interactionAnimationController.Initialize
			(InteractionController,
			_gameObjectPlayer,
			_gameObjectPlayerCamera);

		_interactionFirstPersonRenderer.Initialize(
			_gameSceneManager,
			_playerCameraStateMachineController,
			InteractionController,
			_playerFirstPersonHandRight,
			_playerThirdPersonHandRight);

		_keysManager = new KeysManager();

		ServiceLocator.Register("InteractionController", InteractionController);
		ServiceLocator.Register("GameObjectSpineSlot", GameObjectSpineSlot);
		ServiceLocator.Register("KeysManager", _keysManager);

		yield break;
	}
}
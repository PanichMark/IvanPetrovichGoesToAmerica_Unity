using System.Collections;
using UnityEngine;

public class BootstrapSubProcessInteractionSystem
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private GameScenesManager _gameSceneManager;
	public GameObject GameObjectSpineSlot {  get; private set; }
	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private GameObject _gameObjectBootstrapInteractionSystem;
	public InteractionController InteractionController { get; private set; }
	private InteractionAnimationController _interactionAnimationController;
	//private InteractionFirstPersonRender _interactionFirstPersonRender;


	private GameObject _gameObjectPlayer;
	private KeysManager _keysManager;

	public BootstrapSubProcessInteractionSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessScenesSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager,
		GameObject gameObjectPlayer)
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

		_gameObjectPlayer = gameObjectPlayer;
	}

	public IEnumerator Initialize()
	{
		_gameObjectBootstrapInteractionSystem = new GameObject("Bootstrap_InteractionSystem");

		InteractionController = _gameObjectBootstrapInteractionSystem.AddComponent<InteractionController>();
		_interactionAnimationController = _gameObjectBootstrapInteractionSystem.AddComponent<InteractionAnimationController>();

		GameObjectSpineSlot = _bootstrap.FindDeepGameObject(_gameObjectPlayer, "Spine");

		//_interactionFirstPersonRender = _gameObjectBootstrapInteractionSystem.AddComponent<InteractionFirstPersonRender>();

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
			_gameObjectPlayer);

		_keysManager = new KeysManager();

		ServiceLocator.Register("InteractionController", InteractionController);
		ServiceLocator.Register("GameObjectSpineSlot", GameObjectSpineSlot);
		ServiceLocator.Register("KeysManager", _keysManager);

		yield break;
	}
}
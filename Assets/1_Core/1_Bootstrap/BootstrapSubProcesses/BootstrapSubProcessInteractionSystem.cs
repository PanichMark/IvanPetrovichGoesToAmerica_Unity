using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BootstrapSubProcessInteractionSystem
{
	private Bootstrap _bootstrap;
	private BootstrapSubProcessMenuSystem _bootstrapSubProcessMenuSystem;

	private GameController _gameController;
	private IInputDevice _inputDevice;
	private LocalizationManager _localizationManager;

	private GameSceneManager _gameSceneManager;

	private PlayerBehaviourController _playerBehaviour;
	private PlayerCameraController _playerCameraController;
	private PlayerCameraStateMachineController _playerCameraStateMachineController;

	private GameObject _gameObjectBootstrapInteractionSystem;
	public InteractionController InteractionController { get; private set; }
	//private InteractionAnimationController _interactionAnimationController;
	//private InteractionFirstPersonRender _interactionFirstPersonRender;

	private KeysManager _keysManager;

	public BootstrapSubProcessInteractionSystem(
		Bootstrap bootstrap,
		BootstrapSubProcessSceneSystem bootstrapSubProcessSceneSystem,
		BootstrapSubProcessMenuSystem bootstrapSubProcessMenuSystem,
		BootstrapSubProcessPlayerSystems bootstrapSubProcessPlayerSystems,
		GameController gameController,
		IInputDevice inputDevice,
		LocalizationManager localizationManager)
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
	}

	public IEnumerator InitializeInteractionSystem()
	{
		_gameObjectBootstrapInteractionSystem = new GameObject("Bootstrap_InteractionSystem");

		InteractionController = _gameObjectBootstrapInteractionSystem.AddComponent<InteractionController>();
		//_interactionAnimationController = _gameObjectBootstrapInteractionSystem.AddComponent<InteractionAnimationController>();
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

		_keysManager = new KeysManager();

		ServiceLocator.Register("KeysManager", _keysManager);

		yield break;
	}
}
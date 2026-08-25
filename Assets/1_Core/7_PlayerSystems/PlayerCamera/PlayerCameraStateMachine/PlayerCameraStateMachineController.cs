using System;
using UnityEngine;
using System.Collections;

public class PlayerCameraStateMachineController : MonoBehaviour, IJsonSaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameScenesManager _gameSceneManager;

	private PlayerCameraStateAbstract _playerCameraState;
	private PauseMenuConfirmActionController _pauseMenuConfirmActionController;
	private PlayerMovementController _movementController;
	private PlayerMovementStateMachineController _playerMovementStateMachineController;
	private PlayerCameraController _cameraController;


	public PlayerCameraStateTypes CurrentPlayerCameraStateType { get; private set; }

	public delegate void CameraStateHandler();
	public event CameraStateHandler OnCameraStateChanged;
	public event CameraStateHandler OnFirstPersonCameraState;
	public event CameraStateHandler OnThirdPersonCameraState;

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		GameScenesManager gameSceneManager,
		PauseMenuConfirmActionController pauseMenuConfirmActionController,
		PlayerMovementController playerMovementController,
		PlayerMovementStateMachineController playerMovementStateMachineController,
		PlayerCameraController playerCameraController)
	{
		_pauseMenuConfirmActionController = pauseMenuConfirmActionController;
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_movementController = playerMovementController;
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_cameraController = playerCameraController;

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => SetPlayerCameraState(PlayerCameraStateTypes.MainMenu);

		_pauseMenuConfirmActionController.OnSetPlayerCameraToFirstPerson += () => SetPlayerCameraState(PlayerCameraStateTypes.FirstPerson);

		SetPlayerCameraState(PlayerCameraStateTypes.ThirdPerson);

		Debug.Log("PlayerCameraStateMachineController Initialized");
	}

	private void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
		{
			return;
		}

		_playerCameraState.Update();
	}

	public void SetPlayerCameraState(PlayerCameraStateTypes newPlayerCameraStateType)
	{
		PlayerCameraStateAbstract newState;

		CurrentPlayerCameraStateType = newPlayerCameraStateType;

		_movementController.GiveCurrentPlayerCameraType(CurrentPlayerCameraStateType);

		if (newPlayerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			newState = new PlayerCameraStateFirstPerson(_cameraController, this, _movementController, _playerMovementStateMachineController, _inputDevice);
			OnFirstPersonCameraState?.Invoke();
			_cameraController.SetCameraToFirstPerson();
		}
		else if (newPlayerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			newState = new PlayerCameraStateThirdPerson(_cameraController, this, _inputDevice);
			OnThirdPersonCameraState?.Invoke();
			_cameraController.SetCameraToThirdPerson();
		}
		else if (newPlayerCameraStateType == PlayerCameraStateTypes.Cutscene)
		{
			newState = new PlayerCameraStateCutscene();
		}
		else if (newPlayerCameraStateType == PlayerCameraStateTypes.MainMenu)
		{
			newState = new PlayerCameraStateMainMenu(_cameraController, new Vector3(0.2f, 1.35f, -0.9f), new Vector3(20, -12, 0));
		}
		else
		{
			newState = null;
		}

		_playerCameraState = newState;

		OnCameraStateChanged?.Invoke();

		Debug.Log("CameraState: " + CurrentPlayerCameraStateType);
	
	}

	public IEnumerator SaveJsonData(JsonGameData data)
	{
		data.PlayerCamera.PlayerCameraStateType = CurrentPlayerCameraStateType.ToString();
		yield return null;
	}

	public IEnumerator LoadJsonData(JsonGameData data)
	{
		CurrentPlayerCameraStateType = (PlayerCameraStateTypes)Enum.Parse(typeof(PlayerCameraStateTypes), data.PlayerCamera.PlayerCameraStateType);

		SetPlayerCameraState(CurrentPlayerCameraStateType);

		yield return null;
	}
}
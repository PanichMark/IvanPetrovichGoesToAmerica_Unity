using System;
using UnityEngine;

public class PlayerCameraStateMachineController : MonoBehaviour, ISaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameSceneManager _gameSceneManager;

	private PlayerCameraStateAbstract _playerCameraState;

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
		GameSceneManager gameSceneManager,
		PlayerMovementController playerMovementController,
		PlayerMovementStateMachineController playerMovementStateMachineController,
		PlayerCameraController playerCameraController)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_movementController = playerMovementController;
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_cameraController = playerCameraController;

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => SetPlayerCameraState(PlayerCameraStateTypes.MainMenu);

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

	public void SaveData(ref GameData data)
	{
		data.CurrentPlayerCameraStateType = CurrentPlayerCameraStateType.ToString();
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerCameraStateType = (PlayerCameraStateTypes)Enum.Parse(typeof(PlayerCameraStateTypes), data.CurrentPlayerCameraStateType);

		SetPlayerCameraState(CurrentPlayerCameraStateType);
	}
}
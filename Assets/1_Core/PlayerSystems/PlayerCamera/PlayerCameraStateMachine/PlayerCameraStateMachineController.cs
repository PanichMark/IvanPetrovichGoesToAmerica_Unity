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


	private PlayerCameraStateTypes _playerCameraStateType;
	public string CurrentPlayerCameraStateType { get; private set; }

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

	public void SetPlayerCameraState(PlayerCameraStateTypes playerCameraStateType)
	{
		PlayerCameraStateAbstract newState;

		if (playerCameraStateType == PlayerCameraStateTypes.FirstPerson)
		{
			CurrentPlayerCameraStateType = "FirstPerson";
			_movementController.GiveCurrentPlayerCameraType("FirstPerson");
			newState = new PlayerCameraStateFirstPerson(_cameraController, this, _movementController, _playerMovementStateMachineController, _inputDevice);
			OnFirstPersonCameraState?.Invoke();
			_cameraController.SetCameraToFirstPerson();
		}
		else if (playerCameraStateType == PlayerCameraStateTypes.ThirdPerson)
		{
			CurrentPlayerCameraStateType = "ThirdPerson";
			_movementController.GiveCurrentPlayerCameraType("ThirdPerson");
			newState = new PlayerCameraStateThirdPerson(_cameraController, this, _inputDevice);
			OnThirdPersonCameraState?.Invoke();
			_cameraController.SetCameraToThirdPerson();
		}
		else if (playerCameraStateType == PlayerCameraStateTypes.Cutscene)
		{
			CurrentPlayerCameraStateType = "Cutscene";
			_movementController.GiveCurrentPlayerCameraType("Cutscene");
			newState = new PlayerCameraStateCutscene();
		}
		else if (playerCameraStateType == PlayerCameraStateTypes.MainMenu)
		{
			CurrentPlayerCameraStateType = "MainMenu";
			_movementController.GiveCurrentPlayerCameraType("MainMenu");
			newState = new PlayerCameraStateMainMenu(_cameraController, new Vector3(0.2f, 1.35f, -0.9f), new Vector3(20, -12, 0));
		}
		else
		{
			newState = null;
		}
		OnCameraStateChanged?.Invoke();
		Debug.Log("CameraState: " + CurrentPlayerCameraStateType);
		_playerCameraState = newState;
	}

	public void SaveData(ref GameData data)
	{
		data.CurrentPlayerCameraStateType = CurrentPlayerCameraStateType;
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerCameraStateType = data.CurrentPlayerCameraStateType;

		_playerCameraStateType = (PlayerCameraStateTypes)Enum.Parse(typeof(PlayerCameraStateTypes), CurrentPlayerCameraStateType);
		SetPlayerCameraState(_playerCameraStateType);
	}
}

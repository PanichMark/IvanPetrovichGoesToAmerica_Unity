using System;
using UnityEngine;

public class PlayerMovementStateMachineController : MonoBehaviour, ISaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameSceneManager _gameSceneManager;

	private PlayerMovementController _playerMovementController;
	private PlayerMovementStateAbstract _playerMovementState;
	public PlayerMovementStateTypes CurrentPlayerMovementStateType { get; private set; }

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		PlayerMovementController playerMovementController)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_playerMovementController = playerMovementController;

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);

		_playerMovementController.OnPlayerMovementStateChanged += SetPlayerMovementState;

		Debug.Log("PlayerMovementStateMachineController Initialized");
	}

	public void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized)
			return;

		_playerMovementState.Update();
	}

	public void SetPlayerMovementState(PlayerMovementStateTypes newPlayerMovementStateType)
	{
		if (_playerMovementController.IsAbleToChangeMovementType)
		{
			PlayerMovementStateAbstract newState;

			CurrentPlayerMovementStateType = newPlayerMovementStateType;

			if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerIdle)
			{
				newState = new PlayerMovementStateIdle(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalking)
			{
				newState = new PlayerMovementStateWalking(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingIdle)
			{
				newState = new PlayerMovementStateCrouchingIdle(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingWalking)
			{
				newState = new PlayerMovementStateCrouchingWalking(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerRunning)
			{
				newState = new PlayerMovementStateRunning(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerJumping)
			{
				newState = new PlayerMovementStateJumping(this,	_playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerFalling)
			{
				newState = new PlayerMovementStateFalling(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerSliding)
			{
				newState = new PlayerMovementStateSliding(_playerMovementController);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerLedgeClimbing)
			{
				newState = new PlayerMovementStateLedgeClimbing(_playerMovementController);
			}
			else
			{
				newState = null;
			}

			_playerMovementState = newState;

			Debug.Log("MovementState: " + CurrentPlayerMovementStateType);
		}
	}

	public void SaveData(ref GameData data)
	{
		data.PlayerMovementStateType = CurrentPlayerMovementStateType.ToString();
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerMovementStateType = (PlayerMovementStateTypes)Enum.Parse(typeof(PlayerMovementStateTypes), data.PlayerMovementStateType);

		SetPlayerMovementState(CurrentPlayerMovementStateType);
	}
}
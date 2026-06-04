using System;
using UnityEngine;

public class PlayerMovementStateMachineController : MonoBehaviour, ISaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameSceneManager _gameSceneManager;

	private PlayerMovementController _playerMovementController;
	private PlayerMovementStateAbstract _playerMovementState;
	private PlayerMovementStateTypes _playerMovementStateType;

	public string CurrentPlayerMovementStateType { get; private set; }

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

	public void SetPlayerMovementState(PlayerMovementStateTypes playerMovementStateType)
	{
		if (_playerMovementController.IsAbleToChangeMovementType)
		{
			PlayerMovementStateAbstract newState;

			if (playerMovementStateType == PlayerMovementStateTypes.PlayerIdle)
			{
				newState = new PlayerMovementStateIdle(this, _playerMovementController, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerIdle";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerWalking)
			{
				newState = new PlayerMovementStateWalking(this, _playerMovementController, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerWalking";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingIdle)
			{
				newState = new PlayerMovementStateCrouchingIdle(this, _playerMovementController, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerCrouchingIdle";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingWalking)
			{
				newState = new PlayerMovementStateCrouchingWalking(this, _playerMovementController, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerCrouchingWalking";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerRunning)
			{
				newState = new PlayerMovementStateRunning(this, _playerMovementController, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerRunning";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerJumping)
			{
				newState = new PlayerMovementStateJumping(this,	_playerMovementController, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerJumping";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerFalling)
			{
				newState = new PlayerMovementStateFalling(this, _playerMovementController, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerFalling";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerSliding)
			{
				newState = new PlayerMovementStateSliding(_playerMovementController);
				CurrentPlayerMovementStateType = "PlayerSliding";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerLedgeClimbing)
			{
				newState = new PlayerMovementStateLedgeClimbing(_playerMovementController);
				CurrentPlayerMovementStateType = "PlayerLedgeClimbing";
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
		data.PlayerMovementStateType = CurrentPlayerMovementStateType;
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerMovementStateType = data.PlayerMovementStateType;

		_playerMovementStateType = (PlayerMovementStateTypes)Enum.Parse(typeof(PlayerMovementStateTypes), CurrentPlayerMovementStateType);
		SetPlayerMovementState(_playerMovementStateType);
	}
}
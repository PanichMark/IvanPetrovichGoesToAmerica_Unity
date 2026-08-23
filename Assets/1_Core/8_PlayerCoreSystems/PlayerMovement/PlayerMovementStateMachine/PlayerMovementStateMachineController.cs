using System;
using UnityEngine;
using System.Collections;

public class PlayerMovementStateMachineController : MonoBehaviour, ISaveLoad
{
	private Bootstrap _bootstrap;
	private IInputDevice _inputDevice;
	private GameScenesManager _gameSceneManager;

	private PlayerMovementController _playerMovementController;
	private PlayerMovementStateAbstract _playerMovementState;
	public PlayerMovementStateTypes CurrentPlayerMovementStateType { get; private set; }

	public delegate void MovementStateHandler(PlayerMovementStateTypes playerMovementStateType);
	public event MovementStateHandler OnChangeMovementState;

	public void Initialize(
		Bootstrap bootstrap,
		IInputDevice inputDevice,
		GameScenesManager gameSceneManager,
		PlayerMovementController playerMovementController)
	{
		_bootstrap = bootstrap;
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_playerMovementController = playerMovementController;

		_gameSceneManager.OnBeginLoadingMainMenuScene += () => SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdleStanding);

		_playerMovementController.OnChangeMovementState += SetPlayerMovementState;

		Debug.Log("PlayerMovementStateMachineController Initialized");
	}

	public void Update()
	{
		if (!_bootstrap.IsBootstrapInitialized && _playerMovementState != null && _playerMovementController == null && _gameSceneManager.IsWaitingForGameplayData)
			return;

		_playerMovementState.Update();
	}

	public void SetPlayerMovementState(PlayerMovementStateTypes newPlayerMovementStateType)
	{
		if (_playerMovementController.IsAbleToChangeMovementType)
		{
			PlayerMovementStateAbstract newState;

			CurrentPlayerMovementStateType = newPlayerMovementStateType;

			if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerIdleStanding)
			{
				newState = new PlayerMovementStateIdleStanding(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerIdleCrouhcing)
			{
				newState = new PlayerMovementStateIdleCrouching(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalkingStanding)
			{
				newState = new PlayerMovementStateWalkingStanding(this, _playerMovementController, _inputDevice);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerWalkingCrouching)
			{
				newState = new PlayerMovementStateWalkingCrouching(this, _playerMovementController, _inputDevice);
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
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerLedgeClimbingStanding)
			{
				newState = new PlayerMovementStateLedgeClimbingStanding(_playerMovementController);
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerLedgeClimbingCrouching)
			{
				newState = new PlayerMovementStateLedgeClimbingCrouching();
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerStranglingNPC)
			{
				newState = new PlayerMovementStateStranglingNPC();
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerPlunging)
			{
				newState = new PlayerMovementStatePlunging();
			}
			else if (newPlayerMovementStateType == PlayerMovementStateTypes.PlayerDying)
			{
				newState = new PlayerMovementStateDying();
			}
			else
			{
				newState = null;
			}

			_playerMovementState = newState;

			OnChangeMovementState?.Invoke(CurrentPlayerMovementStateType);

			Debug.Log("MovementState: " + CurrentPlayerMovementStateType);
		}
	}

	public IEnumerator SaveData(GameData data)
	{
		data.PlayerMovement.PlayerMovementStateType = CurrentPlayerMovementStateType.ToString();
		yield return null;
	}

	public IEnumerator LoadData(GameData data)
	{
		CurrentPlayerMovementStateType = (PlayerMovementStateTypes)Enum.Parse(typeof(PlayerMovementStateTypes), data.PlayerMovement.PlayerMovementStateType);

		SetPlayerMovementState(CurrentPlayerMovementStateType);

		yield return null;
	}
}
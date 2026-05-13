using System;
using UnityEngine;

public class PlayerMovementStateMachineController : MonoBehaviour, ISaveLoad
{
	
	

	private IInputDevice _inputDevice;
	private GameSceneManager _gameSceneManager;

	private PlayerMovementController _playerMovementController;
	private PlayerMovementStateAbstract _playerMovementState;
	private PlayerMovementStateTypes _playerMovementStateType;

	private Transform _playerTransform;
	//private Rigidbody _playerRigidBody;

	public string CurrentPlayerMovementStateType { get; private set; }
	private bool _isAbleToChangeMovementType;
	private string _currentPlayerCameraType = "";
	private float _howMuchUp; //?????










	private bool _isInitialized;

	public void Initialize(
		IInputDevice inputDevice,
		GameSceneManager gameSceneManager,
		PlayerMovementController playerMovementController,
		Transform playerTransform,
		Rigidbody playerRigidBody,
		string currentPlayerMovementStateType,
		bool isAbleToChangeMovementType,
		string currentPlayerCameraType,
		float howMuchUp
		)
	{
		_inputDevice = inputDevice;
		_gameSceneManager = gameSceneManager;
		_playerMovementController = playerMovementController;
		_playerTransform = playerTransform;
		_playerRigidBody = playerRigidBody;

		CurrentPlayerMovementStateType = currentPlayerMovementStateType;
		_isAbleToChangeMovementType = isAbleToChangeMovementType;
		_currentPlayerCameraType = currentPlayerCameraType;
		_howMuchUp = howMuchUp;


		_gameSceneManager.OnBeginLoadingMainMenuScene += () => SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);

		SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);

		_playerMovementController.OnPlayerMovementStateChanged += SetPlayerMovementState;

		_isInitialized = true;

		Debug.Log("PlayerMovementStateMachineController Initialized");
	}

	public void Update()
	{
		if (!_isInitialized)
			return;

		_playerMovementState.Update();
	}

	public void SetPlayerMovementState(PlayerMovementStateTypes playerMovementStateType)
	{
		if (_isAbleToChangeMovementType)
		{
			PlayerMovementStateAbstract newState;

			if (playerMovementStateType == PlayerMovementStateTypes.PlayerIdle)
			{
				_playerRigidBody.angularVelocity = Vector3.zero;
				_howMuchUp = 0.3f;
				newState = new PlayerMovementStateIdle(this, _playerMovementController, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerIdle";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerWalking)
			{
				newState = new PlayerMovementStateWalking(this, _playerMovementController, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerWalking";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingIdle)
			{
				_playerRigidBody.angularVelocity = Vector3.zero;
				newState = new PlayerMovementStateCrouchingIdle(this, _playerMovementController, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerCrouchingIdle";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerCrouchingWalking)
			{
				newState = new PlayerMovementStateCrouchingWalking(this, _playerMovementController, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerCrouchingWalking";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerRunning)
			{
				newState = new PlayerMovementStateRunning(this, _playerMovementController, _inputDevice, _playerTransform, _playerRigidBody);
				CurrentPlayerMovementStateType = "PlayerRunning";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerJumping)
			{
				_howMuchUp = 0;
				newState = new PlayerMovementStateJumping(this,	_playerMovementController, _inputDevice);
				CurrentPlayerMovementStateType = "PlayerJumping";
			}
			else if (playerMovementStateType == PlayerMovementStateTypes.PlayerFalling)
			{
				_howMuchUp = 0.3f;
				newState = new PlayerMovementStateFalling(this, _inputDevice);
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
		data.CurrentPlayerMovementStateType = CurrentPlayerMovementStateType;
	}

	public void LoadData(GameData data)
	{
		CurrentPlayerMovementStateType = data.CurrentPlayerMovementStateType;

		_playerMovementStateType = (PlayerMovementStateTypes)Enum.Parse(typeof(PlayerMovementStateTypes), CurrentPlayerMovementStateType);
		SetPlayerMovementState(_playerMovementStateType);
	}
}

using UnityEngine;

public class PlayerMovementStateCrouchingIdle : PlayerMovementStateAbstract
{
	private IInputDevice _inputDevice;
	private Transform _playerTransform;
	private Rigidbody _playerRigidBody;

	public PlayerMovementStateCrouchingIdle(PlayerMovementStateMachineController playerMovementStateMachineController, PlayerMovementController playerMovementController, IInputDevice inputDevice, Transform playerTransform, Rigidbody playerRigidBody)
	{
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerMovementController = playerMovementController;
		_inputDevice = inputDevice;
		_playerTransform = playerTransform;
		_playerRigidBody = playerRigidBody;

		_playerMovementController.ChangePlayerRayPosition(1.2f);
		_playerMovementController.ChangePlayerRotationSpeed(300f);
	}

	public override void Update()
	{
		if ((_inputDevice.GetKeyUp() || _inputDevice.GetKeyDown() || _inputDevice.GetKeyRight() || _inputDevice.GetKeyLeft()))
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingWalking);
		}

		if (_inputDevice.GetKeyRun() && (_inputDevice.GetKeyUp() || _inputDevice.GetKeyDown() || _inputDevice.GetKeyRight() || _inputDevice.GetKeyLeft()) && _playerMovementController.IsPlayerAbleToStandUp)
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerRunning);
		}

		if (_inputDevice.GetKeyJump() && _playerMovementController.IsPlayerGrounded && _playerMovementController.IsPlayerAbleToStandUp)
		{
			
			_playerRigidBody.AddForce(_playerTransform.up * 5f, ForceMode.Impulse);
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerJumping);
		}

		if (_playerMovementController.IsPlayerFalling)
		{
			
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerFalling);
		}

		if (_inputDevice.GetKeyCrouch() && _playerMovementController.IsPlayerAbleToStandUp)
		{
			
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
		}
	}
}
using UnityEngine;

public class PlayerMovementStateIdle : PlayerMovementStateAbstract
{
	private IInputDevice _inputDevice;
	private Transform _playerTransform;
	private Rigidbody _playerRigidBody;

	public PlayerMovementStateIdle(PlayerMovementStateMachineController  playerMovementStateMachineController, PlayerMovementController playerMovementController, IInputDevice inputDevice, Transform playerTransform, Rigidbody playerRigidBody)
	{
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerMovementController = playerMovementController;
		_inputDevice = inputDevice;
		_playerTransform = playerTransform;
		_playerRigidBody = playerRigidBody;
		_playerMovementController.ChangePlayerRayPosition(1.9f);
	}

	public override void Update()
	{

		if (!_inputDevice.GetKeyRun() && (_inputDevice.GetKeyUp() || _inputDevice.GetKeyDown() || _inputDevice.GetKeyRight() || _inputDevice.GetKeyLeft()))
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerWalking);
		}

		if (_inputDevice.GetKeyRun() && (_inputDevice.GetKeyUp() || _inputDevice.GetKeyDown() || _inputDevice.GetKeyRight() || _inputDevice.GetKeyLeft()))
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

		if (_inputDevice.GetKeyCrouch())
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
		}
	}
}
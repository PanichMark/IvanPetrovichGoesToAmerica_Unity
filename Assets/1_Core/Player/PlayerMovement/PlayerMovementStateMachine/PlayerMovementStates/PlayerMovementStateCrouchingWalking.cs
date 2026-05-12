using UnityEngine;

public class PlayerMovementStateCrouchingWalking : PlayerMovementStateAbstract
{
	private IInputDevice _inputDevice;
	private Transform _playerTransform;
	private Rigidbody _playerRigidBody;
	private Vector3 _playerWorldMovement;

	public PlayerMovementStateCrouchingWalking(PlayerMovementController playerMovementController, IInputDevice inputDevice, Transform playerTransform, Rigidbody playerRigidBody)
	{
		_playerMovementController = playerMovementController;
		_inputDevice = inputDevice;
		_playerTransform = playerTransform;
		_playerRigidBody = playerRigidBody;

		_playerMovementController.ChangePlayerMovementSpeed(1.8f);
		_playerMovementController.ChangePlayerRayPosition(1.2f);
	}
	public override void Update()
	{
		if (_inputDevice.GetKeyRight())
		{
			_playerWorldMovement.x = 1;
		}
		else if (_inputDevice.GetKeyLeft())
		{
			_playerWorldMovement.x = -1;
		}
		else
		{
			_playerWorldMovement.x = 0;
		}

		if (_inputDevice.GetKeyUp())
		{
			_playerWorldMovement.z = 1;
		}
		else if (_inputDevice.GetKeyDown())
		{
			_playerWorldMovement.z = -1;
		}
		else
		{
			_playerWorldMovement.z = 0;
		}

		_playerMovementController.SetPlayerWorldMovement(_playerWorldMovement);

		if (_playerWorldMovement.x == 0 && _playerWorldMovement.z == 0)
		{
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
		}

		if (_inputDevice.GetKeyJump() && _playerMovementController.IsPlayerGrounded && _playerMovementController.IsPlayerAbleToStandUp)
		{
			
			_playerRigidBody.AddForce(_playerTransform.up * 5f, ForceMode.Impulse);
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerJumping);
		}

		if (_playerMovementController.IsPlayerFalling)
		{
			
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerFalling);
		}

		if (_inputDevice.GetKeyRun() && _playerMovementController.IsPlayerAbleToStandUp)
		{
			
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerRunning);
		}

		if (_inputDevice.GetKeyCrouch() && _playerMovementController.IsPlayerAbleToStandUp)
		{
			
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerWalking);
		}
	}
}
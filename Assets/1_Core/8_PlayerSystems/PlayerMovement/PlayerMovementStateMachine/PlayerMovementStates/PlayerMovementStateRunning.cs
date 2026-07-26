using UnityEngine;

public class PlayerMovementStateRunning : PlayerMovementStateAbstract
{
	private IInputDevice _inputDevice;
	private Transform _playerTransform;
	private Rigidbody _playerRigidBody;
	private Vector3 _playerWorldMovement;

	public PlayerMovementStateRunning(PlayerMovementStateMachineController  playerMovementStateMachineController, PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerMovementController = playerMovementController;
		_inputDevice = inputDevice;
		_playerTransform = _playerMovementController.PlayerTransform;
		_playerRigidBody = _playerMovementController.PlayerRigidBody;

		_playerMovementController.ChangePlayerMovementSpeed(6f);
		_playerMovementController.ChangePlayerRayPosition(1.9f);
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
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerStandingIdle);
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

		if (!_inputDevice.GetKeyRun())
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerStandingWalking);
		}

		if (_inputDevice.GetKeyCrouch())
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerSliding);
		}
	}
}
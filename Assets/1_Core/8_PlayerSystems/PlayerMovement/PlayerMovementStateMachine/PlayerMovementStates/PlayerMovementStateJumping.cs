using UnityEngine;

public class PlayerMovementStateJumping : PlayerMovementStateAbstract
{
	private IInputDevice _inputDevice;
	private Vector3 _playerWorldMovement;

	public PlayerMovementStateJumping(PlayerMovementStateMachineController  playerMovementStateMachineController, PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerMovementController = playerMovementController;
		_inputDevice = inputDevice;
		_playerMovementController.ChangePlayerRayPosition(1.9f);
		_playerMovementController.SetPlayerFloorDetectionRayCastLengthToZero();
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


		if (_playerMovementController.IsPlayerFalling == true)
		{

			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerFalling);
		}

		if (_playerMovementController.IsPlayerGrounded == true)
		{

			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerStandingIdle);
		}

		if (_inputDevice.GetKeyJumpBeingHeld() && _playerMovementController.IsPlayerAbleToClimbLedge)
		{
			_playerMovementStateMachineController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerLedgeClimbing);
		}
	}
}
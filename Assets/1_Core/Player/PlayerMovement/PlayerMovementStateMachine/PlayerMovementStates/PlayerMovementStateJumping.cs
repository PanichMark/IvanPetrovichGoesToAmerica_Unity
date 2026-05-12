using UnityEngine;

public class PlayerMovementStateJumping : PlayerMovementStateAbstract
{
	private IInputDevice _inputDevice;
	private Vector3 _playerWorldMovement;

	public PlayerMovementStateJumping(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		_playerMovementController = playerMovementController;
		_inputDevice = inputDevice;
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


		if (_playerMovementController.IsPlayerFalling == true)
		{

			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerFalling);
		}

		if (_playerMovementController.IsPlayerGrounded == true)
		{

			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
		}

		if (_inputDevice.GetKeyJumpBeingHeld() && _playerMovementController.IsPlayerAbleToClimbLedge)
		{
			_playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerLedgeClimbing);
		}
	}
}
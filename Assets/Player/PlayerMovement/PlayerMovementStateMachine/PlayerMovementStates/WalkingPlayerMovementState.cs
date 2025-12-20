using UnityEngine;

public class WalkingPlayerMovementState: PlayerMovementState
{
	private IInputDevice inputDevice;

	public WalkingPlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		//Debug.Log("Player Walking");

	}
	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerMoving == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyRun() && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);
		}
		
		if (playerMovementController.IsPlayerFalling == true  && inputDevice.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}
		
		if (inputDevice.GetKeyJump())
		{
			WhatSpeedWas = "walking";
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		}
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingWalking);

		}
		if (playerMovementController.IsPlayerMoving == false && inputDevice.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
		}


	}

	public override void ChangePlayerMovementSpeed()
	{
		
			playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerWalkingSpeed);
		
		
	}
}



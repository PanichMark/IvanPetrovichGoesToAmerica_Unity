using UnityEngine;

public class IdlePlayerMovementState : PlayerMovementState
{

	private IInputDevice inputDevice;

	public IdlePlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		//Debug.Log("Player Walking");

	}


	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyRun() == false)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyRun() && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);
		}
		if (inputDevice.GetKeyJump())
		{
			WhatSpeedWas = "walking";

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		}
		if (playerMovementController.IsPlayerFalling == true && inputDevice.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}
		
		if (playerMovementController.IsPlayerMoving == false && inputDevice.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
		}
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingWalking);
		}
		
	}

	public override void ChangePlayerMovementSpeed()
	{
		// just idle
	}

	
}






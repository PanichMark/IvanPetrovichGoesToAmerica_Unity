using UnityEngine;

public class CrouchingIdlePlayerMovementState : PlayerMovementState
{
	private IInputDevice inputDevice;

	public CrouchingIdlePlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		//Debug.Log("Player Walking");

	}
	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingWalking);
		}
		if (playerMovementController.IsPlayerMoving == false && inputDevice.GetKeyCrouch() == true && playerMovementController.IsPlayerAbleToStandUp == true)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyCrouch() && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		if (inputDevice.GetKeyJump() &&
			playerMovementController.IsPlayerGrounded &&
			playerMovementController.IsPlayerAbleToMove &&
			playerMovementController.IsPlayerAbleToStandUp)
		{
			WhatSpeedWas = "crouching";

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		}


	}

	public override void ChangePlayerMovementSpeed()
	{
		playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerCrouchingSpeed);
	}

	
}


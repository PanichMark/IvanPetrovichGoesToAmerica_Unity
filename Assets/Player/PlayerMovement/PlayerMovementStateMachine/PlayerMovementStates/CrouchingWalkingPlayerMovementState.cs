using UnityEngine;

public class CrouchingWalkingPlayerMovementState : PlayerMovementState
{
	private IInputDevice inputDevice;

	public CrouchingWalkingPlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		//Debug.Log("Player Walking");

	}
	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerMoving == false && inputDevice.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
		}
		if (playerMovementController.IsPlayerMoving == false && inputDevice.GetKeyCrouch() == true && playerMovementController.IsPlayerAbleToStandUp == true)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyCrouch() == true && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyRun() && playerMovementController.IsPlayerAbleToStandUp == true
			&& playerMovementController.IsPlayerAbleToMove)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);
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


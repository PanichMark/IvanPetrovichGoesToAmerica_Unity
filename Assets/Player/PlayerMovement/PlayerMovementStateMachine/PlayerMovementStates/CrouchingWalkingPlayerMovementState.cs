using UnityEngine;

public class CrouchingWalkingPlayerMovementState : PlayerMovementState
{
	public CrouchingWalkingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player Crouching Walking");
		
	}
	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerMoving == false && InputManager.Instance.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
		}
		if (playerMovementController.IsPlayerMoving == false && InputManager.Instance.GetKeyCrouch() == true && playerMovementController.IsPlayerAbleToStandUp == true)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyCrouch() == true && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyRun() && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);
		}
		if (InputManager.Instance.GetKeyJump())
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


using UnityEngine;

public class CrouchingIdlePlayerMovementState : PlayerMovementState
{
	public CrouchingIdlePlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player Crouching Idle");
		
	}
	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingWalking);
		}
		if (playerMovementController.IsPlayerMoving == false && InputManager.Instance.GetKeyCrouch() == true && playerMovementController.IsPlayerAbleToStandUp == true)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyCrouch() && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
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


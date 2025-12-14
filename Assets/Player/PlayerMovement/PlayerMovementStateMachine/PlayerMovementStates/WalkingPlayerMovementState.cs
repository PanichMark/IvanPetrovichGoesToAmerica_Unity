using UnityEngine;

public class WalkingPlayerMovementState: PlayerMovementState
{
	
	public WalkingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player Walking");
		
	}
	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerMoving == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyRun() && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);
		}
		
		if (playerMovementController.IsPlayerFalling == true  && InputManager.Instance.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}
		
		if (InputManager.Instance.GetKeyJump())
		{
			WhatSpeedWas = "walking";
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		}
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingWalking);

		}
		if (playerMovementController.IsPlayerMoving == false && InputManager.Instance.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
		}


	}

	public override void ChangePlayerMovementSpeed()
	{
		
			playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerWalkingSpeed);
		
		
	}
}



using UnityEngine;

public class IdlePlayerMovementState : PlayerMovementState
{ 
	
	public IdlePlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		
		//Debug.Log("Player Idle");
		
	}


	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyRun() == false)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyRun() && playerMovementController.IsPlayerAbleToStandUp == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);
		}
		if (InputManager.Instance.GetKeyJump())
		{
			WhatSpeedWas = "walking";

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		}
		if (playerMovementController.IsPlayerFalling == true && InputManager.Instance.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}
		
		if (playerMovementController.IsPlayerMoving == false && InputManager.Instance.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
		}
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingWalking);
		}
		
	}

	public override void ChangePlayerMovementSpeed()
	{
		// just idle
	}

	
}






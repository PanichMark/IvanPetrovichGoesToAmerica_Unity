using UnityEngine;

public class RunningPlayerMovementState : PlayerMovementState
{
	public RunningPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player Running");
		
	}
	public override void ChangePlayerMovementState()
	{
		
		if (playerMovementController.IsPlayerMoving == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		if (playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyRun() == false)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		if (playerMovementController.IsPlayerFalling == true && InputManager.Instance.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}
		if (InputManager.Instance.GetKeyJump())
		{
			WhatSpeedWas = "running";

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		}
		if (playerMovementController.IsPlayerMoving == true && playerMovementController.IsPlayerFalling == false && InputManager.Instance.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerSliding);
		}


	}

	public override void ChangePlayerMovementSpeed()
	{
		playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerRunningSpeed);
	}
}


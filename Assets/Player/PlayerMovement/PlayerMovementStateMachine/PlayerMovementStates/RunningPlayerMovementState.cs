using UnityEngine;

public class RunningPlayerMovementState : PlayerMovementState
{
	private IInputDevice inputDevice;

	public RunningPlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
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
		if (playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyRun() == false)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		if (playerMovementController.IsPlayerFalling == true && inputDevice.GetKeyCrouch() == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}
		if (inputDevice.GetKeyJump())
		{
			WhatSpeedWas = "running";

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		}
		if (playerMovementController.IsPlayerMoving == true && playerMovementController.IsPlayerFalling == false && inputDevice.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerSliding);
		}


	}

	public override void ChangePlayerMovementSpeed()
	{
		playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerRunningSpeed);
	}
}


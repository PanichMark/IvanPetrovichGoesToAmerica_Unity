using UnityEngine;
using System.Collections;

public class JumpingPlayerMovementState : PlayerMovementState
{
	private float progress = 0f;

	private IInputDevice inputDevice;

	public JumpingPlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		//Debug.Log("Player Walking");

	}

	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerFalling == true
			|| playerMovementController.IsPlayerGrounded && playerMovementController.IsPlayerOnSlope && playerMovementController.JumpingStateWait())
		{
			playerMovementController.StopJumpingStateWait();
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}

		if (playerMovementController.IsPlayerAbleToClimbLedge == true && inputDevice.GetKeyJumpBeingHeld())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerLedgeClimbing);
		}

	}

	public override void ChangePlayerMovementSpeed()
	{
		

		// Обновление прогресса плавно
		progress += Time.deltaTime * 1.6f;

		if (WhatSpeedWas == "crouching")
			playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerCrouchingSpeed);


		if (WhatSpeedWas == "walking")
			//playerMovementController.SetPlayerMovementSpeed(Mathf.Lerp(playerMovementController.PlayerWalkingSpeed, playerMovementController.PlayerCrouchingSpeed, progress));
			playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerWalkingSpeed);


		if (WhatSpeedWas == "running")
			playerMovementController.SetPlayerMovementSpeed(Mathf.Lerp(playerMovementController.PlayerRunningSpeed, playerMovementController.PlayerWalkingSpeed, progress));
	}


}


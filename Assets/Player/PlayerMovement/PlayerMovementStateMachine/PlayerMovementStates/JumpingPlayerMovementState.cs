using UnityEngine;
using System.Collections;

public class JumpingPlayerMovementState : PlayerMovementState
{
	private float progress = 0f;

	public JumpingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player Jumping");
		
	}

	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerFalling == true)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}

		if (playerMovementController.IsPlayerAbleToClimbLedge == true && InputManager.Instance.GetKeyJumpBeingHeld())
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


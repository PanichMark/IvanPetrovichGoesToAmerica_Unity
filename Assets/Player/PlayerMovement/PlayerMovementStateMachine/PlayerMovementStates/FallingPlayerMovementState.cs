using UnityEngine;

public class FallingPlayerMovementState : PlayerMovementState
{
	public FallingPlayerMovementState(PlayerMovementController playerMovementController)
	{
	this.playerMovementController = playerMovementController;
	//Debug.Log("Player Falling");
		
	}

	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerFalling == false && playerMovementController.IsPlayerMoving == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		
		if (playerMovementController.IsPlayerFalling == false && playerMovementController.IsPlayerMoving == true && InputManager.Instance.GetKeyRun())
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);

		}
		 if (playerMovementController.IsPlayerFalling == false && playerMovementController.IsPlayerMoving == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		//if (playerMovementController.playerInputsList.GetKeyJump())
		//{
		//	playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		//}

		if (playerMovementController.IsPlayerAbleToClimbLedge == true && InputManager.Instance.GetKeyJumpBeingHeld())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerLedgeClimbing);

		}
	}

	public override void ChangePlayerMovementSpeed()
	{
		playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerCrouchingSpeed);
	}
}


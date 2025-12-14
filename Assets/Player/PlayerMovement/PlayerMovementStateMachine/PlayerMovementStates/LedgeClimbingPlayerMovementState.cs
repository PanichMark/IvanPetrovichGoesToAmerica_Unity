using UnityEngine;

public class LedgeClimbingPlayerMovementState : PlayerMovementState
{
	public LedgeClimbingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player LedgeClimbing");
	
	}

	public override void ChangePlayerMovementState()
	{

		playerMovementController.StartPlayerLedgeClimbing();
	}

	public override void ChangePlayerMovementSpeed()
	{
		// speed is 0
		//playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerCrouchingSpeed);
	}
}


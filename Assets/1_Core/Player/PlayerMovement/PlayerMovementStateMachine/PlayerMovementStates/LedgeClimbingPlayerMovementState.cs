using UnityEngine;

public class LedgeClimbingPlayerMovementState : AbstractPlayerMovementState
{
	public LedgeClimbingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player LedgeClimbing");
		playerMovementController.StartPlayerLedgeClimbing();
	}

	


}



using UnityEngine;

public class LedgeClimbingPlayerMovementState : PlayerMovementState
{
	public LedgeClimbingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player LedgeClimbing");
		playerMovementController.StartPlayerLedgeClimbing();
	}

	


}



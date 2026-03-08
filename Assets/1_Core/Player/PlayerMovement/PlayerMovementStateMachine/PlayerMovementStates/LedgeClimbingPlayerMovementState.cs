using UnityEngine;

public class LedgeClimbingPlayerMovementState : AbstractNPCState
{
	public LedgeClimbingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;
		//Debug.Log("Player LedgeClimbing");
		playerMovementController.StartPlayerLedgeClimbing();
	}

	


}



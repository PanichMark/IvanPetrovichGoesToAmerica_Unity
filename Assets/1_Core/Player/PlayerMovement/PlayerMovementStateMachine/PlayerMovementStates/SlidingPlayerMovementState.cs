

public class SlidingPlayerMovementState : AbstractPlayerMovementState
{
	

	public SlidingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;

		//Debug.Log("Player Sliding");
		playerMovementController.ChangePlayerMovementSpeed(0f);
		playerMovementController.ChangePlayerRotationSpeed(0);
		playerMovementController.ChangePlayerRayPosition(1.2f);
		playerMovementController.StartPlayerSliding();

	}
	


	
}


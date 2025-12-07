

public class SlidingPlayerMovementState : PlayerMovementState
{
	

	public SlidingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;

		//Debug.Log("Player Sliding");
		
	}
	public override void ChangePlayerMovementState()
	{
		if (playerMovementController.IsPlayerAbleToSlide == true)
		{
			
			playerMovementController.StartPlayerSliding();
		}


	}

	public override void ChangePlayerMovementSpeed()
	{
		playerMovementController.SetPlayerMovementSpeed(playerMovementController.PlayerSlidingSpeed);
	}
	
}
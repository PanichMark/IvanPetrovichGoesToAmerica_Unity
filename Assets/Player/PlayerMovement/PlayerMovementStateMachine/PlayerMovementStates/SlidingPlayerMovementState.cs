

public class SlidingPlayerMovementState : PlayerMovementState
{
	

	public SlidingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		this.playerMovementController = playerMovementController;

		//Debug.Log("Player Sliding");
		
	}
	public override void Update()
	{
		if (playerMovementController.IsPlayerAbleToSlide == true)
		{
			
			playerMovementController.StartPlayerSliding();
		}


	}


	
}

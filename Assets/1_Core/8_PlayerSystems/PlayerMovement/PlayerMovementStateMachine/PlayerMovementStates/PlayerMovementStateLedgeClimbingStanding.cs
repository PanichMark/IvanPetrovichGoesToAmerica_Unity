public class PlayerMovementStateLedgeClimbingStanding : PlayerMovementStateAbstract
{
	public PlayerMovementStateLedgeClimbingStanding(PlayerMovementController playerMovementController)
	{
		_playerMovementController = playerMovementController;
		_playerMovementController.StartPlayerLedgeClimbing();
	}
}
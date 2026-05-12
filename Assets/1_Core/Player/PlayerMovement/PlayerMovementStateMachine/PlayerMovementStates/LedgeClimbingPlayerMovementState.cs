public class LedgeClimbingPlayerMovementState : AbstractPlayerMovementState
{
	public LedgeClimbingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		_playerMovementController = playerMovementController;
		_playerMovementController.StartPlayerLedgeClimbing();
	}
}
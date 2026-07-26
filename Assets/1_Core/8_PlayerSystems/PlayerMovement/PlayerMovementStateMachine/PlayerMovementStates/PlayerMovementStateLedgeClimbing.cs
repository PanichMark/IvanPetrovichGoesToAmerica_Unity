public class PlayerMovementStateLedgeClimbing : PlayerMovementStateAbstract
{
	public PlayerMovementStateLedgeClimbing(PlayerMovementController playerMovementController)
	{
		_playerMovementController = playerMovementController;
		_playerMovementController.StartPlayerLedgeClimbing();
	}
}
public class PlayerMovementStateSliding : PlayerMovementStateAbstract
{
	public PlayerMovementStateSliding(PlayerMovementController playerMovementController)
	{
		_playerMovementController = playerMovementController;

		_playerMovementController.ChangePlayerMovementSpeed(0f, true);
		_playerMovementController.ChangePlayerRotationSpeed(0);
		_playerMovementController.ChangePlayerRayPosition(1.2f);
		_playerMovementController.StartPlayerSliding();
	}
}
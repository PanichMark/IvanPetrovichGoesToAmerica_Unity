public class SlidingPlayerMovementState : AbstractPlayerMovementState
{
	public SlidingPlayerMovementState(PlayerMovementController playerMovementController)
	{
		_playerMovementController = playerMovementController;

		_playerMovementController.ChangePlayerMovementSpeed(0f);
		_playerMovementController.ChangePlayerRotationSpeed(0);
		_playerMovementController.ChangePlayerRayPosition(1.2f);
		_playerMovementController.StartPlayerSliding();

	}
}
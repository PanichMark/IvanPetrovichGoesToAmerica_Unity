using UnityEngine;

public class PlayerMovementStateVaultingCrouching : PlayerMovementStateAbstract
{
	public PlayerMovementStateVaultingCrouching(PlayerMovementController playerMovementController)
	{
		_playerMovementController = playerMovementController;
		//_playerMovementController.StartPlayerLedgeClimbing();
	}
}

using UnityEngine;

public class PlayerMovementStateVaultingStanding: PlayerMovementStateAbstract
{
	public PlayerMovementStateVaultingStanding(PlayerMovementController playerMovementController)
	{
		_playerMovementController = playerMovementController;
		//_playerMovementController.StartPlayerLedgeClimbing();
	}
}

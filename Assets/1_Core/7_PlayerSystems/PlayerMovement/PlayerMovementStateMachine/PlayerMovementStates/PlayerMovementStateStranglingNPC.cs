using UnityEngine;

public class PlayerMovementStateStranglingNPC : PlayerMovementStateAbstract
{
	public PlayerMovementStateStranglingNPC(
		PlayerMovementStateMachineController playerMovementStateMachineController,
		PlayerMovementController playerMovementController)
	{
		_playerMovementStateMachineController = playerMovementStateMachineController;
		_playerMovementController = playerMovementController;

		_playerMovementController.ChangePlayerRayPosition(1.9f);
		_playerMovementController.StopPlayerRigidBodyVelocity();
		_playerMovementController.SetPlayerFloorDetectionRayCastLengthToDefault();
	}
}

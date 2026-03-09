using UnityEngine;

public class CrouchingIdlePlayerMovementState : AbstractPlayerMovementState
{
	private IInputDevice inputDevice;
	private Transform playerTransform;
	private Rigidbody playerRigidBody;

	public CrouchingIdlePlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice, Transform playerTransform, Rigidbody playerRigidBody)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		this.playerTransform = playerTransform;
		this.playerRigidBody = playerRigidBody;

		playerMovementController.ChangePlayerRayPosition(1.2f);
		playerMovementController.ChangePlayerRotationSpeed(300f);
	}
	public override void Update()
	{
		if ((inputDevice.GetKeyUp() || inputDevice.GetKeyDown() || inputDevice.GetKeyRight() || inputDevice.GetKeyLeft()))
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingWalking);
		}

		if (inputDevice.GetKeyRun() && (inputDevice.GetKeyUp() || inputDevice.GetKeyDown() || inputDevice.GetKeyRight() || inputDevice.GetKeyLeft()) && playerMovementController.IsPlayerAbleToStandUp)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerRunning);
		}

		if (inputDevice.GetKeyJump() && playerMovementController.IsPlayerGrounded && playerMovementController.IsPlayerAbleToStandUp)
		{
			
			playerRigidBody.AddForce(playerTransform.up * 5f, ForceMode.Impulse);
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerJumping);
		}

		if (playerMovementController.IsPlayerFalling)
		{
			
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerFalling);
		}

		if (inputDevice.GetKeyCrouch() && playerMovementController.IsPlayerAbleToStandUp)
		{
			
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerIdle);
		}
	}
}

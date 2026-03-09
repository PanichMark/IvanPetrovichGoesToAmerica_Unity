using UnityEngine;

public class IdlePlayerMovementState : AbstractPlayerMovementState
{
	private IInputDevice inputDevice;
	private Transform playerTransform;
	private Rigidbody playerRigidBody;

	public IdlePlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice, Transform playerTransform, Rigidbody playerRigidBody)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		this.playerTransform = playerTransform;
		this.playerRigidBody = playerRigidBody;
		playerMovementController.ChangePlayerRayPosition(1.9f);
	}

	public override void Update()
	{

		if (!inputDevice.GetKeyRun() && (inputDevice.GetKeyUp() || inputDevice.GetKeyDown() || inputDevice.GetKeyRight() || inputDevice.GetKeyLeft()))
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerWalking);
		}

		if (inputDevice.GetKeyRun() && (inputDevice.GetKeyUp() || inputDevice.GetKeyDown() || inputDevice.GetKeyRight() || inputDevice.GetKeyLeft()))
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

		if (inputDevice.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateTypes.PlayerCrouchingIdle);
		}
	}
}

using UnityEngine;

public class IdlePlayerMovementState : PlayerMovementState
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
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}

		if (inputDevice.GetKeyRun() && (inputDevice.GetKeyUp() || inputDevice.GetKeyDown() || inputDevice.GetKeyRight() || inputDevice.GetKeyLeft()))
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);
		}

		if (inputDevice.GetKeyJump() && playerMovementController.IsPlayerGrounded && playerMovementController.IsPlayerAbleToStandUp)
		{
			playerRigidBody.AddForce(playerTransform.up * 5f, ForceMode.Impulse);
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		}

		if (playerMovementController.IsPlayerFalling)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerFalling);
		}

		if (inputDevice.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerCrouchingIdle);
		}
	}
}
using UnityEngine;

public class RunningPlayerMovementState : PlayerMovementState
{
	private IInputDevice inputDevice;
	private Transform playerTransform;
	private Rigidbody playerRigidBody;
	private Vector3 playerWorldMovement;

	public RunningPlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice, Transform playerTransform, Rigidbody playerRigidBody)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		this.playerTransform = playerTransform;
		this.playerRigidBody = playerRigidBody;

		playerMovementController.ChangePlayerMovementSpeed(6f);
		playerMovementController.ChangePlayerRayPosition(1.9f);
	}
	public override void Update()
	{
		if (inputDevice.GetKeyRight())
		{
			playerWorldMovement.x = 1;
		}
		else if (inputDevice.GetKeyLeft())
		{
			playerWorldMovement.x = -1;
		}
		else
		{
			playerWorldMovement.x = 0;
		}

		if (inputDevice.GetKeyUp())
		{
			playerWorldMovement.z = 1;
		}
		else if (inputDevice.GetKeyDown())
		{
			playerWorldMovement.z = -1;
		}
		else
		{
			playerWorldMovement.z = 0;
		}

		playerMovementController.SetPlayerWorldMovement(playerWorldMovement);

		if (playerWorldMovement.x == 0 && playerWorldMovement.z == 0)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
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

		if (!inputDevice.GetKeyRun())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}

		if (inputDevice.GetKeyCrouch())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerSliding);
		}
	}
}
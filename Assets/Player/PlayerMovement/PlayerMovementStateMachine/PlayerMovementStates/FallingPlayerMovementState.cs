using UnityEngine;

public class FallingPlayerMovementState : PlayerMovementState
{
	private IInputDevice inputDevice;

	public FallingPlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		//Debug.Log("Player Walking");

	}

	public override void Update()
	{
		if (playerMovementController.IsPlayerFalling == false && playerMovementController.IsPlayerMoving == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		
		if (playerMovementController.IsPlayerFalling == false && playerMovementController.IsPlayerMoving == true && inputDevice.GetKeyRun() && playerMovementController.IsPlayerAbleToMove)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);

		}
		 if (playerMovementController.IsPlayerFalling == false && playerMovementController.IsPlayerMoving == true)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		//if (playerMovementController.playerInputsList.GetKeyJump())
		//{
		//	playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		//}

		if (playerMovementController.IsPlayerAbleToClimbLedge == true && inputDevice.GetKeyJumpBeingHeld())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerLedgeClimbing);

		}
	}


}


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
		if (playerMovementController.IsPlayerFalling == false)
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerIdle);
		}
		
		if (playerMovementController.IsPlayerFalling == false && inputDevice.GetKeyRun())
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerRunning);

		}
		 if (playerMovementController.IsPlayerFalling == false)
		{

			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerWalking);
		}
		//if (playerMovementController.playerInputsList.GetKeyJump())
		//{
		//	playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerJumping);
		//}

		 /*
		if (inputDevice.GetKeyJumpBeingHeld())
		{
			playerMovementController.SetPlayerMovementState(PlayerMovementStateType.PlayerLedgeClimbing);

		}
		 */
	}


}



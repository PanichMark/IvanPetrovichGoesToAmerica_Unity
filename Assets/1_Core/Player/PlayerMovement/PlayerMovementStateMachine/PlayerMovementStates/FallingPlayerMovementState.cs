using UnityEngine;

public class FallingPlayerMovementState : AbstractNPCState
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
			playerMovementController.SetPlayerMovementState(NPCStateTypes.PlayerIdle);
		}
		
		if (playerMovementController.IsPlayerFalling == false && inputDevice.GetKeyRun())
		{

			playerMovementController.SetPlayerMovementState(NPCStateTypes.PlayerRunning);

		}
		 if (playerMovementController.IsPlayerFalling == false)
		{

			playerMovementController.SetPlayerMovementState(NPCStateTypes.PlayerWalking);
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



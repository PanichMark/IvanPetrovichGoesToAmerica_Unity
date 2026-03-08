using UnityEngine;
using System.Collections;

public class JumpingPlayerMovementState : AbstractNPCState
{
	

	private IInputDevice inputDevice;
	private Vector3 playerWorldMovement;

	public JumpingPlayerMovementState(PlayerMovementController playerMovementController, IInputDevice inputDevice)
	{
		this.playerMovementController = playerMovementController;
		this.inputDevice = inputDevice;
		//Debug.Log("Player Walking");
		this.playerMovementController.ChangePlayerRayPosition(1.9f);
	
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


		
		if (playerMovementController.IsPlayerFalling == true)
		{
			
			playerMovementController.SetPlayerMovementState(NPCStateTypes.PlayerFalling);
		}
		


		if (playerMovementController.IsPlayerGrounded == true)
		{

			playerMovementController.SetPlayerMovementState(NPCStateTypes.PlayerIdle);
		}

		if ( inputDevice.GetKeyJumpBeingHeld() && playerMovementController.IsPlayerAbleToClimbLedge)
		{
			playerMovementController.SetPlayerMovementState(NPCStateTypes.PlayerLedgeClimbing);
		}
		

	}

	

}



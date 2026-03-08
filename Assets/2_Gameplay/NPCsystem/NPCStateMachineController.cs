using UnityEngine;
using UnityEngine.XR;

public class NPCStateMachineController : MonoBehaviour
{
	private AbstractNPCState NPCstate;
	private NPCStateTypes NPCStateType;
	public string CurrentNPCState { get; private set; } = "PlayerIdle";
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

	public void SetPlayerMovementState(NPCStateTypes playerMovementStateType)
	{
		
			AbstractNPCState newState;

			if (playerMovementStateType == NPCStateTypes.Default)
			{
				newState = new DefaultNPCState();
				CurrentNPCState = "PlayerIdle";
			}
			else if (playerMovementStateType == NPCStateTypes.Interested)
			{
				newState = new InterestedNPCState();
				CurrentNPCState = "PlayerWalking";
			}
			else if (playerMovementStateType == NPCStateTypes.Alarmed)
			{
				newState = new AlarmedNPCState();
				CurrentNPCState = "PlayerRunning";
			}
			else if (playerMovementStateType == NPCStateTypes.Chasing)
			{
				
				newState = new ChasingNPCState();
				CurrentNPCState = "PlayerJumping";
			}
			else if (playerMovementStateType == NPCStateTypes.Attacking)
			{
				
				newState = new AttackingNPCState();
				CurrentNPCState = "PlayerFalling";
			}
			else if (playerMovementStateType == NPCStateTypes.Searching)
			{
				newState = new SearchingNPCState();
				CurrentNPCState = "PlayerCrouchingIdle";
			}
			else if (playerMovementStateType == NPCStateTypes.Scared)
			{
				newState = new ScaredNPCState();
				CurrentNPCState = "PlayerSliding";
			}
			else if (playerMovementStateType == NPCStateTypes.Fleeing)
			{
				newState = new FleeingNPCState();
				CurrentNPCState = "PlayerCrouchingWalking";
			}
			else
			{
				newState = null;
			}
			NPCstate = newState;

			Debug.Log("MovementState: " + CurrentNPCState);
		}
	}


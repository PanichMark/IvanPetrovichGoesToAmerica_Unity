using UnityEngine;
using UnityEngine.XR;

public class NPCStateMachineController : MonoBehaviour
{
	[SerializeField] private NPCStateTypes initialState = NPCStateTypes.Default;

	private AbstractNPCState NPCstate;
	private NPCStateTypes NPCStateType;
	private NPCAbstract NPCabstract;
	public string CurrentNPCState { get; private set; } = "PlayerIdle";
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
	
		
		
		NPCabstract = GetComponent<NPCAbstract>();

		//SetPlayerMovementState(NPCStateTypes.Default);

		SetNPCState(initialState); // Применяем выбранное состояние
	}

	public void SetNPCState(NPCStateTypes playerMovementStateType)
	{
		
			AbstractNPCState newState;

			if (playerMovementStateType == NPCStateTypes.Default)
			{
				newState = new DefaultNPCState();
				CurrentNPCState = "Default";
				NPCabstract.gameObject.tag = "Interactable";
			}
			else if (playerMovementStateType == NPCStateTypes.Interested)
			{
				newState = new InterestedNPCState();
				//CurrentNPCState = "PlayerWalking";
			}
			else if (playerMovementStateType == NPCStateTypes.Alarmed)
			{
				newState = new AlarmedNPCState();
				//CurrentNPCState = "PlayerRunning";
			}
			else if (playerMovementStateType == NPCStateTypes.Chasing)
			{
				
				newState = new ChasingNPCState();
			//	CurrentNPCState = "PlayerJumping";
			}
			else if (playerMovementStateType == NPCStateTypes.Attacking)
			{
				
				newState = new AttackingNPCState();
				//CurrentNPCState = "PlayerFalling";
			}
			else if (playerMovementStateType == NPCStateTypes.Searching)
			{
				newState = new SearchingNPCState();
				//CurrentNPCState = "PlayerCrouchingIdle";
			}
			else if (playerMovementStateType == NPCStateTypes.Scared)
			{
				newState = new ScaredNPCState();
				CurrentNPCState = "Scared";
				NPCabstract.gameObject.tag = "Untagged";
			}
			else if (playerMovementStateType == NPCStateTypes.Fleeing)
			{
				newState = new FleeingNPCState();
				//CurrentNPCState = "PlayerCrouchingWalking";
			}
			else if (playerMovementStateType == NPCStateTypes.Choked)
			{
				newState = new ChokedNPCState();
				//CurrentNPCState = "PlayerCrouchingWalking";
			}
			else if (playerMovementStateType == NPCStateTypes.Dead)
			{
				NPCabstract.SetHealthToZero();
				NPCabstract.ConvertToPickableObject();
				newState = new DeadNPCState();
				//CurrentNPCState = "PlayerCrouchingWalking";
			}
		else
			{
				newState = null;
			}
			NPCstate = newState;

			//Debug.Log("MovementState: " + CurrentNPCState);
		}
	}


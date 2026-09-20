using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCstateMachineController : MonoBehaviour
{
	public delegate void NPCstateHandler(NPCstateTypes newState);
	public event NPCstateHandler OnNewNPCstate;

	[SerializeField] private NPCstateTypes _initialState = NPCstateTypes.StationaryAction;


	private NPCmovementController _NPCmovementController;
	//[SerializeField] private List<GameObject> _anchorPoints = new List<GameObject>();
	

	private NPCstateAbstract _NPCstate;
	private NPCstateTypes _NPCstateType;
	private NPClivingBeing _NPClivingBeing;



	public NPCstateTypes CurrentNPCState { get; private set; }

	//public List<GameObject> AnchorPoints => _anchorPoints;
	
	

	public void Initialize(
		NPClivingBeing NPClivingBeing,
		NPCmovementController NPCmovementController)
	{
		_NPClivingBeing = NPClivingBeing;
		_NPCmovementController= NPCmovementController;

		//Debug.Log(_initialState);
		SetNPCState(_initialState);

		/*
		if (_initialState == NPCStateTypes.Dead)
			TurnNavmeshOff();
		else
			TurnNavmeshOn();
		*/
	}



	



	private void Update()
	{
		_NPCstate.Update();
	}








	/*
	public void SetNPCState(NPCstateTypes stateType, float animDuration)
	{
		_animationDuration = animDuration;
		SetNPCState(stateType);
	}
	*/

	public void SetNPCState(NPCstateTypes NPCstateType)
	{
		NPCstateAbstract newState;

		if (NPCstateType == NPCstateTypes.StationaryAction)
		{
			//newState = new NPCstateStationaryAction(this, _animationDuration);
			newState = new NPCstateStationaryAction(this, _NPCmovementController);
			CurrentNPCState = NPCstateTypes.StationaryAction;

			if (_NPClivingBeing is not NPCaggressive)
			{
				_NPClivingBeing.gameObject.tag = "Interactable";
			}
		}
		else if (NPCstateType == NPCstateTypes.Patrolling)
		{
			newState = new NPCstatePatrolling(this, _NPCmovementController);
			CurrentNPCState = NPCstateTypes.Patrolling;

			if (_NPClivingBeing is not NPCaggressive)
			{
				_NPClivingBeing.gameObject.tag = "Interactable";
			}
		}
		else if (NPCstateType == NPCstateTypes.Interested)
		{
			newState = new NPCstateInterested();
		}
		else if (NPCstateType == NPCstateTypes.Alarmed)
		{
			newState = new NPCstateAlarmed();
		}
		else if (NPCstateType == NPCstateTypes.Chasing)
		{
			newState = new NPCstateChasing();
		}
		else if (NPCstateType == NPCstateTypes.Attacking)
		{
			newState = new NPCstateAttacking();
		}
		else if (NPCstateType == NPCstateTypes.Reloading)
		{
			newState = new NPCstateReloading();
		}
		else if (NPCstateType == NPCstateTypes.Searching)
		{
			newState = new NPCstateSearching();
		}
		else if (NPCstateType == NPCstateTypes.Huddled)
		{
			newState = new NPCstateHuddled();
			//CurrentNPCState = "Scared";
			_NPClivingBeing.gameObject.tag = "Untagged";
		}
		else if (NPCstateType == NPCstateTypes.Hysteric)
		{
			newState = new NPCstateHysteric();
		}
		else if (NPCstateType == NPCstateTypes.Fleeing)
		{
			newState = new NPCstateFleeing();
		}
		else if (NPCstateType == NPCstateTypes.Strangled)
		{
			newState = new NPCstateStrangled(this, _NPCmovementController);
		}
		else if (NPCstateType == NPCstateTypes.Hooked)
		{
			newState = new NPCstateHooked(this, _NPCmovementController);
			CurrentNPCState = NPCstateTypes.Hooked;
		}
		else if (NPCstateType == NPCstateTypes.Staggered)
		{
			newState = new NPCstateStaggered();
		}
		else if (NPCstateType == NPCstateTypes.KnockedOff)
		{
			newState = new NPCstateKnockedOff();
		}
		else if (NPCstateType == NPCstateTypes.BlownAway)
		{
			newState = new NPCstateBlownAway();
		}
		else if (NPCstateType == NPCstateTypes.ElectroShocked)
		{
			newState = new NPCstateElectroShocked();
		}		
		else if (NPCstateType == NPCstateTypes.Falling)
		{
			newState = new NPCstateFalling();
		}
		else if (NPCstateType == NPCstateTypes.StandingUp)
		{
			newState = new NPCstateStandingUp();
		}
		else if (NPCstateType == NPCstateTypes.Carried)
		{
			newState = new NPCstateCarried();
		}
		else if (NPCstateType == NPCstateTypes.Dizzy)
		{
			newState = new NPCstateBlownAway();
		}
		else if (NPCstateType == NPCstateTypes.Unconscious)
		{
			newState = new NPCstateUnconscious();
		}
		else if (NPCstateType == NPCstateTypes.Dying)
		{
			newState = new NPCstateDying();
		}
		else if (NPCstateType == NPCstateTypes.Dead)
		{
			newState = new NPCstateDead(_NPCmovementController);

			_NPClivingBeing.ConvertToPickableObject();

			CurrentNPCState = NPCstateTypes.Dead;
		}
		else
		{
			Debug.Log("Invalid state type!");
			return;
		}

		_NPCstate = newState;

		OnNewNPCstate?.Invoke(NPCstateType);
		//_NPCabstract.ShowNPCcurrentState(CurrentNPCState);
	}
}
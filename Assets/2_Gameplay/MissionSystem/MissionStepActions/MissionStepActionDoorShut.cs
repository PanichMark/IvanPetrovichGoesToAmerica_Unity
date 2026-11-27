using UnityEngine;

public class MissionStepActionDoorShut : MonoBehaviour, IMissionStepAction
{
	[SerializeField] private MissionStep _linkedMissionStep;
	private InteractionObjectOpenableDoorUndestructable _doorController;
	[SerializeField] private bool _shutDoor;

	private void Start()
	{
		_doorController = GetComponent<InteractionObjectOpenableDoorUndestructable>();
		_linkedMissionStep.RegisterAction(this);

	}

	public void PerformAction()
	{
		if (_shutDoor == true)
		{
			_doorController.ShutDoor();
		}
		else
		{
			_doorController.UnshutDoor();
		}
	}
}
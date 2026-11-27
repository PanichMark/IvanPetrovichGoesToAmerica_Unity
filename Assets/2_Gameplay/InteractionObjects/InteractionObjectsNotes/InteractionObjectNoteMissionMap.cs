using UnityEngine;

public class InteractionObjectNoteMissionMap : InteractionObjectNote
{
	private MissionMapsController _missionMapsController;
	[SerializeField] private MissionMapData _missionMapID;

	protected override void InitializeNote()
	{
		_missionMapsController = ServiceLocator.Resolve<MissionMapsController>();
	}

	public override void Interact()
	{
		base.Interact();

		_missionMapsController.AddMissionMapToCollected(_missionMapID.MissionMapID.ToString());
	}
}
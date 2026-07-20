using UnityEngine;

public class ViewModelHUDMission
{
	public GameObject ImageMissionGoalMarker;
	public GameObject HUDmission;
	public ViewModelHUDMission(Bootstrap bootstrap, GameObject canvas)
	{
		HUDmission = bootstrap.FindDeepGameObject(canvas, "HUDmission");

		ImageMissionGoalMarker = bootstrap.FindDeepGameObject(canvas, "MissionGoalMarker");
	}
}
using UnityEngine;

public class ViewModelHUDMission
{
	public GameObject ImageMissionGoalMarker;
	public ViewModelHUDMission(Bootstrap bootstrap, GameObject canvas)
	{
		ImageMissionGoalMarker = bootstrap.FindDeepGameObject(canvas, "MissionGoalMarker");
	}
}

using UnityEngine;

public class ViewModelHUDMission
{
	public GameObject HUDmission;

	public GameObject ImageMissionGoalMarker;

	public GameObject TextNewMissionGoal;
	public GameObject TextNewMissionGoalDisplay;

	public ViewModelHUDMission(Bootstrap bootstrap, GameObject canvas)
	{
		HUDmission = bootstrap.FindDeepGameObject(canvas, "HUDmission");

		ImageMissionGoalMarker = bootstrap.FindDeepGameObject(canvas, "MissionGoalMarker");

		TextNewMissionGoal = bootstrap.FindDeepGameObject(canvas, "TextNewMissionGoal");
		TextNewMissionGoalDisplay = bootstrap.FindDeepGameObject(canvas, "TextNewMissionGoalDisplay");
	}
}
using UnityEngine;

public class ViewModelHUDMission : IViewModel
{
	public GameObject ImageMissionGoalMarker;
	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ImageMissionGoalMarker = bootstrap.FindDeepGameObject(canvas, "MissionGoalMarker");
	}
}

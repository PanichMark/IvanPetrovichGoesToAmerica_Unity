using UnityEngine;

public class ViewModelMainMenuChooseMission
{
	public GameObject TextMainMenuChooseMission;

	public GameObject[] Missions = new GameObject[3];
	public GameObject[] TextsMissionsNames = new GameObject[3];
	public GameObject[] TextsScenesNames = new GameObject[3];

	public GameObject ButtonCloseMainMenuChooseMission;
	public GameObject TextButtonCloseMainMenuChooseMission;

	public ViewModelMainMenuChooseMission(Bootstrap bootstrap, GameObject canvas)
	{
		TextMainMenuChooseMission = bootstrap.FindDeepGameObject(canvas, "TextMainMenuChooseMission");

		for (int i = 0; i < Missions.Length; i++)
		{
			string missions = "Mission0." + (i + 1);
			string textsMissionsNames = "TextMissionName";
			string textsScenesNames = "TextSceneName";

			Missions[i] = bootstrap.FindDeepGameObject(canvas, missions);
			TextsMissionsNames[i] = bootstrap.FindDeepGameObject(Missions[i], textsMissionsNames);
			TextsScenesNames[i] = bootstrap.FindDeepGameObject(Missions[i], textsScenesNames);
		}

		ButtonCloseMainMenuChooseMission = bootstrap.FindDeepGameObject(canvas, "ButtonCloseMainMenuChooseMission");
		TextButtonCloseMainMenuChooseMission = bootstrap.FindDeepGameObject(canvas, "TextButtonCloseMainMenuChooseMission");
	}
}

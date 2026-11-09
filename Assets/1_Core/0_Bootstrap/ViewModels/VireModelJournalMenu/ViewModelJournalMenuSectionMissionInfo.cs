using UnityEngine;

public class ViewModelJournalMenuSectionMissionInfo
{
	public GameObject CurrentMissionStep;

	public ViewModelJournalMenuSectionMissionInfo(Bootstrap bootstrap, GameObject canvas)
	{
		CurrentMissionStep = bootstrap.FindDeepGameObject(canvas, "CurrentMissionStep");
	}
}

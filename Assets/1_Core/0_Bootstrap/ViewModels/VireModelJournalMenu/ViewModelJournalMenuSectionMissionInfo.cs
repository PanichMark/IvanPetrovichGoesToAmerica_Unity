using UnityEngine;

public class ViewModelJournalMenuSectionMissionInfo
{
	public GameObject TextCurrentMissionStep;

	public ViewModelJournalMenuSectionMissionInfo(Bootstrap bootstrap, GameObject canvas)
	{
		TextCurrentMissionStep = bootstrap.FindDeepGameObject(canvas, "TextCurrentMissionStep");
	}
}

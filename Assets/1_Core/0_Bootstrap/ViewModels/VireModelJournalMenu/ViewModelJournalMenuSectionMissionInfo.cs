using UnityEngine;

public class ViewModelJournalMenuSectionMissionInfo
{
	public GameObject TextMissionStep;
	public GameObject ImageMissionTarget;
	public GameObject TextMissionDescription;
	public GameObject TextMissionName;

	public ViewModelJournalMenuSectionMissionInfo(Bootstrap bootstrap, GameObject canvas)
	{
		TextMissionStep = bootstrap.FindDeepGameObject(canvas, "TextMissionStep");
		ImageMissionTarget = bootstrap.FindDeepGameObject(canvas, "ImageMissionTarget");
		TextMissionDescription = bootstrap.FindDeepGameObject(canvas, "TextMissionDescription");
		TextMissionName = bootstrap.FindDeepGameObject(canvas, "TextMissionName");
	}
}

using UnityEngine;

public class ViewModelJournalMenuSectionMissionInfo
{
	public GameObject TextMissionStep;
	public GameObject ImageMissionTarget;
	public GameObject TextMissionDescription;
	public GameObject TextMissionName;

	public GameObject TextMissionMapName;
	public GameObject ImageMissionMap;
	public GameObject TextMissionMapDescription;
	public GameObject ButtonNextMissionMap;
	public GameObject ButtonPreviousMissionMap;

	public ViewModelJournalMenuSectionMissionInfo(Bootstrap bootstrap, GameObject canvas)
	{
		TextMissionStep = bootstrap.FindDeepGameObject(canvas, "TextMissionStep");
		ImageMissionTarget = bootstrap.FindDeepGameObject(canvas, "ImageMissionTarget");
		TextMissionDescription = bootstrap.FindDeepGameObject(canvas, "TextMissionDescription");
		TextMissionName = bootstrap.FindDeepGameObject(canvas, "TextMissionName");

		TextMissionMapName = bootstrap.FindDeepGameObject(canvas, "TextMissionMapName");
		ImageMissionMap = bootstrap.FindDeepGameObject(canvas, "ImageMissionMap");
		TextMissionMapDescription = bootstrap.FindDeepGameObject(canvas, "TextMissionMapDescription");
		ButtonNextMissionMap = bootstrap.FindDeepGameObject(canvas, "ButtonNextMissionMap");
		ButtonPreviousMissionMap = bootstrap.FindDeepGameObject(canvas, "ButtonPreviousMissionMap");
	}
}

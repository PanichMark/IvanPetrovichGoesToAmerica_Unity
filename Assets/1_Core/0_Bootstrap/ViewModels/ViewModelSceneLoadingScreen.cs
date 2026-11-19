using UnityEngine;

public class ViewModelSceneLoadingScreen
{
	public GameObject ImageScene;

	public GameObject LoadingScreenInfo;

	public GameObject TextMissionName;
	public GameObject TextSceneName;
	public GameObject TextSceneDescription;
	public GameObject TextLoadingIsReady;

	public GameObject SliderSceneLoadingStatus;

	public ViewModelSceneLoadingScreen(Bootstrap bootstrap, GameObject canvas)
	{
		LoadingScreenInfo = bootstrap.FindDeepGameObject(canvas, "LoadingScreenInfo");

		ImageScene = bootstrap.FindDeepGameObject(canvas, "ImageScene");

		TextMissionName = bootstrap.FindDeepGameObject(canvas, "TextMissionName");
		TextSceneName = bootstrap.FindDeepGameObject(canvas, "TextSceneName");
		TextSceneDescription = bootstrap.FindDeepGameObject(canvas, "TextSceneDescription");
		TextLoadingIsReady = bootstrap.FindDeepGameObject(canvas, "TextLoadingIsReady");

		SliderSceneLoadingStatus = bootstrap.FindDeepGameObject(canvas, "SliderSceneLoadingStatus");
	}
}

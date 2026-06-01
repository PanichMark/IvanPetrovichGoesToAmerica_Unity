using UnityEngine;

public class ViewModelPauseSubMenuLoad
{
	public GameObject TextPauseSubMenuLoad;

	public GameObject[] ButtonsLoadGameFile = new GameObject[5];
	public GameObject[] TextGameFileDateAndTime = new GameObject[5];
	public GameObject[] TextGameFileSceneName = new GameObject[5];
	public GameObject[] ImageSceneGameFile = new GameObject[5];

	public GameObject ButtonClosePauseSubMenuLoad;
	public GameObject TextButtonClosePauseSubMenuLoad;

	public ViewModelPauseSubMenuLoad(Bootstrap bootstrap, GameObject canvas)
	{
		TextPauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "TextPauseSubMenuLoad");

		for (int i = 0; i < 5; i++)
		{
			string buttonLoadGameFile = "ButtonLoadGameFile" + (i + 1);
			string textDateAndTimeGameFile = "TextDateAndTimeGameFile" + (i + 1);
			string textSceneNameGameFile = "TextSceneNameGameFile" + (i + 1);
			string imageSceneGameFile = "ImageSceneGameFile" + (i + 1);

			ButtonsLoadGameFile[i] = bootstrap.FindDeepGameObject(canvas, buttonLoadGameFile);
			TextGameFileDateAndTime[i] = bootstrap.FindDeepGameObject(canvas, textDateAndTimeGameFile);
			TextGameFileSceneName[i] = bootstrap.FindDeepGameObject(canvas, textSceneNameGameFile);
			ImageSceneGameFile[i] = bootstrap.FindDeepGameObject(canvas, imageSceneGameFile);
		}

		ButtonClosePauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuLoad");
		TextButtonClosePauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "TextButtonClosePauseSubMenuLoad");
	}
}
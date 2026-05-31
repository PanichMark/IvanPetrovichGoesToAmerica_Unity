using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuLoad
{
	public GameObject[] ButtonsLoadGameFile = new GameObject[5];
	public GameObject ButtonClosePauseSubMenuLoad;

	public ViewModelPauseSubMenuLoad(Bootstrap bootstrap, GameObject canvas)
	{
		for (int i = 0; i < 5; i++)
		{
			string loadName = "ButtonLoadGameFile" + (i + 1);
			ButtonsLoadGameFile[i] = bootstrap.FindDeepGameObject(canvas, loadName);
		}

		ButtonClosePauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuLoad");
	}
}
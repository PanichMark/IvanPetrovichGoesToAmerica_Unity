using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuLoad : IViewModel
{
	public Button[] ButtonsLoadGameFile = new Button[5];
	public Button ButtonClosePauseSubMenuLoad;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		for (int i = 0; i < 5; i++)
		{
			string loadName = "ButtonLoadGameFile" + (i + 1);
			ButtonsLoadGameFile[i] = bootstrap.FindDeepGameObject(canvas, loadName).GetComponent<Button>();
		}

		ButtonClosePauseSubMenuLoad = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuLoad").GetComponent<Button>();
	}
}
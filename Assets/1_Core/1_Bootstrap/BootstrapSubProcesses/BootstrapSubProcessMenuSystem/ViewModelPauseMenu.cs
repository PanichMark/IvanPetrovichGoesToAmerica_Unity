using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseMenu : IViewModel
{
	public GameObject[] ButtonsPauseMenu;
	public GameObject TextCurrentMissionGoal;
	public TMP_Text TextPlayerMoneyNumber;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonsPauseMenu = new[]
	{
			bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuResume"),
			bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuSave"),
			bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuLoad"),
			bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuAppearance"),
			bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuTutorial"),
			bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuSettings"),
			bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuExitToMainMenu")
		};
		TextCurrentMissionGoal = bootstrap.FindDeepGameObject(canvas, "TextCurrentMissionGoal");
		TextPlayerMoneyNumber = canvas.transform.Find("TextPlayerMoneyNumber").GetComponent<TMP_Text>();
	}
}
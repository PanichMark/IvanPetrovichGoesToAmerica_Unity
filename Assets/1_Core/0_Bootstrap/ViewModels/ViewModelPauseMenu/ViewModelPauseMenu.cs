using UnityEngine;

public class ViewModelPauseMenu
{
	public GameObject[] ButtonsPauseMenu;
	public GameObject[] TextButtonsPauseMenu;

	public GameObject CurrentMission;
	public GameObject TextCurrentMissionGoal;
	public GameObject TextCurrentMissionGoalDisplay;

	public GameObject PlayerMoney;
	public GameObject TextCurrentPlayerMoney;
	public GameObject TextCurrentPlayerMoneyDisplay;

	public GameObject TextDeathMessage;

	public ViewModelPauseMenu(Bootstrap bootstrap, GameObject canvas)
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
		TextButtonsPauseMenu = new[]
		{
			bootstrap.FindDeepGameObject(canvas, "TextButtonPauseMenuResume"),
			bootstrap.FindDeepGameObject(canvas, "TextButtonPauseMenuSave"),
			bootstrap.FindDeepGameObject(canvas, "TextButtonPauseMenuLoad"),
			bootstrap.FindDeepGameObject(canvas, "TextButtonPauseMenuAppearance"),
			bootstrap.FindDeepGameObject(canvas, "TextButtonPauseMenuTutorial"),
			bootstrap.FindDeepGameObject(canvas, "TextButtonPauseMenuSettings"),
			bootstrap.FindDeepGameObject(canvas, "TextButtonPauseMenuExitToMainMenu")
		};

		CurrentMission = bootstrap.FindDeepGameObject(canvas, "CurrentMission");
		TextCurrentMissionGoal = bootstrap.FindDeepGameObject(canvas, "TextCurrentMissionGoal");
		TextCurrentMissionGoalDisplay = bootstrap.FindDeepGameObject(canvas, "TextCurrentMissionGoalDisplay");

		PlayerMoney = bootstrap.FindDeepGameObject(canvas, "PlayerMoney");
		TextCurrentPlayerMoney = bootstrap.FindDeepGameObject(canvas, "TextCurrentPlayerMoney");
		TextCurrentPlayerMoneyDisplay = bootstrap.FindDeepGameObject(canvas, "TextCurrentPlayerMoneyDisplay");

		TextDeathMessage = bootstrap.FindDeepGameObject(canvas, "TextDeathMessage");
	}
}
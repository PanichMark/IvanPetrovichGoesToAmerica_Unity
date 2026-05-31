using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuViewModel : IViewModel
{
	public Button ButtonResume;
	public Button ButtonSave;
	public Button ButtonLoad;
	public Button ButtonAppearance;
	public Button ButtonTutorial;
	public Button ButtonSettings;
	public Button ButtonExitToMainMenu;
	public TextMeshProUGUI TextCurrentMissionGoal;
	public TMP_Text TextPlayerMoneyNumber;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonResume = bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuResume").GetComponent<Button>();
		ButtonSave = bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuSave").GetComponent<Button>();
		ButtonLoad = bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuLoad").GetComponent<Button>();
		ButtonAppearance = bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuAppearance").GetComponent<Button>();
		ButtonTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuTutorial").GetComponent<Button>();
		ButtonSettings = bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuSettings").GetComponent<Button>();
		ButtonExitToMainMenu = bootstrap.FindDeepGameObject(canvas, "ButtonPauseMenuExitToMainMenu").GetComponent<Button>();
		TextCurrentMissionGoal = bootstrap.FindDeepGameObject(canvas, "TextCurrentMissionGoal").GetComponent<TextMeshProUGUI>();
		TextPlayerMoneyNumber = canvas.transform.Find("TextPlayerMoneyNumber").GetComponent<TMP_Text>();
	}
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuTutorial : IViewModel
{
	public GameObject ButtonClosePauseSubMenuTutorial;
	public GameObject ButtonNextTutorial;
	public GameObject ButtonPreviousTutorial;
	public GameObject TutorialNoteText;
	public GameObject TutorialNoteImage;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonClosePauseSubMenuTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuTutorial");
		ButtonNextTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonNextTutorial");
		ButtonPreviousTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonPreviousTutorial");
		TutorialNoteText = bootstrap.FindDeepGameObject(canvas, "TutorialNoteText");
		TutorialNoteImage = bootstrap.FindDeepGameObject(canvas, "TutorialNoteImage");
	}
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelPauseSubMenuTutorial : IViewModel
{
	public Button ButtonClosePauseSubMenuTutorial;
	public Button ButtonNextTutorial;
	public Button ButtonPreviousTutorial;
	public TextMeshProUGUI TutorialNoteText;
	public Image TutorialNoteImage;

	public void Initialize(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonClosePauseSubMenuTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuTutorial").GetComponent<Button>();
		ButtonNextTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonNextTutorial").GetComponent<Button>();
		ButtonPreviousTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonPreviousTutorial").GetComponent<Button>();
		TutorialNoteText = bootstrap.FindDeepGameObject(canvas, "TutorialNoteText").GetComponent<TextMeshProUGUI>();
		TutorialNoteImage = bootstrap.FindDeepGameObject(canvas, "TutorialNoteImage").GetComponent<Image>();
	}
}
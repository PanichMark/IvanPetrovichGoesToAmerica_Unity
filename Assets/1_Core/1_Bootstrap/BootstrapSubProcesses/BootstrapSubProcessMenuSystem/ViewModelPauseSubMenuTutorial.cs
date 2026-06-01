using UnityEngine;

public class ViewModelPauseSubMenuTutorial
{
	public GameObject ImageTutorial;
	public GameObject TextTutorial;

	public GameObject ButtonNextTutorial;
	public GameObject ButtonPreviousTutorial;

	public GameObject ButtonClosePauseSubMenuTutorial;
	public GameObject TextButtonClosePauseSubMenuTutorial;

	public ViewModelPauseSubMenuTutorial(Bootstrap bootstrap, GameObject canvas)
	{
		ImageTutorial = bootstrap.FindDeepGameObject(canvas, "ImageTutorial");
		TextTutorial = bootstrap.FindDeepGameObject(canvas, "TextTutorial");

		ButtonNextTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonNextTutorial");
		ButtonPreviousTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonPreviousTutorial");

		ButtonClosePauseSubMenuTutorial = bootstrap.FindDeepGameObject(canvas, "ButtonClosePauseSubMenuTutorial");
		TextButtonClosePauseSubMenuTutorial = bootstrap.FindDeepGameObject(canvas, "TextButtonClosePauseSubMenuTutorial");
	}
}
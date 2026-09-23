using UnityEngine;

public class ViewModelMenuCutscene
{
	public GameObject TextCutsceneDialogue;

	public GameObject BlackLineUp;
	public GameObject BlackLineDown;

	public ViewModelMenuCutscene(Bootstrap bootstrap, GameObject canvas)
	{
		TextCutsceneDialogue = bootstrap.FindDeepGameObject(canvas, "TextCutsceneDialogue");

		BlackLineUp = bootstrap.FindDeepGameObject(canvas, "BlackLineUp");
		BlackLineDown = bootstrap.FindDeepGameObject(canvas, "BlackLineDown");
	}
}
using UnityEngine;

public class ViewModelMenuCutscene
{
	public GameObject TextCutsceneDialogue;

	public GameObject BlackLineUp;
	public GameObject BlackLineDown;

	public GameObject TextSkipCutscene;
	public GameObject CircleSkipCutscene;

	public ViewModelMenuCutscene(Bootstrap bootstrap, GameObject canvas)
	{
		TextCutsceneDialogue = bootstrap.FindDeepGameObject(canvas, "TextCutsceneDialogue");

		BlackLineUp = bootstrap.FindDeepGameObject(canvas, "BlackLineUp");
		BlackLineDown = bootstrap.FindDeepGameObject(canvas, "BlackLineDown");

		TextSkipCutscene = bootstrap.FindDeepGameObject(canvas, "TextSkipCutscene");
		CircleSkipCutscene = bootstrap.FindDeepGameObject(canvas, "CircleSkipCutscene");
	}
}
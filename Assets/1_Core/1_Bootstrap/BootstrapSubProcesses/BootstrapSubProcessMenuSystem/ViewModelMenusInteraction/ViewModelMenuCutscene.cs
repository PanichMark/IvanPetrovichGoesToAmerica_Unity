using UnityEngine;

public class ViewModelMenuCutscene
{
	public GameObject TextCutsceneDialogue;

	public ViewModelMenuCutscene(Bootstrap bootstrap, GameObject canvas)
	{
		TextCutsceneDialogue = bootstrap.FindDeepGameObject(canvas, "TextCutsceneDialogue");
	}
}
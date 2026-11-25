using UnityEngine;

public class ViewModelMenuDialogue
{
	public GameObject TextDialogueLine;
	public GameObject ButtonDialogueYes;
	public GameObject ButtonDialogueNo;
	public GameObject TextDialogueYes;
	public GameObject TextDialogueNo;

	public ViewModelMenuDialogue(Bootstrap bootstrap, GameObject canvas)
	{
		
		TextDialogueLine = bootstrap.FindDeepGameObject(canvas, "TextDialogue");
		
		ButtonDialogueYes = bootstrap.FindDeepGameObject(canvas, "ButtonDialogueYes");
		ButtonDialogueNo = bootstrap.FindDeepGameObject(canvas, "ButtonDialogueNo");
		TextDialogueYes = bootstrap.FindDeepGameObject(canvas, "TextDialogueYes");
		TextDialogueNo = bootstrap.FindDeepGameObject(canvas, "TextDialogueNo");
	}
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelMenuDialogue
{
	public TextMeshProUGUI TextDialogueLine;
	public Button ButtonDialogueYes;
	public Button ButtonDialogueNo;
	public GameObject TextDialogueYes;
	public GameObject TextDialogueNo;

	public ViewModelMenuDialogue(Bootstrap bootstrap, GameObject canvas)
	{
		TextDialogueLine = canvas.transform.Find("TextDialogue").GetComponent<TextMeshProUGUI>();

		ButtonDialogueYes = canvas.transform.Find("ButtonDialogueYes").GetComponent<Button>();
		ButtonDialogueNo = canvas.transform.Find("ButtonDialogueNo").GetComponent<Button>();
		TextDialogueYes = bootstrap.FindDeepGameObject(canvas, "TextDialogueYes");
		TextDialogueNo = bootstrap.FindDeepGameObject(canvas, "TextDialogueNo");
	}
}

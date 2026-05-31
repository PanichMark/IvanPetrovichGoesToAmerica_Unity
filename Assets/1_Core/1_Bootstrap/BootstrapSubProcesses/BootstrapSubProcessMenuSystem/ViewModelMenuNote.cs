using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewModelMenuNote
{
	public TextMeshProUGUI TextNote;
	public Image ImageNote;
	public Image ImageNoteBlackBackground;
	public Button ButtonCloseMenuNote;

	public ViewModelMenuNote(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseMenuNote = canvas.transform.Find("ButtonExitReadNoteMenu").GetComponent<Button>();
		ImageNote = canvas.transform.Find("ImageNote").GetComponent<Image>();
		TextNote = canvas.transform.Find("TextNote").GetComponent<TextMeshProUGUI>();
		ImageNoteBlackBackground = canvas.transform.Find("ImageNoteBlackBackground").GetComponent<Image>();
	}
}

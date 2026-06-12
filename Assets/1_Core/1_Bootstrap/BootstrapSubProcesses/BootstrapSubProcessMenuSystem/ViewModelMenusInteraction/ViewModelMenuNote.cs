using UnityEngine;

public class ViewModelMenuNote
{
	public GameObject TextNote;
	public GameObject ImageNote;
	public GameObject ImageNoteBlackBackground;
	public GameObject ButtonCloseMenuNote;
	public GameObject TextButtonCloseMenuNote;

	public ViewModelMenuNote(Bootstrap bootstrap, GameObject canvas)
	{
		ButtonCloseMenuNote = bootstrap.FindDeepGameObject(canvas, "ButtonExitReadNoteMenu");
		TextButtonCloseMenuNote = bootstrap.FindDeepGameObject(canvas, "TextButtonExitReadNoteMenu");
		ImageNote = bootstrap.FindDeepGameObject(canvas, "ImageNote");
		TextNote = bootstrap.FindDeepGameObject(canvas, "TextNote");
		ImageNoteBlackBackground = bootstrap.FindDeepGameObject(canvas, "ImageNoteBlackBackground");
	}
}
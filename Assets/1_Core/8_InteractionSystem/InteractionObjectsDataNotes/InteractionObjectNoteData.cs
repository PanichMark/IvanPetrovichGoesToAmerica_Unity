using UnityEngine;

[CreateAssetMenu(fileName = "NoteData", menuName = "InteractionObjects/Notes/NoteData", order = 1)]
public class InteractionObjectNoteData : ScriptableObject
{
	public TextAsset NoteText_RU;
	public TextAsset NoteText_EN;
	public Sprite NoteImage;
	public InteractionObjectNotePosition NotePosition;
	public bool IsNoteToGlanceAt;
}
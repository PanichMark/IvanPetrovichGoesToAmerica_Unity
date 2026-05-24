using UnityEngine;

[CreateAssetMenu(fileName = "InteractionObjectNoteData", menuName = "InteractionObjects/Notes/InteractionObjectNoteData", order = 1)]
public class InteractionObjectNoteData : ScriptableObject
{
	public TextAsset NoteText_RU;
	public TextAsset NoteText_EN;
	public Sprite NoteImage;
}
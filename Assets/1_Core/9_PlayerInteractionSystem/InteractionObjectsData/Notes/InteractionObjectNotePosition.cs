using UnityEngine;

[CreateAssetMenu(fileName = "NotePosition", menuName = "InteractionObjects/Notes/NotePosition")]
public class InteractionObjectNotePosition : ScriptableObject
{
	public Vector3 ImagePosition;
	public Vector2 ImageRotation;
	public float ImageWidth;
	public float ImageHeight;
	public Vector3 TextPosition;
	public Vector2 TextRotation;
	public float TextWidth;
	public float TextHeight;
}

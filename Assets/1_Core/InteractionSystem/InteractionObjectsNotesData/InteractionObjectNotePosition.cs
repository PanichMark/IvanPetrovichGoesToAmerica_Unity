using UnityEngine;

[CreateAssetMenu(fileName = "InteractionObjectNotePosition", menuName = "InteractionObjects/Notes/InteractionObjectNotePosition")]
public class InteractionObjectNotePosition : ScriptableObject
{
	public Vector3 ImagePosition;
	public Vector2 ImageRotation;
	public Vector3 TextPosition;
	public Vector2 TextRotation;
	public bool IsThereText;
}

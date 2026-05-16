using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TutorialNotesList", menuName = "PauseMenu/TutorialSubMenu/TutorialNotesList", order = 0)]
public class TutorialNotesList : ScriptableObject
{
	public List<NoteData> Notes = new List<NoteData>();
}
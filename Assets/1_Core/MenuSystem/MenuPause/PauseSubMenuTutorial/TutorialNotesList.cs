using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TutorialNotesList", menuName = "PauseMenu/TutorialNotesList", order = 0)]
public class TutorialNotesList : ScriptableObject
{
	public List<InteractionObjectNoteData> Notes = new List<InteractionObjectNoteData>();
}
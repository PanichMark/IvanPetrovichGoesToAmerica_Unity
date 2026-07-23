using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameTutorialsList", menuName = "PauseMenu/GameTutorialsList", order = 0)]
public class GameTutorialsList : ScriptableObject
{
	public List<InteractionObjectNoteData> Notes = new List<InteractionObjectNoteData>();
}
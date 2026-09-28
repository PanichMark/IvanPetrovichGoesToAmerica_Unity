using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameDifficultiesList", menuName = "Bootstrap/GameData/GameDifficultiesList", order = 0)]
public class GameDifficultiesList : ScriptableObject
{
	public List<InteractionObjectNoteData> Notes = new List<InteractionObjectNoteData>();
}
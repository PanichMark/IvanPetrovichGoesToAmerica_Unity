using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameScenesList", menuName = "Bootstrap/GameData/GameScenes/GameScenesList")]
public class GameScenesList : ScriptableObject
{
	public List<GameSceneData> GameScenes = new List<GameSceneData>();
}

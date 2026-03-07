using UnityEngine;

[CreateAssetMenu(fileName = "ConfigScene", menuName = "Scriptable Objects/Configs/ConfigScene")]
public class ConfigScene : ScriptableObject
{
	public GameScenesEnum selectedScene;
}

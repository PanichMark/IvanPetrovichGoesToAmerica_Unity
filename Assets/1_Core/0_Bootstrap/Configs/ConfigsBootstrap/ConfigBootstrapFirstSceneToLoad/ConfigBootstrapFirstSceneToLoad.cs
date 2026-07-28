using UnityEngine;

[CreateAssetMenu(fileName = "ConfigBootstrapFirstSceneToLoad", menuName = "Configs/Bootstrap/FirstSceneToLoad")]
public class ConfigBootstrapFirstSceneToLoad : ScriptableObject
{
	public GameScenesEnum FirstSceneToLoad;
}

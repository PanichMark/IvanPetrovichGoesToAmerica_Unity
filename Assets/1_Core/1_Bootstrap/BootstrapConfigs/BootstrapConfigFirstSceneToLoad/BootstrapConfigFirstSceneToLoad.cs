using UnityEngine;

[CreateAssetMenu(fileName = "FirstSceneToLoad", menuName = "BootstrapConfigs/FirstSceneToLoad")]
public class BootstrapConfigFirstSceneToLoad : ScriptableObject
{
	public GameScenesEnum FirstSceneToLoad;
}

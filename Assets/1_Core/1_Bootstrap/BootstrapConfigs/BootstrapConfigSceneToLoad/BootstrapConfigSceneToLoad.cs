using UnityEngine;

[CreateAssetMenu(fileName = "SceneToLoad", menuName = "BootstrapConfigs/SceneToLoad")]
public class BootstrapConfigSceneToLoad : ScriptableObject
{
	public GameScenesEnum SelectedScene;
}

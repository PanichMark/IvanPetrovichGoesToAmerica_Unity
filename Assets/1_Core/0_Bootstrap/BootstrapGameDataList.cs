using UnityEngine;

[CreateAssetMenu(fileName = "BootstrapGameDataList", menuName = "BootstrapConfigs/BootstrapGameDataList")]
public class BootstrapGameDataList : ScriptableObject
{
    public GameCanvasesList GameCanvasesList;
	public GameScenesList GameScenesList;
	public GameMissionsList GameMissionsList;
	public GameTutorialsList GameTutorialsList;
    public GameDifficultiesList GameDifficultiesList;
}

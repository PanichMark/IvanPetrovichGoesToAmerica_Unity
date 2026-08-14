using UnityEngine;

[CreateAssetMenu(fileName = "BootstrapGameDataList", menuName = "Configs/GameData/BootstrapGameDataList")]
public class BootstrapGameDataList : ScriptableObject
{
	public int NumberOfSafeFileSlots;
	public TextAsset LocalizationMain;
	public TermsAndConditions TermsAndConditions;
    public GameCanvasesList GameCanvasesList;
	public GameScenesList GameScenesList;
	public GameMissionsList GameMissionsList;
	public GameObjectPoolsList GameObjectPoolsList;
	public GameTutorialsList GameTutorialsList;
    public GameDifficultiesList GameDifficultiesList;
}

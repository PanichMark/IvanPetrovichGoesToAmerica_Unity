using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "BootstrapGameDataList", menuName = "Configs/GameData/BootstrapGameDataList")]
public class BootstrapGameDataList : ScriptableObject
{
	public int NumberOfSafeFileSlots { get; private set; } = 20;
	public List<Sprite> NPCdetectionSignFrames;
	public AudioMixer AudioMixer;
	public TextAsset LocalizationMain;
	public TermsAndConditions TermsAndConditions;
    public GameCanvasesList GameCanvasesList;
	public GameScenesList GameScenesList;
	public GameMissionsList GameMissionsList;
	public GameObjectPoolsList GameObjectPoolsList;
	public GameTutorialsList GameTutorialsList;
    public GameDifficultiesList GameDifficultiesList;
}

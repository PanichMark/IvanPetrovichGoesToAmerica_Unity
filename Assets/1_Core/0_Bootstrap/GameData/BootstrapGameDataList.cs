using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "BootstrapGameDataList", menuName = "Bootstrap/GameData/BootstrapGameDataList")]
public class BootstrapGameDataList : ScriptableObject
{
	public int NumberOfSafeFileSlots { get; private set; } = 20;
	public int NumberOfInventoryRows { get; private set; } = 5;
	public int NumberOfInventorySlots { get; private set; } = 20;
	public List<Sprite> NPCdetectionSignFrames;
	public AudioMixer AudioMixer;
	public TextAsset LocalizationMain;
	public TermsAndConditions TermsAndConditions;
	public GameNews GameNews;
	public UniversalRendererData RendererMain;
	public UniversalRendererData RendererFirstPerson;
	public GameCanvasesList GameCanvasesList;
	public GameScenesList GameScenesList;
	public GameMissionsList GameMissionsList;
	public MissionMapsList MissionMapsList;
	public GameObjectPoolsList GameObjectPoolsList;
	public GameTutorialsList GameTutorialsList;
	public GameDifficultiesList GameDifficultiesList;
	public InteractionObjectsKeyItemsList KeyItemsList;
}

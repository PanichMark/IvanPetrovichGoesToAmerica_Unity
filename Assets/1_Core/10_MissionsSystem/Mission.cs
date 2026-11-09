using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Mission", menuName = "Missions/Mission")]

public class Mission : ScriptableObject
{
	[SerializeField] private GameMissionsNamesEnum _missionName;
	[SerializeField] private MissionResourcesData _missionResources;
	[SerializeField] private Sprite _missionTargetSprite;
	[SerializeField] private TextAsset _missionDescription_RU;
	[SerializeField] private TextAsset _missionDescription_EN;

	[SerializeField] private List<InteractionObjectNoteData> _missionMaps;

	public string MissionName => _missionName.ToString();

	[TextArea(3, 10)]
	public string MissionDescription;

	public MissionResourcesData MissionResources => _missionResources;
	public MissionStep[] MissionSteps;

	public Sprite MissionTargetSprite => _missionTargetSprite;
	public TextAsset MissionDescription_RU => _missionDescription_RU;
	public TextAsset MissionDescription_EN => _missionDescription_EN;

	public List<InteractionObjectNoteData> MissionMaps => _missionMaps;
}
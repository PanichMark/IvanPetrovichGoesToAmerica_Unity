using UnityEngine;

[CreateAssetMenu(fileName = "NPCbodyType", menuName = "NPC/ConfigsNPC/NPCbodyType")]
public class ConfigNPCBodyType : ScriptableObject
{
    public NPCSexTypes NPCsexType;
    public NPCPhysiqueTypes NPCphysique;
	[Range(0.7f, 1.2f)] public float NPCspeedMultiplier;
}

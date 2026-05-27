using UnityEngine;

[CreateAssetMenu(fileName = "ConfigNPCbodyType", menuName = "ConfigsNPC/ConfigNPCbodyType")]
public class ConfigNPCBodyType : ScriptableObject
{
    public NPCSexTypes NPCsexType;
    public NPCPhysiqueTypes NPCphysique;
	[Range(0.7f, 1.2f)] public float NPCspeedMultiplier;
}

using UnityEngine;

[CreateAssetMenu(fileName = "NPCconfigBodyType", menuName = "Scriptable Objects/NPCconfigBodyType")]
public class NPCconfigBodyType : ScriptableObject
{
    public NPCsexTypes NPCSexTypes;
    public NPCphysiqueTypes NPCBodyType;
	[Range(0f, 1f)] public float NPCspeedMultiplier;
}

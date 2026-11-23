using UnityEngine;

[CreateAssetMenu(fileName = "NPCphrasesDataNonPeaceful", menuName = "NPC/NPCphrasesDataTypes/NonPeaceful")]
public class NPCphrasesDataNonPeaceful : ScriptableObject
{
	public NPCphrasesData PhrasesDefault;
	public NPCphrasesData PhrasesInterested;
	public NPCphrasesData PhrasesAlarmed;
	public NPCphrasesData PhrasesChasing;
	public NPCphrasesData PhrasesAttacking;
	public NPCphrasesData PhrasesSeaching;
}

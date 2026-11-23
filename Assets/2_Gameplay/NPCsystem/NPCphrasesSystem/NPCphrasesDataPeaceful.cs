using UnityEngine;

[CreateAssetMenu(fileName = "NPCphrasesDataPeaceful", menuName = "NPC/NPCphrasesDataPeaceful")]
public class NPCphrasesDataPeaceful : ScriptableObject
{
	public NPCphrasesData PhrasesDefault;
	public NPCphrasesData PhrasesInterested;
	public NPCphrasesData PhrasesAlarmed;
	public NPCphrasesData PhrasesAfraid;
}

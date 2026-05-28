using UnityEngine;

[CreateAssetMenu(fileName = "NPCphrasesData", menuName = "NPC/NPCphrasesData")]
public class NPCPhrasesData : ScriptableObject
{
	public TextAsset PhrasesFileRussian;
	public AudioClip[] PhrasesVoicelinesRussian;

	public TextAsset PhrasesFileEnglish;
	public AudioClip[] PhrasesVoicelinesEnglish;
}

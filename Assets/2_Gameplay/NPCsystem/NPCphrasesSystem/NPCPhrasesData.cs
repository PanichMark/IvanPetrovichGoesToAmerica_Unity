using UnityEngine;

[CreateAssetMenu(fileName = "NPCphrasesData", menuName = "NPC/NPCphrasesData")]
public class NPCphrasesData : ScriptableObject
{
	public TextAsset PhrasesTextFileRussian;
	public AudioClip[] PhrasesVoicelinesRussian;

	public TextAsset PhrasesTextFileEnglish;
	public AudioClip[] PhrasesVoicelinesEnglish;
}

using UnityEngine;

[CreateAssetMenu(fileName = "NPCdialogueData", menuName = "NPC/NPCdialogueData")]
public class NPCdialogueData : ScriptableObject
{
	public TextAsset DialogueTextfileRussian;
	public AudioClip[] DialogueVoicelinesRussian;

	public TextAsset DialogueTextfileEnglish;
	public AudioClip[] DialogueVoicelinesEnglish;
}

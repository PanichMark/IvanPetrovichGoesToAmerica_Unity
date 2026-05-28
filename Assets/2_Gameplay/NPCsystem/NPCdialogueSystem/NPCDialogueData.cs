using UnityEngine;

[CreateAssetMenu(fileName = "NPCdialogueData", menuName = "NPC/NPCdialogueData")]
public class NPCDialogueData : ScriptableObject
{
	public TextAsset DialogueTextfileRussian;
	public AudioClip[] DialogueVoicelinesRussian;

	public TextAsset DialogueTextfileEnglish;
	public AudioClip[] DialogueVoicelinesEnglish;
}

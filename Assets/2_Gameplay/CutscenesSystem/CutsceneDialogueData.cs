using UnityEngine;

[CreateAssetMenu(fileName = "CutsceneDialogueData", menuName = "Cutscenes/CutsceneDialogueData")]
public class CutsceneDialogueData : ScriptableObject
{
	public TextAsset CutsceneDialogueFileRussian;
	public AudioClip[] CutsceneVoicelinesRussian;

	public TextAsset CutsceneDialogueFileEnglish;
	public AudioClip[] CutsceneVoicelinesEnglish;
}
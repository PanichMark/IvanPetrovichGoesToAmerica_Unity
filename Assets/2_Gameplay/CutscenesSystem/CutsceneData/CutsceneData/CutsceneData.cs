using UnityEngine;

[CreateAssetMenu(fileName = "CutsceneData", menuName = "Cutscenes/CutsceneData")]
public class CutsceneData : ScriptableObject
{
	public TextAsset CutsceneTextFileRussian;
	public AudioClip[] CutsceneVoicelinesRussian;

	public TextAsset CutsceneTextFileEnglish;
	public AudioClip[] CutsceneVoicelinesEnglish;
}
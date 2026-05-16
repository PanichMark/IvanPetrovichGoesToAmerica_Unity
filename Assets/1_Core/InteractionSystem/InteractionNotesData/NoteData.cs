using UnityEngine;

[CreateAssetMenu(fileName = "NoteData", menuName = "PauseMenu/TutorialSubMenu/NoteData", order = 1)]
public class NoteData : ScriptableObject
{
	public TextAsset Text_RU;
	public TextAsset Text_EN;
	public Sprite Image;
}